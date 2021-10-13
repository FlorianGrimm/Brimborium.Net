#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions
#pragma warning disable CS0219 // Variable is assigned but its value is never used

using Xunit;

namespace Brimborium.CodeBlocks.Library.Test {
    public class CBParserTests {
        [Fact]
        public void Tokenize_001Tokenize() {
            var i = @"
aa
  /*<< hugo --*/
bb
  /*--hugo>>*/
cc/*<<gna--*/dd/*--gna>>*/ee

";

#if false
            var o = @"
aa
/*<<hugo:abc--*/
dd
/*--hugo>>*/
cc
";
#endif
            var sut = new CBParser();
            var act = sut.Tokenize(i);
            Assert.Equal(9, act.Count);

            // 0
            Assert.Equal(CBParserResultKind.Fixed, act[0].Kind);
            Assert.Equal(@"
aa
  ", act[0].Text);

            // 1
            Assert.Equal(CBParserResultKind.Replacement, act[1].Kind);
            Assert.Equal(@"  ", act[1].IndentWS);
            Assert.Equal(true, act[1].Start);
            Assert.Equal("hugo", act[1].Text);
            Assert.Equal(false, act[1].Finish);
            Assert.Equal(9, act.Count);

            // 2
            Assert.Equal(CBParserResultKind.Fixed, act[2].Kind);
            Assert.Equal(@"
bb
  ", act[2].Text);

            // 3
            Assert.Equal(CBParserResultKind.Replacement, act[3].Kind);
            Assert.Equal(@"  ", act[1].IndentWS);
            Assert.Equal(false, act[3].Start);
            Assert.Equal("hugo", act[3].Text);
            Assert.Equal(true, act[3].Finish);

            //var a = new List<CBValue>();
            //a.Add(new CBValue<string>("hugo", "dd"));
        }

        [Fact]
        public void Tokenize_002_Parse() {
            var i = @"
aa
  /*<<hugo--*/
  bb
  /*--hugo>>*/
cc/*<<gna--*/dd/*--gna>>*/ee

";
            var sut = new CBParser();
            var tokens = sut.Tokenize(i);
            var act = sut.Parse(tokens);

            Assert.Null(act.StartToken);
            Assert.Null(act.ContentToken);
            Assert.Null(act.FinishToken);
            Assert.True(act.Items.Count >= 2);

            Assert.Equal(@"
aa
  ", act.Items[0].ToString());

            Assert.NotNull(act.Items[1].StartToken);
            Assert.Null(act.Items[1].ContentToken);
            Assert.NotNull(act.Items[1].FinishToken);
            Assert.Equal(@"/*<< hugo --*/", act.Items[1].StartToken?.ToString());
            Assert.Equal(@"/*-- hugo >>*/", act.Items[1].FinishToken?.ToString());
            Assert.Equal(@"
  bb
  ", act.Items[1].Items[0].ContentToken?.ToString());

            Assert.Null(act.Items[2].StartToken);
            Assert.NotNull(act.Items[2].ContentToken);
            Assert.Null(act.Items[2].FinishToken);
            Assert.Equal(@"
cc", act.Items[2].ToString());

            Assert.NotNull(act.Items[3].StartToken);
            Assert.Null(act.Items[3].ContentToken);
            Assert.NotNull(act.Items[3].Items[0].ContentToken);
            Assert.NotNull(act.Items[3].FinishToken);
            Assert.Equal(@"/*<< gna --*/", act.Items[3].StartToken?.ToString());
            Assert.Equal(@"/*-- gna >>*/", act.Items[3].FinishToken?.ToString());
            Assert.Equal(@"dd", act.Items[3].Items[0].ContentToken?.ToString());

            Assert.Null(act.Items[4].StartToken);
            Assert.NotNull(act.Items[4].ContentToken);
            Assert.Null(act.Items[4].FinishToken);
            Assert.Equal(@"ee

", act.Items[4].ToString());

            Assert.Equal(5, act.Items.Count);

        }

        [Fact]
        public void Tokenize_003_OneReplacement() {
            var generatedCode = @"aaa2 {
  /*<<hugo--*/
    bbbGenerated
  /*--hugo>>*/
}
ccc2
";
            var oldCode = @"aaa1 {
  /*<<hugo--*/
    bbb1
  /*--hugo>>*/
}
ccc1
";
            var expectedCode = @"aaa2 {
  /*<< hugo --*/
    bbb1
  /*-- hugo >>*/
}
ccc2
";
            var copyReplacer = new CBCopyReplacer();
            var actCode = copyReplacer.ContentCopyReplace(generatedCode, oldCode);
            Assert.Equal(removeWS(expectedCode), removeWS(actCode));
            //Assert.Equal(expectedCode, actCode);
        }

        [Fact]
        public void Tokenize_004_TwoReplacement() {
            var generatedCode = @"aaa2 {
  /*<<hugo--*/
    bbbGenerated
  /*--hugo>>*/
}
ccc2 {
  /*<<gna--*/
    bbbGenerated
  /*--gna>>*/
} eee2
";
            var oldCode = @"aaa1 {
  /*<<hugo--*/
    bbb1
  /*--hugo>>*/
}
ccc1 {
  /*<< gna --*/
    ddd1
  /*-- gna >>*/
} eee1
";

            var expectedCode = @"aaa2 {
  /*<< hugo --*/
    bbb1
  /*-- hugo >>*/
}
ccc2 {
  /*<< gna --*/
    ddd1
  /*-- gna >>*/
} eee2
";
            var copyReplacer = new CBCopyReplacer();
            var actCode = copyReplacer.ContentCopyReplace(generatedCode, oldCode);
            Assert.Equal(removeWS(expectedCode), removeWS(actCode));
            Assert.Equal(expectedCode, actCode);
        }


