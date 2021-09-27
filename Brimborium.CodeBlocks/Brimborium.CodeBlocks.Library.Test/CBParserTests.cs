#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Xunit;

namespace Brimborium.CodeBlocks.Library.Test {
    public class CBParserTests {
        [Fact]
        public void Tokenize_001() {
            var i = @"
aa
  /*<<hugo--*/
bb
  /*--hugo>>*/
cc/*<<gna--*/dd/*--gna>>*/ee

";

            var o = @"
aa
/*<<hugo:abc--*/
dd
/*--hugo>>*/
cc
";
            var sut =new CBParser();
            var act= sut.Tokenize(i);
            Assert.Equal(9, act.Count);

            // 0
            Assert.Equal(CBParserResultKind.Fixed, act[0].Kind);
            Assert.Equal(@"
aa", act[0].Text);

            // 1
            Assert.Equal(CBParserResultKind.Replacement, act[1].Kind);
            Assert.Equal(@"
  ", act[1].PrefixWS);
            Assert.Equal(true, act[1].Start);
            Assert.Equal("hugo", act[1].Text);
            Assert.Equal(false, act[1].Finish);
            Assert.Equal(9, act.Count);

            // 2
            Assert.Equal(CBParserResultKind.Fixed, act[2].Kind);
            Assert.Equal(@"
bb", act[2].Text);

            // 3
            Assert.Equal(CBParserResultKind.Replacement, act[3].Kind);
            Assert.Equal(@"
  ", act[1].PrefixWS);
            Assert.Equal(false, act[3].Start);
            Assert.Equal("hugo", act[3].Text);
            Assert.Equal(true, act[3].Finish);

            //var a = new List<CBValue>();
            //a.Add(new CBValue<string>("hugo", "dd"));
        }
    }
}
