using System.IO;

namespace TE.ComponentLibrary.ExcelImporter.Code.Documents
{
    public class FileReader : IFileReader
    {
        public Stream Read(string filePath)
        {
            return File.OpenRead(filePath);
        }
    }
}