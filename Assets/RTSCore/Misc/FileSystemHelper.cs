using System;
using System.IO;

namespace Assets.RTSCore.Misc
{
    public static class FileSystemHelper
    {
        public static string Root = Environment.CurrentDirectory;

        public static string GameDirectory
        {
            get
            {
                return CombineDirectorySafe(Root, "MetroD");
            }
        }
        
        public static string DataDirectory
        {
            get
            {
                return CombineDirectorySafe(GameDirectory, "Data");
            }
        }

        public static string ItemConversionChartMaster
        {
            get
            {
                return Path.Combine(DataDirectory, "ConversionChartMaster.xml");
            }
        }

        public static string CombineDirectorySafe(string directory1, string directory2)
        {
            string path = Path.Combine(directory1, directory2);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
