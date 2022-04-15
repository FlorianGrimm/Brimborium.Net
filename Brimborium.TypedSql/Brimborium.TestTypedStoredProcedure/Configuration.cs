using Brimborium.TypedStoredProcedure;

namespace Brimborium.TestTypedStoredProcedure;

public static partial class Program {
    public static DatabaseDefintion GetDefintion() {
        return new DatabaseDefintion(
             StoredProcedures: new StoredProcedureDefintion[] {
             },
             IgnoreTypePropertyNames: new TypePropertyNames[] {
             }
        );
    }
}
