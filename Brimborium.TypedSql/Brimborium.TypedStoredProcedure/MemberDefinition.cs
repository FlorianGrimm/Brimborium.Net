namespace Brimborium.TypedStoredProcedure {
    public sealed record MemberDefinition(
            string Name,
            CSTypeDefinition Type
        );
}
