#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

using Xunit;

namespace Brimborium.CodeBlocks.Library.Test {
    public class CBRenderContextTests {
        [Fact]
        public void CBRenderContext001() {
            var sb = new StringBuilder();
            var templateProvider = new CBTemplateProvider();
            var writer = new IndentedTextWriter(new StringWriter(sb), "  ");
            var ctxt = new CBRenderContext(templateProvider, writer);
            ctxt.Write("if (a == b)").Write(" {").WriteLine(indent: +1)
                .Write("hugo();").WriteLine(indent: -1)
                .Write("}").WriteLine()
                ;
            Assert.Equal(@"if (a == b) {
  hugo();
}
", sb.ToString());

        }
    }
}
