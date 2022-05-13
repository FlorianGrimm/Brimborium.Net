namespace Brimborium.Tracking;

public interface ITrackingSetEvent<TValue>
    where TValue : class {
    void OnAdding(
        TValue value,
        TrackingSet<TValue> trackingSet,
        TrackingContext trackingContext
        );
    void OnUpdating(
        TValue newValue,
        TValue oldValue,
        TrackingStatus oldTrackingStatus,
        TrackingSet<TValue> trackingSet,
        TrackingContext trackingContext
        );
    void OnDeleting(
        TValue newValue,
        TValue oldValue,
        TrackingStatus oldTrackingStatus,
        TrackingSet<TValue> trackingSet,
        TrackingContext trackingContext
        );
}

#warning WEICHEI
//public class TrackingSetEvent<
//    TValue,
//    TTrackingSet,
//    TTrackingContext>
//    : ITrackingSetEvent<TValue>
//    where TValue : class
//    where TTrackingSet : TrackingSet<TValue>
//    where TTrackingContext : TrackingContext {
//    public TrackingSetEvent() {
//    }

//    void ITrackingSetEvent<TValue>.Adding(TValue item, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
//        this.Adding(item, (TTrackingSet)trackingSet, (TTrackingContext) trackingContext);
//    }

//    public virtual void Adding(TValue item, TTrackingSet trackingSet, TTrackingContext trackingContext) {
//    }

//    void ITrackingSetEvent<TValue>.Updating(TValue item, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
//        this.Updating(item, (TTrackingSet)trackingSet, (TTrackingContext)trackingContext);
//    }

//    public virtual void Updating(TValue item, TTrackingSet trackingSet, TTrackingContext trackingContext) {
//    }

//    void ITrackingSetEvent<TValue>.Deleting(TValue item, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
//        this.Deleting(item, (TTrackingSet)trackingSet, (TTrackingContext)trackingContext);
//    }

//    public virtual void Deleting(TValue item, TTrackingSet trackingSet, TTrackingContext trackingContext) {
//    }

//}