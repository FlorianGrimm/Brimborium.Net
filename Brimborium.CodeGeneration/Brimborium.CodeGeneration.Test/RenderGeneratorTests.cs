namespace Brimborium.CodeGeneration {
    public class RenderGeneratorTests {
        [Fact()]
        public void GetValueTest() {
            var renderGenerator = new RenderGenerator(
                new List<RenderBinding>(){
                    new RenderBinding<int>(
                        Data:1234,
                        Template: new RenderTemplate<int>(
                            Render: (data,ctxt)=>ctxt.AppendLine(data.ToString()),
                            NameFn:(_)=>"abc"))
                },
                new Dictionary<string, string>()
                );
            Assert.Equal("    -- abc|", renderGenerator.GetValue("HELP").Replace(System.Environment.NewLine, "|"));
            Assert.Equal("    -- Replace=abc --|    1234|    -- Replace#abc --||", renderGenerator.GetValue("SNIPPETS").Replace(System.Environment.NewLine, "|"));
            renderGenerator.Add(
                new RenderBinding<int>(
                        Data: 2345,
                        Template: new RenderTemplate<int>(
                            Render: (data, ctxt) => ctxt.AppendLine(data.ToString()),
                            NameFn: (_) => "def")));
            Assert.False(
                renderGenerator.Add(
                    new RenderBinding<int>(
                            Data: 6789,
                            Template: new RenderTemplate<int>(
                                Render: (data, ctxt) => ctxt.AppendLine(data.ToString()),
                                NameFn: (_) => ""))));
            Assert.Equal("1234|", renderGenerator.GetValue("abc", 0).Replace(System.Environment.NewLine, "|"));
            Assert.Equal("2345|", renderGenerator.GetValue("def", 0).Replace(System.Environment.NewLine, "|"));
            Assert.Equal(" 1234|", renderGenerator.GetValue("abc", 1).Replace(System.Environment.NewLine, "|"));
            Assert.Equal(" 2345|", renderGenerator.GetValue("def", 1).Replace(System.Environment.NewLine, "|"));
            Assert.Equal("  1234|", renderGenerator.GetValue("abc", 2).Replace(System.Environment.NewLine, "|"));
            Assert.Equal("  2345|", renderGenerator.GetValue("def", 2).Replace(System.Environment.NewLine, "|"));
        }
    }
}