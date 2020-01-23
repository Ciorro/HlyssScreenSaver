using HlyssUI;
using HlyssUI.Components;
using HlyssUI.Graphics;
using HlyssUI.Layout;
using HlyssUI.Styling;
using HlyssUI.Themes;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Screensaver.Components
{
    class FolderBrowser : Component
    {
        public struct FSEntry
        {
            public string Name;
            public string FullPath;
            public bool IsDirectory;
            public FileAttributes Attributes;

            public FSEntry(string path)
            {
                Name = Path.GetFileName(path);
                FullPath = path;
                Attributes = File.GetAttributes(path);
                IsDirectory = Attributes.HasFlag(FileAttributes.Directory);
            }
        }

        public delegate void FileSelectedHandler(object sender, FSEntry entry);
        public event FileSelectedHandler OnFileSelected;

        private HlyssApp _internalApp;
        private bool _shouldClose = false;
        private string _currentDirectory = string.Empty;
        private List<FSEntry> _entries = new List<FSEntry>();

        private string CurrentPath
        {
            get { return (_internalApp.Root.FindChild("directory_box") as TextBox).Text; }
            set { (_internalApp.Root.FindChild("directory_box") as TextBox).Text = value; }
        }

        public RenderWindow Window { get; private set; }

        public bool IsOpen
        {
            get { return Window != null && Window.IsOpen; }
        }

        public string StartDirectory { get; set; } = "C:\\";

        public void Show()
        {
            if (!IsOpen)
            {
                Window = new RenderWindow(new VideoMode(550, 400), "Przeglądanie w poszukiwaniu folderu");
                Window.Closed += Window_Closed;
                Window.SetFramerateLimit(60);
                Initialize();
            }

            Visible = true;
        }

        public void Close()
        {
            if (IsOpen)
            {
                Window.Close();
                Window.Dispose();
                Window = null;
            }

            Visible = false;
        }

        public override void Update()
        {
            base.Update();

            if (IsOpen)
            {
                Window.Clear(Theme.GetColor("primary"));
                Window.DispatchEvents();

                _internalApp.Update();
                _internalApp.Draw();

                Window.Display();

                if (_shouldClose)
                {
                    Close();
                    _shouldClose = false;
                }
            }
        }

        public void Navigate(string path)
        {
            if (Directory.Exists(path))
            {
                _currentDirectory = path;
                CurrentPath = path;

                _entries.Clear();

                foreach (var file in Directory.GetFileSystemEntries(path))
                {
                    FSEntry fsEntry = new FSEntry(file);

                    if (fsEntry.Attributes.HasFlag(FileAttributes.Hidden) == false
                        && fsEntry.Attributes.HasFlag(FileAttributes.System) == false
                        && fsEntry.IsDirectory)
                    {
                        _entries.Add(fsEntry);
                    }
                }

                RefreshList();
            }
        }

        private void RefreshList()
        {
            Component list = _internalApp.Root.FindChild("files_panel");
            list.ScrollY(0);
            list.Children.Clear();

            foreach (var entry in _entries)
            {
                list.Children.Add(new FileEntry(this, entry));
            }
        }

        private void Initialize()
        {
            string style = @"<hss><style name=""drives""><size-ease>instant</size-ease></style></hss>";
            StyleBank.LoadFromString(style);

            _internalApp = new HlyssApp(Window);

            _internalApp.Root.Children.Add(new Component()
            {
                Width = "100%",
                Height = "100%",
                Padding = "10px",
                Layout = LayoutType.Column,
                Children = new List<Component>()
                {
                    new Component()
                    {
                        Width = "100%",
                        AutosizeY = true,
                        CenterContent = true,
                        Children = new List<Component>()
                        {
                            new Button()
                            {
                                Padding = "10px",
                                Children = new List<Component>()
                                {
                                    new Icon(Icons.AngleUp)
                                },
                                Name = "up_btn"
                            },
                            new TextBox()
                            {
                                Expand = true,
                                Margin = "0px 10px",
                                Height = "0px",
                                Name = "directory_box"
                            },
                            new Dropdown()
                            {
                                Width = "100px",
                                Name = "drives",
                                Items = DriveInfo.GetDrives().Select(o => o.Name).ToList()
                            }
                        }
                    },
                    new Panel()
                    {
                        Width = "100%",
                        MarginTop = "10px",
                        Padding = "5px 1px",
                        Expand = true,
                        Name = "files_panel",
                        Layout = LayoutType.Column,
                        Overflow = OverflowType.Scroll
                    },
                    new Component()
                    {
                        Width = "100%",
                        AutosizeY = true,
                        Reversed = true,
                        MarginTop = "10px",
                        Children = new List<Component>()
                        {
                            new Button("Otwórz")
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
                }
            });

            _internalApp.Root.FindChild("dropdown_menu").Slot.Style = "drives";
            _internalApp.Root.FindChild("dropdown_button").Padding = "10px";

            _internalApp.Root.FindChild("close_btn").Clicked += (object sender) =>
            {
                _internalApp.Window.Close();
            };

            _internalApp.Root.FindChild("up_btn").Clicked += Up;

            _internalApp.Window.KeyReleased += (object sender, KeyEventArgs e) =>
            {
                if (e.Code == Keyboard.Key.Enter)
                    Navigate(CurrentPath);
            };

            (_internalApp.Root.FindChild("drives") as Dropdown).OnSelected += (object sender, string text, int id) =>
            {
                Navigate(text);
            };

            _internalApp.Root.FindChild("ok_btn").Clicked += (object sender) =>
            {
                OnFileSelected?.Invoke(this, new FSEntry(CurrentPath));
                _internalApp.Window.Close();
            };

            Navigate(StartDirectory);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _shouldClose = true;
        }

        private void Up(object sender)
        {
            if (Directory.Exists(_currentDirectory))
            {
                DirectoryInfo dInfo = new DirectoryInfo(_currentDirectory);
                if (dInfo.Parent != null)
                {
                    Navigate(dInfo.Parent.FullName);
                }
            }
        }
    }
}
