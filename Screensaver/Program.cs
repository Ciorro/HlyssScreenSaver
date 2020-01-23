using HlyssUI;
using HlyssUI.Components;
using HlyssUI.Graphics;
using HlyssUI.Layout;
using HlyssUI.Themes;
using Screensaver.Components;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace Screensaver
{
    class Program
    {
        public static string PATH
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                if (args[0].ToLower().StartsWith("/c"))
                    ShowSettings();
                if (args[0].ToLower().StartsWith("/s"))
                    ShowScreensaver();
            }
            else
            {
                ShowSettings();
            }
        }

        static void ShowScreensaver()
        {
            ContextSettings contextSettings = new ContextSettings()
            {
                AntialiasingLevel = 8
            };
            RenderWindow window = new RenderWindow(new VideoMode(), "Ustawienia pokazu slajdów", Styles.Fullscreen, contextSettings);
            window.SetFramerateLimit(20);
            window.SetMouseCursorVisible(false);
            window.Closed += (object sender, EventArgs e) => { window.Close(); };

            Theme.Load(PATH + "theme.ini", "light");
            HlyssApp app = new HlyssApp(window);

            app.Root.Children.Add(new SlideShow());

            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();

                app.Update();
                app.Draw();

                window.Display();
            }
        }

        static void ShowSettings()
        {
            ContextSettings contextSettings = new ContextSettings()
            {
                AntialiasingLevel = 8
            };
            RenderWindow window = new RenderWindow(new VideoMode(400, 230), "Ustawienia pokazu slajdów", Styles.Close, contextSettings);
            window.SetFramerateLimit(60);
            window.Closed += (object sender, EventArgs e) => { window.Close(); };

            Theme.Load(PATH + "theme.ini", "light");
            HlyssApp app = new HlyssApp(window);

            app.Root.Children.Add(new SettingsComponent());

            while(window.IsOpen)
            {
                window.Clear(Theme.GetColor("primary"));
                window.DispatchEvents();

                app.Update();
                app.Draw();

                window.Display();
            }
        }
    }
}
