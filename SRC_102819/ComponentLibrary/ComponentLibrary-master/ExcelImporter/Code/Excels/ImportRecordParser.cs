using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TE.ComponentLibrary.ExcelImporter.Code.Documents;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class ImportRecordParser
    {
        private readonly IFileVerifier _fileVerifier;

        public ImportRecordParser(IFileVerifier fileVerifier)
        {
            _fileVerifier = fileVerifier;
        }

        public Dictionary<string, List<ImportRecord>> Parse(IEnumerable<string> records)
        {
            var checkListDictionary = new Dictionary<string, List<ImportRecord>>();
            foreach (var record in records)
            {
                var checkLists = new List<ImportRecord>();

                var splitString = record.Split('~');
                if (splitString.Length < 2)
                {
                    WriteMessage($"Invalid Record {record},Skipping.");
                    continue;
                }
                var materialId = splitString[0].Trim();
                foreach (var checkList in splitString.Skip(1))
                {
                    var checkListDetails = checkList.Split(':', '%');
                    if (checkListDetails.Length != 3)
                    {
                        WriteMessage($"Invalid Record {record},Skipping.");
                        continue;
                    }

                    var templateName = ToCamelCase(checkListDetails[1].Trim());
                    var header = ToCamelCase(checkListDetails[0].Trim());
                    var checkListId = checkListDetails[2].Trim();
                    if (string.IsNullOrWhiteSpace(checkListId))
                    {
                        WriteMessage($"Empty Checklist in header {header} for {record},Skipping.");
                        continue;
                    }
                    if (checkListId.Equals("NA"))
                    {
                        WriteMessage($"Checklist has name as NA in header {header} for {record},Skipping.");
                        continue;
                    }
                    var checkListPath = _fileVerifier.ParseFilePath(checkListId);
                    checkLists.Add(new ImportRecord
                    {
                        Header = header,
                        Template = templateName,
                        CheckListId = checkListId,
                        CheckListPath = checkListPath
                    });
                }
                if (checkLists.Count > 0)
                {
                    checkListDictionary.Add(materialId, checkLists);
                }
            }
            return checkListDictionary;
        }

        private string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return char.ToLower(value[0]) + value.Substring(1);
        }

        private void WriteMessage(string message)
        {
            var filePath = "CheklistUpload.txt";
            using (var streamWriter = new StreamWriter(filePath, true))
            {
                Console.WriteLine(message);
                streamWriter.WriteLine(message);
            }
        }
    }
}