using System;

using Xunit;

namespace Brimborium.Typing.Test {
    public class UnitTest1 {
        [Fact]
        public void Test1() {
            //var typingRepository = new TypingRepository();
            //var sut = new DotNetTyping(typingRepository);
            //sut.ScanType(typeof(ClassTwo));
        }
    }
    public class ClassOne {
        public ClassOne() {
            this.Name = string.Empty;
        }
        public string Name { get; set; }
    }
    public class ClassTwo:ClassOne {
        public ClassTwo() {
            this.Value = string.Empty;
        }
        public string Value { get; set; }
        public string? Ebbes { get; set; }
    }
}
