namespace Brimborium.Tracking.Extensions;

public sealed class ValidateEntityVersion<TKey, TValue> : ITrackingSetEvent<TKey, TValue>
    where TKey:notnull
    where TValue : class, IEntityWithVersion {
    private static ValidateEntityVersion<TKey, TValue>? _Instance;
    public static ValidateEntityVersion<TKey, TValue> Instance => _Instance ??= new ValidateEntityVersion<TKey, TValue>();

    private ValidateEntityVersion() { }

    public AddingArgument<TKey, TValue> OnAdding(AddingArgument<TKey, TValue> argument) {
        var (value, _, _) = argument;
        if (value.EntityVersion != 0) {
            throw new InvalidModificationException("EntityVersion!=0");
        }
        return argument;
    }

    public UpdatingArgument<TKey, TValue> OnUpdating(UpdatingArgument<TKey, TValue> argument) {
        var (newValue, _, oldValue, _, _, _) = argument;
        if (!oldValue.EntityVersion.EntityVersionDoesMatch(newValue.EntityVersion)) {
            throw new InvalidModificationException("EntityVersion does not match");
        }
        return argument;
    }

    public DeletingArgument<TKey, TValue> OnDeleting(DeletingArgument<TKey, TValue> argument) {
        var (newValue, oldValue, _, _, _) = argument;
        if (!oldValue.EntityVersion.EntityVersionDoesMatch(newValue.EntityVersion)) {
            throw new InvalidModificationException("EntityVersion does not match");
        }
        return argument;
    }
}
