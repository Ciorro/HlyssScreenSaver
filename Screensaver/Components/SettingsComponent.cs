using HlyssUI.Components;
using HlyssUI.Graphics;
using HlyssUI.Layout;
using System;
using System.Collections.Generic;

namespace Screensaver.Components
{
    class SettingsComponent : Component
    {
        public SettingsComponent()
        {
            Width = "100%";
            Height = "100%";
            Padding = "20px";
            Layout = LayoutType.Column;
            Children = new List<Component>()
            {
                new Component()
                {
                    Width = "100%",
                    AutosizeY = true,
                    CenterContent = true,
                    Children = new List<Component>()
                    {
                        new Label("Użyj obrazów z:")
                        {
                            Font = Fonts.MontserratSemiBold
                        },
                        new Spacer(),
                        new Button("Przeglądaj...")
                        {
                            Padding = "4px 7px",
                            Name = "browse_btn"
                        },
                        new FolderBrowser()
                        {
                            Name = "folder_browser"
                        }
                    },
                },
                new Label("Moje obrazy")
                {
                    MarginBottom = "10px",
                    Name = "current_dir"
                },
                new CheckBox("Użyj obrazów z lokalizacji podrzędnych")
                {
                    Name = "use_subdirectories"
                },
                new Divider()
                {
                    Margin = "5px 0px"
                },
                new Label("Prędkość pokazu slajdów:")
                {
                    Font = Fonts.MontserratSemiBold
                },
                new Component()
                {
                    Width = "100%",
                    AutosizeY = true,
                    CenterContent = true,
                    Layout = LayoutType.Row,
                    Name = "speed_select",
                    Children = new List<Component>()
                    {
                        new RadioButton("Wolno")
                        {
                            Margin = "5px 10px",
                            MarginLeft = "0px"
                        },
                        new RadioButton("Średnio")
                        {
                            Margin = "5px 10px"
                        },
                        new RadioButton("Szybko")
                        {
                            Margin = "5px 10px"
                        },
                    }
                },
                new Divider()
                {
                    Margin = "5px 0px"
                },
                new CheckBox("Losuj obrazy")
                {
                    Name = "randomise"
                },
                new Component()
                {
                    Width = "100%",
                    AutosizeY = true,
                    Reversed = true,
                    Children = new List<Component>()
                    {
                        new Button("Zapisz")
                        {
                            Appearance = Button.ButtonStyle.Filled,
                            Name = "ok_btn"
                        },
                        new Button("Anuluj")
                        {
                            MarginRight = "5px",
                            Name = "close_btn"
                        }
                    }
                }
            };

            FindChild("browse_btn").Clicked += (object sender) => 
            {
                (FindChild("folder_browser") as FolderBrowser).Show();    
            };

            FindChild("close_btn").Clicked += (object sender) =>
            {
                App.Window.Close();
            };

            FindChild("ok_btn").Clicked += (object sender) =>
            {
                int intervalId = 0;

                for (int i = 0; i < 3; i++)
                {
                    if ((FindChild("speed_select").Children[i] as RadioButton).IsMarked)
                        intervalId = i;
                }

                SlideShowConfig config = new SlideShowConfig()
                {
                    Directory = (FindChild("current_dir") as Label).Text,
                    Random = (FindChild("randomise") as CheckBox).IsChecked,
                    UseSubdirectories = (FindChild("use_subdirectories") as CheckBox).IsChecked,
                    IntervalId = intervalId
                };

                config.Save();

                App.Root.Children.Clear();
                App.Root.Children.Add(new PrepareCollection());
            };

            (FindChild("folder_browser") as FolderBrowser).OnFileSelected += (object sender, FolderBrowser.FSEntry entry) =>
            {
                (FindChild("current_dir") as Label).Text = entry.FullPath;
            };

            LoadSettings();
        }

        private void LoadSettings()
        {
            SlideShowConfig config = new SlideShowConfig();
            config.Load();

            (FindChild("current_dir") as Label).Text = config.Directory;
            (FindChild("use_subdirectories") as CheckBox).IsChecked = config.UseSubdirectories;
            (FindChild("speed_select").Children[config.IntervalId] as RadioButton).IsMarked = true;
            (FindChild("randomise") as CheckBox).IsChecked = config.Random;
        }
    }
}

