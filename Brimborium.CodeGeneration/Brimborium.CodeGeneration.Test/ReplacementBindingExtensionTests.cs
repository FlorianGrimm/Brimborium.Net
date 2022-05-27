namespace Brimborium.CodeGeneration {
    public class ReplacementBindingExtensionTests {
        [Fact()]
        public void ContainsReplaceTest() {
            Assert.True(ReplacementBindingExtension.ContainsReplace("-- Replace=abc --|abc|-- Replace#abc --|abc|".Replace("|", System.Environment.NewLine)));
            Assert.False(ReplacementBindingExtension.ContainsReplace("- Replace=abc --|-- Replac=abc --|-- Replace,abc --|-- Replace= --|-- Replace=abc -|".Replace("|", System.Environment.NewLine)));
        }

        [Fact()]
        public void IndexOfReplaceStartTest() {
            {
                var fileContent = "-- Replace=abc --|abc|-- Replace#abc --|abc|"
                    .Replace("|", System.Environment.NewLine);
                var act = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                Assert.Equal(0, act.start);
                Assert.Equal(19, act.len);
                Assert.Equal("abc", act.name);
            }
            {
                var fileContent = "  sof|  -- Replace=abc --|  abc|-- Replace#abc --|  eof|"
                    .Replace("|", System.Environment.NewLine);
                var act = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                Assert.Equal(9, act.start);
                Assert.Equal(19, act.len);
                Assert.Equal("abc", act.name);
            }
        }

        [Fact()]
        public void IndexOfReplaceStopTest() {
            {
                var fileContent = "-- Replace=abc --|abc|-- Replace#abc --|abc|".Replace("|", System.Environment.NewLine);
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(22, act.start);
                Assert.Equal(19, act.prefixTokenLen);
                Assert.Equal("-- Replace#abc --", act.tokenStop);
            }
            {
                var fileContent = "  sof|  -- Replace=abc --|  abc|  -- Replace#abc --|  eof|".Replace("|", System.Environment.NewLine);
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(36, act.start);
                Assert.Equal(18, act.prefixTokenLen);
                Assert.Equal("-- Replace#abc --", act.tokenStop);
            }
        }

        [Fact()]
        public void ReplaceTest() {
            {
                var (changed, content) = ReplacementBindingExtension.Replace("-- Replace=abc --|abc|-- Replace#abc --|abc|".Replace("|", System.Environment.NewLine), (a, b) => "DEF");
                Assert.True(changed);
                Assert.Equal("-- Replace=abc --|DEF|-- Replace#abc --|abc|", content.Replace(System.Environment.NewLine, "|"));
                (changed, content) = ReplacementBindingExtension.Replace(content, (a, b) => "DEF");
                Assert.False(changed);
                Assert.Equal("-- Replace=abc --|DEF|-- Replace#abc --|abc|", content.Replace(System.Environment.NewLine, "|"));
            }
            {
                var fileContent = "-- Replace=abc --|abc|-- Replace#abc --|abc|".Replace("|", System.Environment.NewLine);
                var (changed, content) = ReplacementBindingExtension.Replace(fileContent, (a, b) => String.Empty);
                Assert.False(changed);
                Assert.Equal(fileContent, content);
            }
            {
                var fileContent = "  sof|  -- Replace=abc --|  abc|  -- Replace#abc --|  eof|".Replace("|", System.Environment.NewLine);
                var (changed, content) = ReplacementBindingExtension.Replace(fileContent, (a, b) => (new string(' ', b) + "DEF" + System.Environment.NewLine));
                Assert.True(changed);
                Assert.Equal("  sof|  -- Replace=abc --|  DEF|  -- Replace#abc --|  eof|", content.Replace(System.Environment.NewLine, "|"));
            }
            {
                var fileContent = "  sof|  -- Replace=abc --|  abc|  -- Replace#abc --|  eof|".Replace("|", System.Environment.NewLine);
                var (changed, content) = ReplacementBindingExtension.Replace(fileContent, (a, b) => String.Empty);
                Assert.False(changed);
                Assert.Equal(fileContent, content);
            }
        }
    }
}