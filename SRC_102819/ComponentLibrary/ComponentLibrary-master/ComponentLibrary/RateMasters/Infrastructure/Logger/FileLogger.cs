using System;
using System.IO;
using ComponentLibrary.RateMasters.Domain;

namespace ComponentLibrary.RateMasters.Infrastructure.Logger
{
    public class FileLogger : ILogger
    {
        private static readonly string FileName = @"c:\ComponentLibraryLog.txt";
        private readonly Type _context;

        public FileLogger(Type context)
        {
            _context = context;
        }

        public void Debug(string msg)
        {
            WriteToFile(msg, "DEBUG");
        }

        public void Warn(string msg)
        {
            WriteToFile(msg, "WARN");
        }

        public void Info(string msg)
        {
            WriteToFile(msg, "INFO");
        }

        public void Error(string msg)
        {
            WriteToFile(msg, "ERROR");
        }

        public void Fatal(string msg)
        {
            WriteToFile(msg, "FATAL");
        }

        private void WriteToFile(string msg, string logLevel)
        {
//            using (var streamWriter = File.CreateText(FileName))
//            {
//                streamWriter.WriteLineAsync($"{logLevel} : {_context.FullName} : {DateTime.Now} : {msg}");
//                streamWriter.FlushAsync();
//            }
        }
    }
}