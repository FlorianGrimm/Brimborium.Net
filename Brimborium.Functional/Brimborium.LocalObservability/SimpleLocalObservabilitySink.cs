//using System.Runtime.CompilerServices;
//[CallerMemberName]

namespace Brimborium.LocalObservability;

public class SimpleLocalObservabilitySink
    : ILocalObservabilitySink
    , ISimpleLocalObservabilityCollector {
    private const int Size = 1024;

    private SimpleLocalObservabilityItem[] _Items;
    private int _Current;

    public SimpleLocalObservabilitySink() {
        this._Current = 0;
        this._Items = new SimpleLocalObservabilityItem[Size];
    }

    public void Visit(CodePoint location, CodePoint? direction = default, string? args = default) {
        this._Items[this._Current % Size] = new SimpleLocalObservabilityItem(
            location,
            direction,
            args);
        this._Current++;
    }

    public List<SimpleLocalObservabilityItem> Read() {
        var current = this._Current;
        if (current <= 1024) {
            var result = new List<SimpleLocalObservabilityItem>(current);
            result.AddRange(this._Items[0..current]);
            return result;
        } else {
            var result = new List<SimpleLocalObservabilityItem>(Size);
            var idx = current % Size; ;
            result.AddRange(_Items[idx..(Size)]);
            result.AddRange(_Items[0..idx]);
            return result;
        }
    }
}
