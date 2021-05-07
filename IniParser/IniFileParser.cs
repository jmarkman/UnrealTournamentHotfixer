using IniParser.Models;
using System;
using System.IO;
using System.Text;

namespace IniParser
{
    public class IniFileParser
    {
        private IniParserEngine parserEngine;

        // TODO: This will be updated when I extract this library to its own project
        // such that a config object can be passed to this ctor that the engine can
        // subsequently consume
        public IniFileParser()
        {
            parserEngine = new IniParserEngine();
        }

        public IniFile ReadFile(string filePath)
        {
            return ReadFile(filePath, Encoding.ASCII);
        }

        public IniFile ReadFile(string filePath, Encoding encoding)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The provided path was either invalid or empty.");
            }

            try
            {
                // ini-parser uses FileShare.ReadWrite, which is questionable: why are we modifying the file by hand
                // while parsing it from the parser? Maybe for debugging, but I don't currently see why else.
                using FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using StreamReader streamReader = new(fileStream, encoding);

                return parserEngine.Parse(streamReader.ReadToEnd());
            }
            catch (IOException ioEx)
            {
                throw new IOException(ioEx.Message);
            }
        }

        public void WriteFile(string filePath, IniFile data, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The provided path was either invalid or empty.");
            }

            if (data == null)
            {
                throw new ArgumentException("The provided INI data object was null");
            }

            using FileStream fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write);
            using StreamWriter streamWriter = new(fileStream, encoding);

            streamWriter.Write(data.ToString());
        }
    }
}
