﻿namespace Brimborium.CodeGeneration {
    public class FileContentTests {
        [Fact()]
        public void HasChanged_simple_Test() {
            var sut = new FileContent("a", "a");
            Assert.False(sut.HasChanged("a"));
            Assert.True(sut.HasChanged("b"));
            Assert.True(sut.HasChanged("a|".Replace("|", System.Environment.NewLine)));
        }

        [Fact()]
        public void HasChanged_iffalse_Test() {
            var sut = new FileContent("a", "#if false|abc|#endif|".Replace("|", System.Environment.NewLine));
            Assert.False(sut.HasChanged("a"));
            Assert.False(sut.HasChanged("b"));
        }

        [Fact()]
        public void HasChanged_not_iffalse_Test() {
            var sut = new FileContent("a", "#if true|abc|#endif|".Replace("|", System.Environment.NewLine));
            Assert.True(sut.HasChanged("a"));
            Assert.True(sut.HasChanged("b"));
        }
    }
}