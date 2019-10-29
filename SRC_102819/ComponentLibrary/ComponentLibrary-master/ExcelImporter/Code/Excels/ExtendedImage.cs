using System.Drawing;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class ExtendedImage
    {
        public ExtendedImage(Image image, string name)
        {
            Image = image;
            Name = name;
        }

        public Image Image { get; }
        public string Name { get; }

        public void Save(string path)
        {
            Image.Save(path + Name);
        }
    }
}