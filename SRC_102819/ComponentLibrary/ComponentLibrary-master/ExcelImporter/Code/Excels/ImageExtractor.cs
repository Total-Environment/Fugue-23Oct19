using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DocumentFormat.OpenXml.Packaging;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class ImageExtractor
    {
        public ExtendedImage GetImage(WorksheetPart workSheetPart, string path)
        {
            var imageParts = GetImageParts(workSheetPart);
            ExtendedImage extendedImage = null;
            if (imageParts == null)
                throw new FileNotFoundException(
                    $"Not image found in sheet named \"{""}\" in excel file at path \"{path}\"");
            foreach (var imagePart in imageParts)
            {
                var nameOfImage = NameOfImage(imagePart.Uri.OriginalString);
                var image = Image(imagePart);
                if (image != null)
                    extendedImage = new ExtendedImage(image, nameOfImage);
            }
            return extendedImage;
        }

        public IEnumerable<ImagePart> GetImageParts(WorksheetPart worksheetPart)
        {
            var drawingsPart = worksheetPart?.DrawingsPart;
            return drawingsPart?.ImageParts;
        }

        private Image Image(OpenXmlPart imagePart)
        {
            var byteStream = ImageByteStream(imagePart);
            return ByteArrayToImage(byteStream);
        }

        private byte[] ImageByteStream(OpenXmlPart imagePart)
        {
            var stream = imagePart.GetStream();
            var length = stream.Length;
            var byteStream = new byte[length];
            stream.Read(byteStream, 0, (int) length);
            return byteStream;
        }

        private string NameOfImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return null;
            var index = imagePath.LastIndexOf(@"/", StringComparison.Ordinal);
            return imagePath.Substring(++index);
        }

        private Image ByteArrayToImage(byte[] byteArrayIn)
        {
            return System.Drawing.Image.FromStream(new MemoryStream(byteArrayIn));
        }
    }
}