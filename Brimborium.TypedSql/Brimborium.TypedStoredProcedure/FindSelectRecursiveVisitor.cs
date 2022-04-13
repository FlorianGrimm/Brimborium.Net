
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;

using System.Collections.Generic;
using System.Linq;

namespace Brimborium.TypedStoredProcedure {
    internal class FindSelectRecursiveVisitor : SqlCodeObjectRecursiveVisitor {
        private readonly Stack<FindSelectResult> _Results;

        public FindSelectRecursiveVisitor(FindSelectResult result) {
            this._Results = new Stack<FindSelectResult>();
            this._Results.Push(result);

        }

        public override void Visit(SqlCompoundStatement statement) {
            var subResult = new FindSelectResult(new List<FindSelectResult>(), true, null);
            this._Results.Push(subResult);
            base.Visit(statement);
            var subResult2 = this._Results.Pop();
            if (!ReferenceEquals(subResult, subResult2)) { throw new InvalidOperandException(); }
            if (subResult.HasChildrenOrSelect()) {
                var currentResult = this._Results.Peek();
                currentResult.Children.Add(subResult);
                //if (subResult.HasOnlySelect()) {
                //    currentResult.Children.Add(subResult);
                //} else if (subResult.HasOnlyOneChild()) {
                //    currentResult.Children.Add(subResult.Children.Single());
                //} else if (currentResult.IsSequential == subResult.IsSequential) {
                //    currentResult.Children.AddRange(subResult.Children);
                //}
            } else {
                // ignore empty
            }
        }
        public override void Visit(SqlCursorDeclareStatement statement) {
            // base.Visit(statement);
        }
        public override void Visit(SqlVariableDeclareStatement statement) {
            // base.Visit(statement);
        }
        public override void Visit(SqlExecuteModuleStatement statement) {
            // TODO
            // base.Visit(statement);
        }
        public override void Visit(SqlSelectStatement statement) {
            //base.Visit(statement);
            if (statement.SelectSpecification.QueryExpression is SqlQuerySpecification queryExpression) {
                if (queryExpression.SelectClause.SelectExpressions.Any(se => se is SqlSelectScalarExpression)) {
                    var currentResult = this._Results.Peek();
                    currentResult.Children.Add(new FindSelectResult(new List<FindSelectResult>(), true, statement));
                }
            }
        }

        public override void Visit(SqlIfElseStatement statement) {
            //base.Visit(statement);
            var subResult = new FindSelectResult(new List<FindSelectResult>(), false, null);
            var subResultTrue = new FindSelectResult(new List<FindSelectResult>(), true, null);
            var subResultFalse = new FindSelectResult(new List<FindSelectResult>(), true, null);
            this._Results.Push(subResult);
            // visitor?.Visit(statement);
            this._Results.Push(subResultTrue);
            statement.TrueStatement.Accept(this);
            var subResultTrue2 = this._Results.Pop();
            if (!ReferenceEquals(subResultTrue, subResultTrue2)) { throw new InvalidOperandException(); }

            if (statement.FalseStatement != null) {
                this._Results.Push(subResultFalse);
                statement.FalseStatement.Accept(this);
                var subResultFalse2 = this._Results.Pop();
                if (!ReferenceEquals(subResultFalse, subResultFalse2)) { throw new InvalidOperandException(); }
            }
            var subResult2 = this._Results.Pop();
            if (!ReferenceEquals(subResult, subResult2)) { throw new InvalidOperandException(); }

            if (subResultTrue.HasChildrenOrSelect()) {
                //if (subResultTrue.HasOnlyOneChild()) {
                //    subResult.Children.Add(subResultTrue.Children.Single());
                //} else if (subResultTrue.HasOnlySelect()) {
                //    subResult.Children.Add(subResultTrue);
                //} else if (subResultTrue.HasOnlyChildren(c => c.IsSequential == false)) {
                //    subResult.Children.AddRange(subResultTrue.Children);
                //} else {
                //    subResult.Children.Add(subResultTrue);
                //}
                subResult.Children.Add(subResultTrue);
            }
            if (subResultFalse.HasChildrenOrSelect()) {
                //if (subResultFalse.HasOnlyOneChild()) {
                //    subResult.Children.Add(subResultFalse.Children.Single());
                //} else if (subResultFalse.HasOnlySelect()) {
                //    subResult.Children.Add(subResultFalse);
                //} else if (subResultFalse.HasOnlyChildren(c => c.IsSequential == false)) {
                //    subResult.Children.AddRange(subResultFalse.Children);
                //} else {
                //    subResult.Children.Add(subResultFalse);
                //}
                subResult.Children.Add(subResultFalse);
            }
            var currentResult = this._Results.Peek();
            if (subResult.HasChildrenOrSelect()) {
                //if (subResult.HasOnlyOneChild()) {
                //    currentResult.Children.Add(subResult.Children.Single());
                //} else {
                //    currentResult.Children.Add(subResult);
                //}
                currentResult.Children.Add(subResult);
            }
        }
    }
}
