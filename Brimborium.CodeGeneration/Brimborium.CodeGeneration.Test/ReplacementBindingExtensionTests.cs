namespace Brimborium.CodeGeneration {
    public class ReplacementBindingExtensionTests {
        [Fact()]
        public void ContainsReplaceTest() {
            Assert.True(ReplacementBindingExtension.ContainsReplace("-- Replace=abc --|abc|-- /Replace=abc --|abc|".ReplacePipeToNewLine()));
            Assert.False(ReplacementBindingExtension.ContainsReplace("- Replace=abc --|".ReplacePipeToNewLine()));
            Assert.False(ReplacementBindingExtension.ContainsReplace("-- Replac=abc --|".ReplacePipeToNewLine()));
            Assert.False(ReplacementBindingExtension.ContainsReplace("-- Replace,abc --|".ReplacePipeToNewLine()));
            Assert.False(ReplacementBindingExtension.ContainsReplace("-- Replace= --|".ReplacePipeToNewLine()));
            Assert.False(ReplacementBindingExtension.ContainsReplace("-- Replace=abc -|".ReplacePipeToNewLine()));
        }

        [Fact()]
        public void IndexOfReplaceStartSQLTest() {
            {
                var fileContent = "-- Replace=abc --|abc|-- /Replace=abc --|abc|"
                    .ReplacePipeToNewLine();
                var act = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                Assert.Equal(0, act.start);
                Assert.Equal(19, act.len);
                Assert.Equal("abc", act.name);
            }
            {
                var fileContent = "  sof|  -- Replace=abc --|  abc|-- /Replace=abc --|  eof|"
                    .ReplacePipeToNewLine();
                var act = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                Assert.Equal(9, act.start);
                Assert.Equal(19, act.len);
                Assert.Equal("abc", act.name);
            }
        }
        [Fact()]
        public void IndexOfReplaceStartCSTest() {
            {
                var fileContent = "/*-- Replace=abc --*/|abc|/*-- /Replace=abc --*/|abc|"
                    .ReplacePipeToNewLine();
                var act = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                Assert.Equal(0, act.start);
                Assert.Equal(23, act.len);
                Assert.Equal("abc", act.name);
            }
            {
                var fileContent = "  sof|  /*-- Replace=abc --*/|  abc|/*-- /Replace=abc --*/|  eof|"
                    .ReplacePipeToNewLine();
                var act = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                Assert.Equal(9, act.start);
                Assert.Equal(23, act.len);
                Assert.Equal("abc", act.name);
            }
        }

        [Fact()]
        public void IndexOfReplaceStop1Test() {
            {
                var fileContent = "-- Replace=abc --|abc|-- /Replace=abc --|abc|".ReplacePipeToNewLine();
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(22, act.start);
                Assert.Equal(20, act.prefixTokenLen);
                Assert.Equal("-- /Replace=abc --", act.tokenStop);
            }
            {
                var fileContent = "  sof|  -- Replace=abc --|  abc|  -- /Replace=abc --|  eof|".ReplacePipeToNewLine();
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(36, act.start);
                Assert.Equal(19, act.prefixTokenLen);
                Assert.Equal("-- /Replace=abc --", act.tokenStop);
            }
        }

        [Fact()]
        public void IndexOfReplaceStop2Test() {
            {
                var fileContent = "/*-- Replace=abc --*/|abc|/*-- /Replace=abc --*/|abc|".ReplacePipeToNewLine();
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(26, act.start);
                Assert.Equal(24, act.prefixTokenLen);
                Assert.Equal("/*-- /Replace=abc --*/", act.tokenStop);
            }

            {
                var fileContent = "  sof|  /*-- Replace=abc --*/|  abc|  /*-- /Replace=abc --*/|  eof|".ReplacePipeToNewLine();
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(40, act.start);
                Assert.Equal(23, act.prefixTokenLen);
                Assert.Equal("/*-- /Replace=abc --*/", act.tokenStop);
            }

            {
                var fileContent = "  sof|  /*-- Replace=abc --*/|  abc|  -- /Replace=abc --|  eof|".ReplacePipeToNewLine();
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(-1, act.start);
            }
        }

        [Fact()]
        public void Replace1Test() {
            {
                var (changed, content) = ReplacementBindingExtension.Replace("-- Replace=abc --|abc|-- /Replace=abc --|abc|".ReplacePipeToNewLine(), (a, b) => "DEF");
                Assert.True(changed);
                Assert.Equal("-- Replace=abc --|DEF|-- /Replace=abc --|abc|", content.ReplaceNewLineToPipe());
                (changed, content) = ReplacementBindingExtension.Replace(content, (a, b) => "DEF");
                Assert.False(changed);
                Assert.Equal("-- Replace=abc --|DEF|-- /Replace=abc --|abc|", content.ReplaceNewLineToPipe());
            }
            {
                var fileContent = "-- Replace=abc --|abc|-- /Replace=abc --|abc|".ReplacePipeToNewLine();
                var (changed, content) = ReplacementBindingExtension.Replace(fileContent, (a, b) => String.Empty);
                Assert.False(changed);
                Assert.Equal(fileContent, content);
            }
            {
                var fileContent = "  sof|  -- Replace=abc --|  abc|  -- /Replace=abc --|  eof|".ReplacePipeToNewLine();
                var (changed, content) = ReplacementBindingExtension.Replace(fileContent, (a, b) => (new string(' ', b) + "DEF" + System.Environment.NewLine));
                Assert.True(changed);
                Assert.Equal("  sof|  -- Replace=abc --|  DEF|  -- /Replace=abc --|  eof|", content.ReplaceNewLineToPipe());
            }
            {
                var fileContent = "  sof|  -- Replace=abc --|  abc|  -- /Replace=abc --|  eof|".ReplacePipeToNewLine();
                var (changed, content) = ReplacementBindingExtension.Replace(fileContent, (a, b) => String.Empty);
                Assert.False(changed);
                Assert.Equal(fileContent, content);
            }
        }

        [Fact()]
        public void Replace2Test() {
            {
                var (changed, content) = ReplacementBindingExtension.Replace("/*-- Replace=abc --*/|abc|/*-- /Replace=abc --*/|abc|".ReplacePipeToNewLine(), (a, b) => "DEF");
                Assert.True(changed);
                Assert.Equal("/*-- Replace=abc --*/|DEF|/*-- /Replace=abc --*/|abc|", content.ReplaceNewLineToPipe());
                (changed, content) = ReplacementBindingExtension.Replace(content, (a, b) => "DEF");
                Assert.False(changed);
                Assert.Equal("/*-- Replace=abc --*/|DEF|/*-- /Replace=abc --*/|abc|", content.ReplaceNewLineToPipe());
            }
            {
                var fileContent = "/*-- Replace=abc --*/|abc|/*-- /Replace=abc --*/|abc|".ReplacePipeToNewLine();
                var (changed, content) = ReplacementBindingExtension.Replace(fileContent, (a, b) => String.Empty);
                Assert.False(changed);
                Assert.Equal(fileContent, content);
            }
            {
                var fileContent = "  sof|  /*-- Replace=abc --*/|  abc|  /*-- /Replace=abc --*/|  eof|".ReplacePipeToNewLine();
                var (changed, content) = ReplacementBindingExtension.Replace(fileContent, (a, b) => (new string(' ', b) + "DEF" + System.Environment.NewLine));
                Assert.True(changed);
                Assert.Equal("  sof|  /*-- Replace=abc --*/|  DEF|  /*-- /Replace=abc --*/|  eof|", content.ReplaceNewLineToPipe());
            }
            {
                var fileContent = "  sof|  /*-- Replace=abc --*/|  abc|  /*-- /Replace=abc --*/|  eof|".ReplacePipeToNewLine();
                var (changed, content) = ReplacementBindingExtension.Replace(fileContent, (a, b) => String.Empty);
                Assert.False(changed);
                Assert.Equal(fileContent, content);
            }
        }
    }
}