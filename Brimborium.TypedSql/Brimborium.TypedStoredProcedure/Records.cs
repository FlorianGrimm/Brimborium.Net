namespace Brimborium.TypedStoredProcedure {
    record ReaderDefinition(
        string ReaderName,
        StoredProcedureResultSet ResultSet,
        CSTypeDefinition spDef_Return,
        string csReturnTypeRecord
        );

    record ReturnTypeNames(
        string csCompleteReturnType,
        string csReturnTypeRecord,
        string csReturnTypeRecordQ
        );

    record MappingReturnItemCSCode(
        CalculateMappingReturnItem MappingReturnItem,
        string ReadCSCode,
        string CSVariableName
        ) {
        public string Identity => this.MappingReturnItem.Column.Name;
        public int Index => this.MappingReturnItem.Column.Index;
    }

    record MappingResults(
        string PropertyName,
        int Index,
        bool IsMapped
        );
}