        [Fact]
        public void Tokenize_005_OneMoreReplacementBefore() {
            var generatedCode = @"aaa2 {
  /*<<hugo--*/
    hugoGenerated
  /*--hugo>>*/
}
ccc2 {
  /*<<gna--*/
    gnaGenerated
  /*--gna>>*/
} eee2
";
            var oldCode = @"
ccc1 {
  /*<< gna --*/
    ddd1
  /*-- gna >>*/
} eee1
";
            var expectedCode = @"aaa2 {
  /*<< hugo --*/
    hugoGenerated
  /*-- hugo >>*/
}
ccc2 {
  /*<< gna --*/
    ddd1
  /*-- gna >>*/
} eee2
";
            var copyReplacer = new CBCopyReplacer();
            var actCode = copyReplacer.ContentCopyReplace(generatedCode, oldCode);
            Assert.Equal(removeWS(expectedCode), removeWS(actCode));
            Assert.Equal(expectedCode, actCode);
        }

        [Fact]
        public void Tokenize_006_OneMoreReplacementAfter() {
            var generatedCode = @"aaa2 {
  /*<<hugo--*/
    hugoGenerated
  /*--hugo>>*/
}
ccc2 {
  /*<<gna--*/
    gnaGenerated
  /*--gna>>*/
} eee2
";
            var oldCode = @"aaa1 {
  /*<<hugo--*/
    bbb1
  /*--hugo>>*/
}
ccc1
eee1
";
            var expectedCode = @"aaa2 {
  /*<< hugo --*/
    bbb1
  /*-- hugo >>*/
}
ccc2 {
  /*<< gna --*/
    gnaGenerated
  /*-- gna >>*/
} eee2
";
            var copyReplacer = new CBCopyReplacer();
            var actCode = copyReplacer.ContentCopyReplace(generatedCode, oldCode);
            Assert.Equal(removeWS(expectedCode), removeWS(actCode));
            Assert.Equal(expectedCode, actCode);
        }
        static string removeWS(string s) {
            return s.Replace('\r', ' ').Replace('\n', ' ').Replace("    ", " ").Replace("   ", " ").Replace("  ", " ");
        }
    }
}
