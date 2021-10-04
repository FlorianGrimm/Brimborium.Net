using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.CodeBlocks.Library {
    public class CBFileContent {
        public CBFileContent(string fileName, string content) {
            this.FileName = fileName;
            this.Content = content;
        }

        public string FileName { get; set; }
        public string Content { get; set; }
        public bool TempModified { get; internal set; }
    }
}
