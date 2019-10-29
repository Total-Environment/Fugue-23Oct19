namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public interface ICustomCell
    {
        TypeOfData TypeOfData { get; }
        string Value { get; }
        string GetHyperLink();
    }
}