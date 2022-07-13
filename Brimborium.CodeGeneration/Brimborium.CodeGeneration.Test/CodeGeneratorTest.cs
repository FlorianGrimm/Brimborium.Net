#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
namespace Brimborium.CodeGeneration {
    public class CodeGeneratorTest {
        [Fact]
        public void CodeGenerator_1_Customize_off_Test() {
            List<FileContent> lstFileContent = new List<FileContent>();
            TestFileContentService testFileContentService = new TestFileContentService();
            Dictionary<string, string> templateVariables = new Dictionary<string, string>();
            CodeGeneratorBindings codeGeneratorBindings = new CodeGeneratorBindings();
            Action<string>? log = (msg) => { };
            bool isVerbose = true;
            List<string> names = new List<string>() { "abc", "def" };
            codeGeneratorBindings.AddRenderBindings("CodeGenerator_1_Test",
                names.Select(n => new RenderBinding<string>(
                n, new RenderTemplate<string>(
                    Render:(data, ctxt) => {
                        ctxt.AppendHeader();
                        ctxt.AppendPartsLine("public partial class ", data," {");
                        var ctxt2 = ctxt.GetIndented();
                        ctxt2.AppendPartsLine("public ",data,"() {");
                        var ctxt3 = ctxt2.GetIndented();
                        ctxt3.AppendPartsLine("//");
                        ctxt3.AppendCustomize("ctor");
                        ctxt2.AppendPartsLine("}");
                        ctxt.AppendPartsLine("}");
                    },
                    FileNameFn: (data, b) => $"{data}.cs"
                    ))));
            CodeGenerator.Generate("", lstFileContent, templateVariables, codeGeneratorBindings, log, isVerbose, testFileContentService);
           
            Assert.Equal(2, testFileContentService.DictFileContent.Count);
           
            Assert.True(testFileContentService.DictFileContent.ContainsKey("abc.cs"));
            Assert.True(testFileContentService.DictFileContent.ContainsKey("def.cs"));
            
            Assert.Equal(
                "/*-- AutoGenerate:on --*/|/*-- Customize:off --*/|public partial class abc {|    public abc() {|        //|    }|}|",
                testFileContentService.DictFileContent["abc.cs"]!.Content.ReplaceNewLineToPipe());
            Assert.Equal(
                "/*-- AutoGenerate:on --*/|/*-- Customize:off --*/|public partial class def {|    public def() {|        //|    }|}|",
                testFileContentService.DictFileContent["def.cs"]!.Content.ReplaceNewLineToPipe());
        }

