using Brimborium.CodeBlocks.Library;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Tool {
    public class ToolService {
        private readonly ILogger _Logger;

        public ToolService(
            FileSystemService baseFileSystem,
            FileSystemService tempFileSystem,
            ILogger logger
            ) {
            this.Items = new List<CBFileContent>();
            this.BaseFileSystem = baseFileSystem;
            this.TempFileSystem = tempFileSystem;
            this._Logger = logger;
        }

        public List<CBFileContent> Items { get; }
        public FileSystemService BaseFileSystem { get; }
        public FileSystemService TempFileSystem { get; }

        public CBFileContent SetFileContent(string fileName, string content) {
            var result = new CBFileContent(fileName, content);
            this.Items.Add(result);
            this.TempFileSystem.SetFileContent(result);
            return result;
        }

        public bool SetFileContent(CBFileContent fileContent) {
            var result = this.TempFileSystem.SetFileContent(fileContent);
            this.Items.Add(fileContent);
            fileContent.TempModified = result;
            return result;
        }

        public void CopyReplace(bool testOnly) {
            var cbCopyReplacer = new CBCopyReplacer();
            foreach (var item in this.Items) {
                if (this.BaseFileSystem.TryGetFileContent(item, out var current)) {
                    var next = cbCopyReplacer.ContentCopyReplace(item.Content, current.Content);
                    this.BaseFileSystem.SetFileContent(current, next, testOnly);
                } else {
                    current = new CBFileContent(item.FileName, string.Empty);
                    this.BaseFileSystem.SetFileContent(current, item.Content, testOnly);
                }
            }
        }
    }
}
