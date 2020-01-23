namespace Screensaver
{
    class SlideShowConfig
    {
        public bool Random;
        public bool UseSubdirectories;
        public int IntervalId;
        public string Directory;

        public SlideShowConfig()
        {
            Random = false;
            UseSubdirectories = false;
            IntervalId = 0;
            Directory = string.Empty;
        }

        public void Load()
        {
            var parser = new IniParser.FileIniDataParser();
            var data = parser.ReadFile(Program.PATH + "settings.ini");

            Random = bool.Parse(data["settings"]["random"]);
            IntervalId = int.Parse(data["settings"]["speed"]);
            UseSubdirectories = bool.Parse(data["settings"]["subdirectories"]);
            Directory = data["settings"]["directory"];
        }

        public void Save()
        {
            var parser = new IniParser.FileIniDataParser();
            var data = parser.ReadFile(Program.PATH + "settings.ini");

            data["settings"]["random"] = Random.ToString();
            data["settings"]["speed"] = IntervalId.ToString();
            data["settings"]["subdirectories"] = UseSubdirectories.ToString();
            data["settings"]["directory"] = Directory;

            parser.WriteFile(Program.PATH + "settings.ini", data);
        }

        public int GetInterval()
        {
            return IntervalId switch
            {
                0 => 10000,
                1 => 6000,
                2 => 3000,
                _ => 6000
            };
        }
    }
}
