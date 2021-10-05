using Brimborium.CodeBlocks.Library;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public bool TryGetFileContent(CBFileContent fileContent, [MaybeNullWhen(false)] out CBFileContent read) {
            var fileName = System.IO.Path.Combine(this.RootPath, fileContent.FileName);
            var fi = new System.IO.FileInfo(fileName);
            if (fi.Exists) {
                string oldConent;
                using (StreamReader sr = new StreamReader(fi.OpenRead(), System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true)) {
                    oldConent = sr.ReadToEnd();
                }
                read = new CBFileContent(fileContent.FileName, oldConent);
                return true;
            } else {
                read = default;
                return false;
            }
        }

        public bool SetFileContent(CBFileContent fileContent) {
            this.Items.Add(fileContent);
            var fileName = System.IO.Path.Combine(this.RootPath, fileContent.FileName);
            var fi = new System.IO.FileInfo(fileName);
            bool result;
            if (fi.Exists) {
                string oldContent;
                using (StreamReader sr = new StreamReader(fi.OpenRead(), System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true)) {
                    oldContent = sr.ReadToEnd();
                }
                result = StringComparer.Ordinal.Equals(oldContent, fileContent.Content);
                this._Logger.LogDebug("{fileName} changed: {result}", fileContent.FileName, result);
            } else {
                this._Logger.LogDebug("{fileName} new content.", fileContent.FileName);
                result = true;
            }
            if (result) {
                CreateDirectory(System.IO.Path.GetDirectoryName(fileName)!);
                System.IO.File.WriteAllText(fileName, fileContent.Content, System.Text.Encoding.UTF8);
            }
            return result;
        }

        public bool SetFileContent(CBFileContent current, string nextContent, bool testOnly) {
            string fileName = System.IO.Path.Combine(this.RootPath, current.FileName);
            bool changed = !StringComparer.Ordinal.Equals(current.Content, nextContent);
            this._Logger.LogDebug("{fileName} changed: {changed}", current.FileName, changed);

            if (changed && !testOnly) {
                CreateDirectory(System.IO.Path.GetDirectoryName(fileName)!);
                System.IO.File.WriteAllText(fileName, nextContent, System.Text.Encoding.UTF8);
            }
            return changed;
        }
    }
}
