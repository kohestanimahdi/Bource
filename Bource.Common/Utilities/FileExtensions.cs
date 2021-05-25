using System.IO;

namespace Bource.Common.Utilities
{
    public static class FileExtensions
    {
        public static void CreateIfNotExists(string path)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
        }

        public static string GetDirectory(string path)
            => Path.Combine(Directory.GetCurrentDirectory(), path);

        public static void DeleteFileIfExists(string path)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
