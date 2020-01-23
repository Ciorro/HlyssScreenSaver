using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Screensaver
{
    class ImageSearcher
    {
        public List<string> Images { get; private set; } = new List<string>();

        public List<string> Extensions = new List<string>()
        {
            ".jpg", ".png", ".bmp"
        };

        public void Search(string location, bool useSubdirectories)
        {
            string[] files = Directory.GetFiles(location);
            string[] directories = Directory.GetDirectories(location);

            foreach (var file in files)
            {
                if(Extensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    Images.Add(file);
                }
            }

            foreach (var directory in directories)
            {
                if (useSubdirectories)
                    Search(directory, useSubdirectories);
            }
        }
    }
}
