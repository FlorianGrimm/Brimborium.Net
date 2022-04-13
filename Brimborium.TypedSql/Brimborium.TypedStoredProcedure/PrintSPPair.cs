namespace Brimborium.TypedStoredProcedure {
    public sealed record PrintSPPair(
        DatabaseStoredProcedure dbSP,
        StoredProcedureDefintion spDef
        );
}
