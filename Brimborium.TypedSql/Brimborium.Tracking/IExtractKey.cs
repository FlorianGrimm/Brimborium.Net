namespace Brimborium.Tracking;

public interface IExtractKey<TKey, TValue> {
    bool TryExtractKey(TValue value, [MaybeNullWhen(false)] out TKey key);
    //[System.Obsolete]
    //TKey ExtractKey(TValue value);
    //public bool TryExtractKey(TValue value, [MaybeNullWhen(false)] out TKey key) {
    //    key = this.ExtractKey(value);
    //    return true;
    //}
}
