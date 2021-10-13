#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Text;

using Xunit;

namespace Brimborium.CodeBlocks.Library.Test {
    public class CBTemplateTests {

        [Fact]
        public void CBTemplate001() {
            var sb = new StringBuilder();
            var templateProvider = new CBTemplateProvider();
            var writer = new IndentedTextWriter(new StringWriter(sb), "  ");
            var ctxt = new CBRenderContext(templateProvider, writer);
            var data = new VM2(new VM1("a", "b"), new VM1("c", "d"));
            new TemplateVM2().RenderT(data, ctxt);
            Assert.Equal(@"a
c
", sb.ToString());

        }

        [Fact]
        public void CBTemplate002() {
            var sb = new StringBuilder();
            var templateProvider = new CBTemplateProvider();
            var writer = new IndentedTextWriter(new StringWriter(sb), "  ");
            var ctxt = new CBRenderContext(templateProvider, writer);
            var data = new VM2(new VM1("a", "b"), new VM1("c", "d"));
            templateProvider.AddTemplate(new TemplateVM2(), "", "");
            Assert.True(ctxt.TemplateProvider.TryGetTemplate<VM2>("", true, out var template));
            (template as CBTemplate<VM2>)?.RenderT(data, ctxt);
            Assert.Equal(@"a
c
", sb.ToString());
        }

        [Fact]
        public void CBTemplate003() {
            var sb = new StringBuilder();
            var templateProvider = new CBTemplateProvider();
            templateProvider.AddTemplateFromFunction<VM2>((v, c) => {
                c.CallTemplateStrict(v.Left);
                c.CallTemplateStrict(v.Right, "=");
            }, "", "");
            templateProvider.AddTemplateFromFunction<VM1>((v, c) => c.WriteLine($"{v.Name}: {v.Value}"), "", "");
            templateProvider.AddTemplateFromFunction<VM1>((v, c) => c.WriteLine($"{v.Name}= {v.Value}"), "", "=");
            //
            var writer = new IndentedTextWriter(new StringWriter(sb), "  ");
            var ctxt = new CBRenderContext(templateProvider, writer);
            var data = new VM2(new VM1("a", "b"), new VM1("c", "d"));
            ctxt.CallTemplateStrict(new VM2(new VM1("a", "b"), new VM1("c", "d")));
            Assert.Equal(@"a: b
c= d
", sb.ToString());
        }

        [Fact]
        public void CBTemplate004() {
            var sb = new StringBuilder();
            var templateProvider = new CBTemplateProvider();
            templateProvider.AddTemplateFromFunction<VM2>((v, c) => {
                c.CallTemplateStrict(v.Left, ":");
                c.CallTemplateStrict(v.Right, "=");
            },
            "Test", "");
            templateProvider.AddTemplateFromFunction<VM1>((v, c) => c.WriteLine($"NO"), "Other", "");
            templateProvider.AddTemplateFromFunction<VM1>((v, c) => c.WriteLine($"{v.Name}: {v.Value}"), "Test", "");
            templateProvider.AddTemplateFromFunction<VM1>((v, c) => c.WriteLine($"{v.Name}= {v.Value}"), "Test", "=");
            //
            var writer = new IndentedTextWriter(new StringWriter(sb), "  ");
            var ctxt = new CBRenderContext(templateProvider.GetTemplateByLanguage("Test"), writer);
            var data = new VM2(new VM1("a", "b"), new VM1("c", "d"));
            ctxt.CallTemplateStrict(new VM2(new VM1("a", "b"), new VM1("c", "d")), "");
            Assert.Equal(@"a: b
c= d
", sb.ToString());
        }

        record VM1(string Name, string Value);
        record VM2(VM1 Left, VM1 Right);

        class TemplateVM1 : CBTemplate<VM1> {
            public override void RenderT(VM1 value, CBRenderContext ctxt) {
                ctxt.WriteLine(value.Name);
            }
        }

        class TemplateVM2 : CBTemplate<VM2> {
            public override void RenderT(VM2 value, CBRenderContext ctxt) {
                var template = new TemplateVM1();
                template.RenderT(value.Left, ctxt);
                template.RenderT(value.Right, ctxt);
            }
        }
    }
}
