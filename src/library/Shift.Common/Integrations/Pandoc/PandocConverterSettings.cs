using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shift.Common
{
    public class PandocConverterSettings
    {
        #region Enums

        public enum PanDocType
        {
            Unknown,
            Word,
            Markdown
        }

        #endregion

        #region Dictionaries

        private Dictionary<PanDocType, string> fileExtension = new Dictionary<PanDocType, string>()
        {
            { PanDocType.Word,".docx"},
            { PanDocType.Markdown,".md" }
        };

        private Dictionary<PanDocType, string> convertArguments = new Dictionary<PanDocType, string>()
        {
            { PanDocType.Word,"docx"},
            { PanDocType.Markdown,"markdown_strict" }
        };

        public Dictionary<PanDocType, List<PanDocType>> convertionMatrix = new Dictionary<PanDocType, List<PanDocType>>()
        {
            { PanDocType.Word, new List<PanDocType>{PanDocType.Markdown } }
        };

        #endregion

        #region Properties

        public string ExePath { get; set; }

        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Physical file path to convert
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Physical file path of the convert file
        /// </summary>
        public string OutputFilePath { get; set; }

        /// <summary>
        /// Web cache directory
        /// </summary>
        public string WebCacheDirectory { get; set; }

        /// <summary>
        /// File name without extension
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Converted file extension
        /// </summary>
        public string ConvertFileExtension { get; set; }

        private string ArgFrom { get; set; }

        private string ArgTo { get; set; }

        /// <summary>
        /// Property that states that should we include images with converted file - default to false;
        /// </summary>
        public bool ConvertWithImages { get; set; }

        #endregion

        #region Construction

        public PandocConverterSettings(string exePath, string filePath, PanDocType convertTo, bool withImages = false)
        {
            if (string.IsNullOrEmpty(exePath) || string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException();

            if (!CanConvert(GetFileExtensionType(Path.GetExtension(filePath)), convertTo))
                throw new PandocConverterException($"File extension not supported for conversion:\r\n");

            ConvertFileExtension = GetConvertFileExtension(convertTo);
            ExePath = exePath;
            FilePath = filePath;
            WorkingDirectory = Path.GetDirectoryName(filePath);
            FileName = Path.GetFileNameWithoutExtension(filePath);
            WebCacheDirectory = Path.Combine(WorkingDirectory, FileName);
            ConvertWithImages = withImages;
            ArgFrom = GetConvertArgument(Path.GetExtension(filePath));
            ArgTo = GetConvertArgument(Path.GetExtension(ConvertFileExtension));
            OutputFilePath = WebCacheDirectory + ConvertFileExtension;
        }

        public string ReadSettings()
        {
            return $"ConvertFileExtension:{ConvertFileExtension} \r\n" +
                $"ExePath:{ExePath} \r\n" +
                $"FilePath:{FilePath} \r\n" +
                $"WorkingDirectory:{WorkingDirectory} \r\n" +
                $"FileName{FileName} \r\n" +
                $"WebCacheDirectory{WebCacheDirectory} \r\n" +
                $"ConvertWithImages{ConvertWithImages} \r\n" +
                $"ArgFrom{ArgFrom} \r\n" +
                $"ArgTo{ArgTo} \r\n" +
                $"OutputFilePath{OutputFilePath}\r\n";
        }

        #endregion

        public string GetArgs()
        {
            if (ConvertWithImages)
                return $@"--from {this.ArgFrom} --to {this.ArgTo} --data-dir=""{this.WorkingDirectory}"" --output ""{this.OutputFilePath}"" ""{this.FilePath}"" --extract-media=""{this.WebCacheDirectory}"" --wrap=none";
            else
                return $@"--from {this.ArgFrom} --to {this.ArgTo} --data-dir=""{this.WorkingDirectory}"" --output ""{this.OutputFilePath}"" ""{this.FilePath}"" --wrap=nonr";
        }

        #region Helper Methods

        private string GetConvertFileExtension(PanDocType convertType)
        {
            return fileExtension.FirstOrDefault(x => x.Key == convertType).Value;
        }

        private string GetConvertArgument(string fileExt)
        {
            var type = fileExtension.FirstOrDefault(x => x.Value.Equals(fileExt, StringComparison.OrdinalIgnoreCase)).Key;
            if (type == PanDocType.Unknown)
                throw new PandocConverterException($"File extension not supported for conversion:\r\n {fileExt}");

            return convertArguments.FirstOrDefault(x => x.Key == type).Value;
        }

        private PanDocType GetFileExtensionType(string fileExt)
        {
            var type = fileExtension.FirstOrDefault(x => x.Value == fileExt).Key;
            if (type == PanDocType.Unknown)
                throw new PandocConverterException($"File extension not supported for conversion:\r\n {fileExt}");

            return type;
        }

        private bool CanConvert(PanDocType _from, PanDocType _to)
        {
            if (convertionMatrix.ContainsKey(_from))
            {
                List<PanDocType> permissionValues;
                convertionMatrix.TryGetValue(_from, out permissionValues);
                if (permissionValues != null)
                    if (permissionValues.Count > 0)
                        return permissionValues.Where(x => x.Equals(_to)).Any();
            }

            return false;
        }

        private string RemoveAllWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        #endregion
    }
}
