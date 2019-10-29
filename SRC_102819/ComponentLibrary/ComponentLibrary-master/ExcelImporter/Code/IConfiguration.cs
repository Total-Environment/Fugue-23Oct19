namespace TE.ComponentLibrary.ExcelImporter.Code
{
    public interface IConfiguration
    {
        string RowNumberToStartFrom { get; }
        string ComponentLibraryBaseUrl { get; }
        string DocumentLoggingPath { get; }
        string ExcelPath { get; }

        string XmlPathFor(string group);

        string AzureConnectionString { get; }
        string CheckListBasePath { get; }
    }
}