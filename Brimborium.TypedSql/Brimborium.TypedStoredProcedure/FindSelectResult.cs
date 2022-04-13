
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brimborium.TypedStoredProcedure {
    public sealed record FindSelectResult(List<FindSelectResult> Children, bool IsSequential, SqlSelectStatement? SelectStatement) {
        public bool HasChildrenOrSelect() => this.Children.Count > 0 || this.SelectStatement is object;
        public bool HasOnlyChildren() => this.Children.Count > 0 && this.SelectStatement is null;
        public bool HasOnlySelect() => this.Children.Count == 0 && this.SelectStatement is object;
        public bool HasOnlyChildren(Func<FindSelectResult, bool> predicate) => this.Children.Count > 0 && this.SelectStatement is null && this.Children.All(predicate);
        public bool HasOnlyOneChild() => this.Children.Count == 1 && this.SelectStatement is null;
        public bool HasOnlyOneChild(Func<FindSelectResult, bool> predicate) => this.Children.Count == 1 && this.SelectStatement is null && this.Children.All(predicate);
        public int CountSelects() {
            if (this.SelectStatement is object) { return 1; }
            var result = 0;
            foreach (var child in this.Children) {
                result += child.CountSelects();
            }
            return result;
        }
    }
}
