using HlyssUI.Components;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Screensaver.Components
{
    class SlideShow : Component
    {
        private readonly SlideShowConfig _config;
        private readonly string[] _images;
        private readonly Stopwatch _stopwatch;
        private Texture _texture;
        private Random _randomizer = new Random();

        private int _currentImgId = 0;

        public SlideShow()
        {
            if (!(File.Exists(Program.PATH + "settings.ini") && File.Exists(Program.PATH + "imagelist.cfg")))
            {
                StartConfig();
            }

            _config = new SlideShowConfig();
            _config.Load();
            _images = File.ReadAllLines(Program.PATH + "imagelist.cfg");

            if (_images.Length < 1)
                StartConfig();

            Width = "100%";
            Height = "100%";

            Children = new List<Component>()
            {
                new PictureBox(_images[0])
                {
                    Width = "100%",
                    Height = "100%",
                    StretchMode = PictureBox.Stretch.Letterbox,
                    Name = "picturebox",
                    SmoothImage = true
                }
            };

            _stopwatch = Stopwatch.StartNew();
        }

        public override void OnInitialized()
        {
            base.OnInitialized();

            App.Window.KeyPressed += Window_KeyPressed;
            App.Window.MouseMoved += Window_MouseMoved;
        }

        private void Window_MouseMoved(object sender, SFML.Window.MouseMoveEventArgs e)
        {
            if ((DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()).TotalSeconds > 2)
            {
                App.Window.Close();
            }
        }

        private void Window_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Right)
            {
                NextImage();
            }
            else if (e.Code == Keyboard.Key.Left)
            {
                PreviousImage();
            }
            else
            {
                App.Window.Close();
            }

            _stopwatch.Restart();
        }

        private void StartConfig()
        {
            Console.WriteLine("Config not found!");

            string path = AppDomain.CurrentDomain.BaseDirectory;
            string name = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
            string extension = ".scr";

            try
            {
                Process.Start($"{path}{name}{extension}");
                Environment.Exit(0);
            }
            catch { }
        }

        public override void Update()
        {
            base.Update();

            if (_stopwatch.ElapsedMilliseconds > _config.GetInterval())
            {
                _stopwatch.Restart();

                if (!_config.Random)
                    NextImage();
                else
                    RandomImage();
            }

            (FindChild("picturebox") as PictureBox).ForceRefresh();
        }

        private void RandomImage()
        {
            _currentImgId = _randomizer.Next(0, _images.Length);

            if (_currentImgId < _images.Length)
                LoadTexture(_currentImgId);

            UpdateImage();
        }

        private void NextImage()
        {
            _currentImgId++;

            if (_currentImgId < _images.Length)
                LoadTexture(_currentImgId);

            UpdateImage();
        }

        private void PreviousImage()
        {
            _currentImgId--;

            if (_currentImgId >= 0)
                LoadTexture(_currentImgId);

            UpdateImage();
        }

        private void UpdateImage()
        {
            (FindChild("picturebox") as PictureBox).Image = _texture;
        }

        private void LoadTexture(int id)
        {
            if (_texture != null)
                _texture.Dispose();

            _texture = new Texture(_images[_currentImgId]);
        }
    }
}
