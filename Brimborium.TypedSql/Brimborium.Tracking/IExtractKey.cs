namespace Brimborium.Tracking;

public interface IExtractKey<TValue, TKey> {
    TKey ExtractKey(TValue value);
}
