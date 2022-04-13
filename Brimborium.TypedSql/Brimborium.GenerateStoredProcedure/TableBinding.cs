using System;
using System.Collections.Generic;

namespace Brimborium.GenerateStoredProcedure {
    public record TableBinding(
        TableInfo Data,
        RenderTemplate<TableInfo> Template
        ) : TemplateBinding<TableInfo>(Data, Template) {
        public override string GetFilename(Dictionary<string, string> boundVariables) {
            boundVariables["Schema"] = this.Data.Table.Schema;
            boundVariables["Name"] = this.Data.Table.Name;
            return this.Template.GetFilename(this.Data, boundVariables);
        }
    }

    public record DataTemplateBinding<T>(
        T Data,
        Func<T, TableInfo> fnData,
        RenderTemplate<T> Template
        ) : TemplateBinding<T>(Data, Template)
        where T : notnull {
        public override string GetFilename(Dictionary<string, string> boundVariables) {
            var tableInfo = fnData(Data);
            boundVariables["Schema"] = tableInfo.Schema;
            boundVariables["Name"] = tableInfo.Name;
            return this.Template.GetFilename(this.Data, boundVariables);
        }
    }

    /*
    public record TableBindingPairedArgument<TArgument>(
        TableInfo Data,
        TableInfo Related,
        TArgument Argument,
        RenderTemplate<TableInfo> Template
        ) : TableBinding(Data, Template) {
        public override string GetFilename(Dictionary<string, string> boundVariables) {
            boundVariables["Schema"] = this.Data.Table.Schema;
            boundVariables["Name"] = this.Data.Table.Name;
            return this.Template.GetFilename(this.Data, boundVariables);
        }
    }
    */

}
