using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.CodeBlocks.Library.Test {
    public class CBCodeTypeTests {
        [Fact]
        public void Test001_int() {
            var sutObject = CBCodeType.FromClr(type: typeof(object));
            var sutInt=CBCodeType.FromClr(type: typeof(int));
            Assert.Same(sutObject, sutInt.BaseType);
        }

        [Fact]
        public void Test002_List_int() {
            var sutObject = CBCodeType.FromClr(type: typeof(object));
            var sutInt = CBCodeType.FromClr(type: typeof(int));
            var sutListOf = CBCodeType.FromClr(type: typeof(List<>));
            var sutListInt = CBCodeType.FromClr(type: typeof(List<int>));
            
            Assert.Same(sutObject, sutInt.BaseType);
            Assert.Same(sutListOf, sutInt.GenericTypeDefinition);
            Assert.Same(sutListOf, sutInt.GenericTypeDefinition);
        }
    }
}
