using System.IO;
using System.IO.Compression;

namespace Osmuim.GUI.Helpers
{
    public static class ExtractionHelper
    {
        public static void ExtractFiles(string sourceFolder, string targetFolder, string version)
        {
            string extractDir = Path.Combine(targetFolder, $"Chrome/{version}");

            Directory.CreateDirectory(extractDir);

            foreach (var zipFilePath in Directory.GetFiles(sourceFolder, "*.zip"))
            {
                //string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(zipFilePath);
                //string extractPath = Path.Combine(extractDir, fileNameWithoutExtension);

                //Directory.CreateDirectory(extractPath);
                //ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                ZipFile.ExtractToDirectory(zipFilePath, extractDir);
            }
        }
    }
}
