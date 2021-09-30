using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.CodeBlocks.Library.Test {

    public class CBRendererOutMostTests {
        [Fact]
        public async Task Test001() {
            var sbOutput = new StringBuilder();
            using var stringWriter = new StringWriter(sbOutput);
            using var indentedTextWriter = new IndentedTextWriter(stringWriter);
            var cbRenderIndentedTextWriter = new CBRenderIndentedTextWriter(indentedTextWriter);
            var cbRendererOutMost = new CBRendererOutMost(cbRenderIndentedTextWriter);
            cbRendererOutMost.WriteText("a");
            cbRendererOutMost.EnsureNewLine();
            cbRendererOutMost.WriteText("b");
            cbRendererOutMost.WriteText("c");
            cbRendererOutMost.EnsureNewLine();
            cbRendererOutMost.WriteText("d");
            cbRendererOutMost.WriteText("e");
            cbRendererOutMost.WriteText("f");
            cbRendererOutMost.EnsureNewLine();
            await cbRendererOutMost.CloseAsync();
            Assert.Equal(@"a
bc
def
", sbOutput.ToString());
        }
    }
}
