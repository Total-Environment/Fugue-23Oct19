using System.IO;

namespace TE.ComponentLibrary.ExcelImporter.Code.Documents
{
    public interface IFileReader
    {
        Stream Read(string filePath);
    }
}