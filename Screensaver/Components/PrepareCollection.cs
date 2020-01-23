using HlyssUI.Components;
using HlyssUI.Graphics;
using HlyssUI.Layout;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Screensaver.Components
{
    class PrepareCollection : Component
    {
        private bool _collectionPrepared = false;

        public PrepareCollection()
        {
            Width = "100%";
            Height = "100%";
            Padding = "20px";
            Layout = LayoutType.Column;
            Children = new List<Component>()
            {
                new Label("Przygotowywanie kolekcji zdjęć")
                {
                    Font = Fonts.MontserratSemiBold,
                    MarginBottom = "10px"
                },
                new Label("To może trochę potrwać..."),
                new Spacer(),
                new ProgressBar()
                {
                    Width = "100%",
                    Intermediate = true
                }
            };

            SlideShowConfig config = new SlideShowConfig();
            config.Load();

            Task searchTask = Task.Run(() => Search(config.Directory, config.UseSubdirectories));
        }

        public void Search(string path, bool useSubdirectories)
        {
            ImageSearcher searcher = new ImageSearcher();
            searcher.Search(path, useSubdirectories);

            if (File.Exists(Program.PATH + "imagelist.cfg"))
                File.Delete(Program.PATH + "imagelist.cfg");

            File.WriteAllLines(Program.PATH + "imagelist.cfg", searcher.Images.ToArray());
            _collectionPrepared = true;
        }

        public override void Update()
        {
            base.Update();

            if(_collectionPrepared)
            {
                App.Window.Close();
            }
        }
    }
}
