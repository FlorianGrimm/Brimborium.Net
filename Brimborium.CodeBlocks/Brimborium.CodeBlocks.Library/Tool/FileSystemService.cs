using Brimborium.CodeBlocks.Library;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;

namespace Brimborium.CodeBlocks.Tool {
    public class FileSystemService {
        public readonly string RootPath;
        private readonly ILogger _Logger;
        public readonly List<CBFileContent> Items;

        public FileSystemService(string rootPath, ILogger logger) {
            this.Items = new List<CBFileContent>();
            this.RootPath = rootPath;
            this._Logger = logger;
        }

        public void CreateDirectory(string relativePath) {
            var path = Path.Combine(this.RootPath, relativePath);
            if (Directory.Exists(path)) {
                // OK
            } else {
                Directory.CreateDirectory(path);
            }
        }

        public bool SetFileContent(CBFileContent fileContent) {
            this.Items.Add(fileContent);
            var fileName = System.IO.Path.Combine(this.RootPath, fileContent.FileName);
            var fi = new System.IO.FileInfo(fileName);
            if (fi.Exists) {
                string oldConent;
                using (StreamReader sr = new StreamReader(fi.OpenRead(), System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true)) {
                    oldConent = sr.ReadToEnd();
                }
                var result = StringComparer.Ordinal.Equals(oldConent, fileContent);
                this._Logger.LogDebug("{fileName} changes: {result}", fileName, result);
                if (result) {
                    CreateDirectory(System.IO.Path.GetDirectoryName(fileContent.FileName)!);
                    using (StreamWriter sw = new StreamWriter(fi.OpenWrite(), System.Text.Encoding.UTF8)) {
                        sw.Write(fileContent.Content);
                    }
                }
                return result;
            } else {
                this._Logger.LogDebug("{fileName} new content.", fileName);
                return true;
            }
        }


        public void CopyReplace() {
        }

    }
}
