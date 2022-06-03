#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
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
                Assert.Equal(24, act.start);
                Assert.Equal("-- /Replace=abc --", act.tokenStop);
            }
            {
                var fileContent = "  sof|  -- Replace=abc --|  abc|  -- /Replace=abc --|  eof|".ReplacePipeToNewLine();
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(37, act.start);
                Assert.Equal("-- /Replace=abc --", act.tokenStop);
            }
        }

        [Fact()]
        public void IndexOfReplaceStop2Test() {
            {
                var fileContent = "/*-- Replace=abc --*/|abc|/*-- /Replace=abc --*/|abc|".ReplacePipeToNewLine();
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(28, act.start);
                Assert.Equal("/*-- /Replace=abc --*/", act.tokenStop);
            }

            {
                var fileContent = "  sof|  /*-- Replace=abc --*/|  abc|  /*-- /Replace=abc --*/|  eof|".ReplacePipeToNewLine();
                var rs = ReplacementBindingExtension.IndexOfReplaceStart(fileContent, 0);
                var act = ReplacementBindingExtension.IndexOfReplaceStop(fileContent, rs);
                Assert.Equal(41, act.start);
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
                var (changed, content) = ReplacementBindingExtension.Replace("-- Replace=abc --|abc|-- /Replace=abc --|abc|".ReplacePipeToNewLine(), (a, b) => "DEF|".ReplacePipeToNewLine());
                Assert.True(changed);
                Assert.Equal("-- Replace=abc --|DEF|-- /Replace=abc --|abc|", content.ReplaceNewLineToPipe());
                (changed, content) = ReplacementBindingExtension.Replace(content, (a, b) => "DEF|".ReplacePipeToNewLine());
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
                var (changed, content) = ReplacementBindingExtension.Replace("/*-- Replace=abc --*/|abc|/*-- /Replace=abc --*/|abc|".ReplacePipeToNewLine(), (a, b) => "DEF|".ReplacePipeToNewLine());
                Assert.True(changed);
                Assert.Equal("/*-- Replace=abc --*/|DEF|/*-- /Replace=abc --*/|abc|", content.ReplaceNewLineToPipe());
                (changed, content) = ReplacementBindingExtension.Replace(content, (a, b) => "DEF|".ReplacePipeToNewLine());
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

        [Fact()]
        public void IndexOfCustomizeStartTest() {
            var content = "aaaa|/*-- Customize=abc --*/|bbbb|cccc|/*-- /Customize=abc --*/|dddd".ReplacePipeToNewLine();
            var positionStart = ReplacementBindingExtension.IndexOfCustomizeStart(content, 0);
            Assert.True(positionStart.start >= 0, "positionStart");
            var positionStop = ReplacementBindingExtension.IndexOfCustomizeStop(content, positionStart);
            Assert.True(positionStop.start >= 0, "positionStop");
            var contentCustomizeStart = positionStart.start + positionStart.len;
            var contentCustomize = content.Substring(contentCustomizeStart, positionStop.start - contentCustomizeStart);
            Assert.Equal("bbbb|cccc|", contentCustomize.ReplaceNewLineToPipe());
        }

        [Fact()]
        public void ReadCustomizeTest() {
            var content = "/*-- Customize:on --*/|/*-- Customize=abc --*/|bbbb|cccc|/*-- /Customize=abc --*/|dddd".ReplacePipeToNewLine();
            var flags = ReplacementBindingExtension.ReadFlags(content);
            var act = ReplacementBindingExtension.ReadCustomize(content);
            Assert.Equal(1, act.Count);
            Assert.True(act.ContainsKey("abc"));
            Assert.Equal("bbbb|cccc|", act["abc"].ReplaceNewLineToPipe());
        }

        [Fact()]
        public void ReadFlagsTest() {
            var content = "/*-- AutoGenerate:on --*/|/*-- Customize:off --*/|bbbb|cccc|/*-- /Customize=abc --*/|dddd".ReplacePipeToNewLine();
            var act = ReplacementBindingExtension.ReadFlags(content);
            Assert.Equal(3, act.Count);
            Assert.Equal("on", act["AutoGenerate"]);
            Assert.Equal("off", act["Customize"]);
        }
    }
}