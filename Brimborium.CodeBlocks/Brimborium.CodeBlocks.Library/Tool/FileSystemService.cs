using System;
using System.IO;
using System.Reflection.Metadata;

namespace Brimborium.CodeBlocks.Tool {
    public class FileSystemService {
        public readonly string RootPath;

        public FileSystemService(string rootPath) {
            this.RootPath = rootPath;
        }

        public void CreateDirectory(string relativePath) {
            var path = Path.Combine(this.RootPath, relativePath);
            if (Directory.Exists(path)) {
                // OK
            } else {
                Directory.CreateDirectory(path);
            }
        }

        public void CopyReplace() {
        }
    }
}
