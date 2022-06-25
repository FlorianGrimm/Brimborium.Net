namespace Brimborium.Tracking.Extensions;

public sealed class ValidateEntityVersion<TValue> : ITrackingSetEvent<TValue>
    where TValue : class, IEntityWithVersion {
    private static ValidateEntityVersion<TValue>? _Instance;
    public static ValidateEntityVersion<TValue> Instance => _Instance ??= new ValidateEntityVersion<TValue>();

    private ValidateEntityVersion() { }

    public AddingArgument<TValue> OnAdding(AddingArgument<TValue> argument) {
        var (value, _, _) = argument;
        if (value.EntityVersion != 0) {
            throw new InvalidModificationException("EntityVersion!=0");
        }
        return argument;
    }

    public UpdatingArgument<TValue> OnUpdating(UpdatingArgument<TValue> argument) {
        var (newValue, oldValue, _, _, _) = argument;
        if (!oldValue.EntityVersion.EntityVersionDoesMatch(newValue.EntityVersion)) {
            throw new InvalidModificationException("EntityVersion does not match");
        }
        return argument;
    }

    public DeletingArgument<TValue> OnDeleting(DeletingArgument<TValue> argument) {
        var (newValue, oldValue, _, _, _) = argument;
        if (!oldValue.EntityVersion.EntityVersionDoesMatch(newValue.EntityVersion)) {
            throw new InvalidModificationException("EntityVersion does not match");
        }
        return argument;
    }
}
