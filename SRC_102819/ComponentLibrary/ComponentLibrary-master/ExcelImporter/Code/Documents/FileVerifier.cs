using System.IO;
using System.Linq;

namespace TE.ComponentLibrary.ExcelImporter.Code.Documents
{
    public class FileVerifier : IFileVerifier
    {
        private readonly string _rootDirectory;
        private readonly string[] _configurationCheckListFileExtension;

        public FileVerifier(string rootDirectory, string[] configurationCheckListFileExtension)
        {
            _rootDirectory = rootDirectory;
            _configurationCheckListFileExtension = configurationCheckListFileExtension;
        }

        public string ParseFilePath(string fileName)
        {
            var staticDirectoryInfo = new DirectoryInfo(_rootDirectory);
            var filePath = staticDirectoryInfo.GetFiles()
                .Select(f => f.FullName)
                .SingleOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileName && _configurationCheckListFileExtension.Contains(Path.GetExtension(f)));
            return string.IsNullOrEmpty(filePath) ? "" : filePath;
        }
    }
}