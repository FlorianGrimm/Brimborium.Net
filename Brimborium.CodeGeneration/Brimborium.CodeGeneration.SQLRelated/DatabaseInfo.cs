namespace Brimborium.CodeGeneration.SQLRelated {
    public sealed record DatabaseInfo() {
        public List<TableInfo> Tables { get; init; } = new List<TableInfo>();
        public List<ForeignKeyInfo> ForeignKey { get; init; } = new List<ForeignKeyInfo>();

        public DatabaseInfo Build() {
            this.ForeignKey.AddRange(this.Tables.SelectMany(t => t.ForeignKeys));
            return this;
        }
    };
}
