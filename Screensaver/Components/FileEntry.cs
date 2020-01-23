using HlyssUI.Components;
using HlyssUI.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Screensaver.Components
{
    class FileEntry : ListItem
    {
        private FolderBrowser _browser;
        private FolderBrowser.FSEntry _entry;
        private bool _selected = false;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if(value != _selected)
                {
                    _selected = value;
                    //select
                }
            }
        }

        public FileEntry(FolderBrowser browser, FolderBrowser.FSEntry entry) : base(entry.Name)
        {
            _browser = browser;
            _entry = entry;

            Clicked += FileEntry_Clicked;
            DoubleClicked += FileEntry_DoubleClicked;

            Icon = (entry.IsDirectory) ? Icons.Folder : Icons.File;
        }

        private void FileEntry_DoubleClicked(object sender)
        {
            if (_entry.IsDirectory)
                _browser.Navigate(_entry.FullPath);
        }

        private void FileEntry_Clicked(object sender)
        {
            if (!_entry.IsDirectory)
                Selected = true;
        }
    }
}
