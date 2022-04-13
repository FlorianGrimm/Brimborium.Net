namespace Brimborium.TypedStoredProcedure {
    public sealed record StoredProcedureDefintion(
            string Schema,
            string Name,
            CSTypeDefinition Argument,
            ExecutionMode ExecutionMode,
            CSTypeDefinition Return) {
        public bool Enabled => !(
               this.ExecutionMode == ExecutionMode.Ignore
            || this.ExecutionMode == ExecutionMode.Obsolete
            || this.ExecutionMode == ExecutionMode.Unknown
            || this.ExecutionMode == ExecutionMode.Unsure
            );
        public string SqlName => $"[{this.Schema}].[{this.Name}]";
    }
}
