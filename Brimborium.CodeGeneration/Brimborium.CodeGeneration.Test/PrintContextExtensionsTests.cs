namespace Brimborium.CodeGeneration {
    public class PrintContextExtensionsTests {

        [Fact()]
        public void AppendIndentedTest() {

        }

        [Fact()]
        public void RenderTemplateTest() {

        }

        [Fact()]
        public void AppendListTest_Action() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendList(
                System.Linq.Enumerable.Range(1, 5),
                (data, ctxt) => {
                    ctxt.AppendPartsLine(data.ToString(), ";");
                });
            Assert.Equal("1;|2;|3;|4;|5;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendListTest_RenderTemplate() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendList(
                System.Linq.Enumerable.Range(1, 5),
                new RenderTemplate<int>(
                    (data, ctxt) => {
                        ctxt.AppendPartsLine(data.ToString(), ";");
                    }));
            Assert.Equal("1;|2;|3;|4;|5;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendCurlyBlockTest_Action_Full() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendCurlyBlock(
                Data: 1,
                FirstBefore: (data, ctxt) => {
                    ctxt.Append("start;");
                },
                Render: (data, ctxt) => {
                    ctxt.AppendLine("inner;");
                },
                LastAfter: (data, ctxt) => {
                    ctxt.AppendLine("end;");
                });
            Assert.Equal("start; {|    inner;|} end;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendCurlyBlockTest_Action_OnlyInner() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendCurlyBlock(
                Data: 1,
                FirstBefore: null,
                Render: (data, ctxt) => {
                    ctxt.AppendLine("inner;");
                },
                LastAfter: null);
            Assert.Equal("{|    inner;|}|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendCurlyBlockTest_RenderTemplate_Full() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendCurlyBlock(
                Data: 1,
                FirstBefore: new RenderTemplate<int>(
                    Render: (data, ctxt) => {
                        ctxt.Append("start;");
                    }),
                Inner: new RenderTemplate<int>(
                    Render: (data, ctxt) => {
                        ctxt.AppendLine("inner;");
                    }),
                LastAfter: new RenderTemplate<int>(
                    Render: (data, ctxt) => {
                        ctxt.AppendLine("end;");
                    }));
            Assert.Equal("start; {|    inner;|} end;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendCurlyBlockTest_RenderTemplate_OnlyInner() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendCurlyBlock(
                Data: 1,
                FirstBefore: null,
                Inner: new RenderTemplate<int>(
                    Render: (data, ctxt) => {
                        ctxt.AppendLine("inner;");
                    }),
                LastAfter: null);
            Assert.Equal("{|    inner;|}|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void SwitchFirstTest() {
            var output = new StringBuilder();
            var pcA = new PrintContext(output).GetIndented("a:");
            var pcB = new PrintContext(pcA, "b:", 0, 3);
            pcA.AppendLine("Start;");
            while (pcB.Index < pcB.Count) {
                pcB.AppendPartsLine(pcB.Index.ToString(), "-", pcB.SwitchFirst("F", "f"), ";");
                pcB.Index++;
            }
            pcA.AppendLine("End;");
            Assert.Equal("a:Start;|b:0-F;|b:1-f;|b:2-f;|a:End;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void IfFirstTest() {
            var output = new StringBuilder();
            var pcA = new PrintContext(output).GetIndented("a:");
            var pcB = new PrintContext(pcA, "b:", 0, 3);
            pcA.AppendLine("Start;");
            while (pcB.Index < pcB.Count) {
                pcB.AppendPartsLine(pcB.Index.ToString(), "-", pcB.IfFirst("F"), ";");
                pcB.Index++;
            }
            pcA.AppendLine("End;");
            Assert.Equal("a:Start;|b:0-F;|b:1-;|b:2-;|a:End;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void IfNotFirstTest() {
            var output = new StringBuilder();
            var pcA = new PrintContext(output).GetIndented("a:");
            var pcB = new PrintContext(pcA, "b:", 0, 3);
            pcA.AppendLine("Start;");
            while (pcB.Index < pcB.Count) {
                pcB.AppendPartsLine(pcB.Index.ToString(), "-", pcB.IfNotFirst("f"), ";");
                pcB.Index++;
            }
            pcA.AppendLine("End;");
            Assert.Equal("a:Start;|b:0-;|b:1-f;|b:2-f;|a:End;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void IfLastTest() {
            var output = new StringBuilder();
            var pcA = new PrintContext(output).GetIndented("a:");
            var pcB = new PrintContext(pcA, "b:", 0, 3);
            pcA.AppendLine("Start;");
            while (pcB.Index < pcB.Count) {
                pcB.AppendPartsLine(pcB.Index.ToString(), "-", pcB.IfLast("L"), ";");
                pcB.Index++;
            }
            pcA.AppendLine("End;");
            Assert.Equal("a:Start;|b:0-;|b:1-;|b:2-L;|a:End;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void IfNotLastTest() {
            var output = new StringBuilder();
            var pcA = new PrintContext(output).GetIndented("a:");
            var pcB = new PrintContext(pcA, "b:", 0, 3);
            pcA.AppendLine("Start;");
            while (pcB.Index < pcB.Count) {
                pcB.AppendPartsLine(pcB.Index.ToString(), "-", pcB.IfNotLast("l"), ";");
                pcB.Index++;
            }
            pcA.AppendLine("End;");
            Assert.Equal("a:Start;|b:0-l;|b:1-l;|b:2-;|a:End;|", output.ToString().ReplaceNewLineToPipe());
        }
    }
}