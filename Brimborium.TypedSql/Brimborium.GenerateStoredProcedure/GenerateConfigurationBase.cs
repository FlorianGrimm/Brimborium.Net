﻿namespace Brimborium.GenerateStoredProcedure {
    public class GenerateConfigurationBase {
        public readonly KnownTemplates KnownTemplates;
        //public readonly List<ReplacementTemplate<TableInfo>> ReplacementTableTemplates;

        public GenerateConfigurationBase() {
            this.KnownTemplates = new KnownTemplates();
            //this.ReplacementTableTemplates = new List<ReplacementTemplate<TableInfo>>();

        }

        public virtual ConfigurationBound Build(DatabaseInfo databaseInfo, bool isVerbose) {
            var result = new ConfigurationBound();
            this.AddKnownReplacementBindings(databaseInfo, result);
            return result;
        }

        public virtual void AddKnownReplacementBindings(DatabaseInfo databaseInfo, ConfigurationBound result) {
            result.AddReplacementBindings(
                nameof(KnownTemplates.ColumnRowversion), 
                databaseInfo.Tables.Select(t=>new TableBinding(t, this.KnownTemplates.ColumnRowversion)));
            result.AddReplacementBindings(
                nameof(KnownTemplates.SelectTableColumns), 
                databaseInfo.Tables.Select(t=>new TableBinding(t, this.KnownTemplates.SelectTableColumns)));
            result.AddReplacementBindings(
                nameof(KnownTemplates.TableColumnsAsParameter),
                databaseInfo.Tables.Select(t=>new TableBinding(t, this.KnownTemplates.TableColumnsAsParameter)));
        }
    }
}
