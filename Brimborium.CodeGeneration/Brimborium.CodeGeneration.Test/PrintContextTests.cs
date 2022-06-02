namespace Brimborium.CodeGeneration {
    public class PrintContextTests {
        [Fact()]
        public void PrintContextTest() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            Assert.Same(output, pc.Output);
            Assert.Same(output, pc.PrintOutput.Output);
            Assert.NotNull(pc.PrintOutput.BoundVariables);
            Assert.False(pc.PrintOutput.IndentWritten);
            pc.PrintOutput.Append("  ", "a",false);
            Assert.True(pc.PrintOutput.IndentWritten);
            pc.PrintOutput.Append("  ", "b",false);
            pc.PrintOutput.Append("  ", "c",true);
            Assert.False(pc.PrintOutput.IndentWritten);
            pc.PrintOutput.Append("  ", "d",false);
            pc.PrintOutput.AppendLine();            
            Assert.Equal("  abc|  d|", output.ToString().ReplaceNewLineToPipe());

        }

        [Fact()]
        public void PrintContextTest1() {
            var output = new StringBuilder();
            var pcA = new PrintContext(output).GetIndented("a:");
            var pcB = new PrintContext(pcA, "b:", 0, 3);
            
            Assert.Same(pcA.PrintOutput, pcB.PrintOutput);

            pcA.AppendLine("Start;");
            while (pcB.Index < pcB.Count) {
                pcB.AppendPartsLine(pcB.Index.ToString(), "-", pcB.IfFirst("F"), "-", pcB.IfLast("L"), ";");
                pcB.Index++;
            }
            pcA.AppendLine("End;");
            Assert.Equal("a:Start;|b:0-F-;|b:1--;|b:2--L;|a:End;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void GetIndentedTest() {
            var output = new StringBuilder();
            var pc = new PrintContext(output).GetIndented("a:");
            pc.AppendLine("1;");
            pc.GetIndented("b:").AppendLine("2;");
            Assert.Equal("a:1;|a:b:2;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendTest() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.Append("1;");
            pc.GetIndented("b:").Append("2;");
            Assert.Equal("1;2;", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendLineAndErrorTest() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendLineAndError("1;");
            Assert.Equal("1;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendPartsTest() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendParts("1;", "2;");
            Assert.Equal("1;2;", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendLineTest() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendLine("1;");
            pc.AppendLine("2;");
            Assert.Equal("1;|2;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void AppendPartsLineTest() {
            var output = new StringBuilder();
            var pc = new PrintContext(output);
            pc.AppendPartsLine("1;", "2;");
            pc.AppendPartsLine("1;", "2;");
            Assert.Equal("1;2;|1;2;|", output.ToString().ReplaceNewLineToPipe());
        }

        [Fact()]
        public void SetListPositionTest() {

        }
    }
}