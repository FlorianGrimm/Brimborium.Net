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
            this.Items.Add(fileContent);
            var result = this.TempFileSystem.SetFileContent(fileContent);
            fileContent.TempModified = result;
            return result;
        }

        public void CopyReplace() {
            this._Logger.LogInformation("TODO CopyReplace");
        }
    }
}