        [Fact]
        public void CodeGenerator_2_Customize_on_nocontent_Test() {
            List<FileContent> lstFileContent = new List<FileContent>();
            TestFileContentService testFileContentService = new TestFileContentService();
            Dictionary<string, string> templateVariables = new Dictionary<string, string>();
            CodeGeneratorBindings codeGeneratorBindings = new CodeGeneratorBindings();
            Action<string>? log = (msg) => { };
            bool isVerbose = true;
            List<string> names = new List<string>() { "abc", "def" };
            codeGeneratorBindings.AddRenderBindings("CodeGenerator_2_Test",
                names.Select(n => new RenderBinding<string>(
                n, new RenderTemplate<string>(
                    Render: (data, ctxt) => {
                        ctxt.AppendHeader();
                        ctxt.AppendPartsLine("public partial class ", data, " {");
                        var ctxt2 = ctxt.GetIndented();
                        ctxt2.AppendPartsLine("public ", data, "() {");
                        var ctxt3 = ctxt2.GetIndented();
                        ctxt3.AppendPartsLine("//");
                        ctxt3.AppendCustomize("ctor");
                        ctxt2.AppendPartsLine("}");
                        ctxt.AppendPartsLine("}");
                    },
                    FileNameFn: (data, b) => $"{data}.cs"
                    ))));
            testFileContentService.Add(
                "abc.cs",
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|public partial class abc {|    public abc() {|        //|    }|}|".ReplacePipeToNewLine()
                );
            testFileContentService.Add(
                "def.cs",
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|public partial class def {|    public def() {|        //|    }|}|".ReplacePipeToNewLine()
                );
            CodeGenerator.Generate("", lstFileContent, templateVariables, codeGeneratorBindings, log, isVerbose, testFileContentService);
           
            Assert.Equal(2, testFileContentService.DictFileContent.Count);
            Assert.True(testFileContentService.DictFileContent.ContainsKey("abc.cs"));
            Assert.True(testFileContentService.DictFileContent.ContainsKey("def.cs"));
           
            Assert.Equal(
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|public partial class abc {|    public abc() {|        //|        /*-- Customize=ctor --*/|        /*-- /Customize=ctor --*/|    }|}|",
                testFileContentService.DictFileContent["abc.cs"]!.Content.ReplaceNewLineToPipe());
            Assert.Equal(
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|public partial class def {|    public def() {|        //|        /*-- Customize=ctor --*/|        /*-- /Customize=ctor --*/|    }|}|",
                testFileContentService.DictFileContent["def.cs"]!.Content.ReplaceNewLineToPipe());
        }


        [Fact]
        public void CodeGenerator_3_Customize_on_withcontent_Test() {
            List<FileContent> lstFileContent = new List<FileContent>();
            TestFileContentService testFileContentService = new TestFileContentService();
            Dictionary<string, string> templateVariables = new Dictionary<string, string>();
            CodeGeneratorBindings codeGeneratorBindings = new CodeGeneratorBindings();
            Action<string>? log = (msg) => { };
            bool isVerbose = true;
            List<string> names = new List<string>() { "abc", "def" };
            codeGeneratorBindings.AddRenderBindings("CodeGenerator_3_Test",
                names.Select(n => new RenderBinding<string>(
                n, new RenderTemplate<string>(
                    Render: (data, ctxt) => {
                        ctxt.AppendHeader();
                        ctxt.AppendPartsLine("public partial class ", data, " {");
                        var ctxt2 = ctxt.GetIndented();
                        ctxt2.AppendPartsLine("public ", data, "() {");
                        var ctxt3 = ctxt2.GetIndented();
                        ctxt3.AppendPartsLine("//");
                        ctxt3.AppendCustomize("ctor");
                        ctxt2.AppendPartsLine("}");
                        ctxt.AppendPartsLine("}");
                    },
                    FileNameFn: (data, b) => $"{data}.cs"
                    ))));

            testFileContentService.Add(
                "abc.cs",
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|public partial class abc {|    public abc(int gone) {|        //|        /*-- Customize=ctor --*/|        dosomthing();|        /*-- /Customize=ctor --*/|    }|}|".ReplacePipeToNewLine()
                );
            testFileContentService.Add(
                "def.cs",
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|public partial class def {|    public def(int gone) {|        //|        /*-- Customize=ctor --*/|        dosomthing();|        /*-- /Customize=ctor --*/|    }|}|".ReplacePipeToNewLine()
                );
            lstFileContent.AddRange(testFileContentService.DictFileContent.Values);
            Assert.Equal(2, lstFileContent.Count);
            
            CodeGenerator.Generate("", lstFileContent, templateVariables, codeGeneratorBindings, log, isVerbose, testFileContentService);
            
            Assert.Equal(2, testFileContentService.DictFileContent.Count);
            Assert.True(testFileContentService.DictFileContent.ContainsKey("abc.cs"));
            Assert.True(testFileContentService.DictFileContent.ContainsKey("def.cs"));
           
            Assert.Equal(
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|public partial class abc {|    public abc() {|        //|        /*-- Customize=ctor --*/|        dosomthing();|        /*-- /Customize=ctor --*/|    }|}|",
                testFileContentService.DictFileContent["abc.cs"]!.Content.ReplaceNewLineToPipe());
            Assert.Equal(
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|public partial class def {|    public def() {|        //|        /*-- Customize=ctor --*/|        dosomthing();|        /*-- /Customize=ctor --*/|    }|}|",
                testFileContentService.DictFileContent["def.cs"]!.Content.ReplaceNewLineToPipe());
        }

        [Fact]
        public void CodeGenerator_4_AutoGenerate_off_Test() {
            List<FileContent> lstFileContent = new List<FileContent>();
            TestFileContentService testFileContentService = new TestFileContentService();
            Dictionary<string, string> templateVariables = new Dictionary<string, string>();
            CodeGeneratorBindings codeGeneratorBindings = new CodeGeneratorBindings();
            Action<string>? log = (msg) => { };
            bool isVerbose = true;
            List<string> names = new List<string>() { "abc", "def" };
            codeGeneratorBindings.AddRenderBindings("CodeGenerator_4_Test",
                names.Select(n => new RenderBinding<string>(
                n, new RenderTemplate<string>(
                    Render: (data, ctxt) => {
                        ctxt.AppendHeader();
                        ctxt.AppendPartsLine("new");
                    },
                    FileNameFn: (data, b) => $"{data}.cs"
                    ))));
            testFileContentService.Add(
                "abc.cs",
                "/*-- AutoGenerate:off --*/|/*-- Customize:on --*/|old|".ReplacePipeToNewLine()
                );
            testFileContentService.Add(
                "def.cs",
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|old|".ReplacePipeToNewLine()
                );
            lstFileContent.AddRange(testFileContentService.DictFileContent.Values);
            Assert.Equal(2, lstFileContent.Count);
            
            CodeGenerator.Generate("", lstFileContent, templateVariables, codeGeneratorBindings, log, isVerbose, testFileContentService);
            
            Assert.Equal(2, testFileContentService.DictFileContent.Count);
            Assert.True(testFileContentService.DictFileContent.ContainsKey("abc.cs"));
            Assert.True(testFileContentService.DictFileContent.ContainsKey("def.cs"));

            Assert.Equal(
                "/*-- AutoGenerate:off --*/|/*-- Customize:on --*/|old|",
                testFileContentService.DictFileContent["abc.cs"]!.Content.ReplaceNewLineToPipe());
            Assert.Equal(
                "/*-- AutoGenerate:on --*/|/*-- Customize:on --*/|new|",
                testFileContentService.DictFileContent["def.cs"]!.Content.ReplaceNewLineToPipe());
        }
    }
}
