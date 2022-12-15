namespace Brimborium.LocalObservability;

public class CodePointState : ICodePointState {
    private Dictionary<string, ConcurrentDictionary<string, ICodePointState>>? _Child;
    private Dictionary<string, object>? _Values;
    public readonly List<ActualPolymorphCodePoint> CodePoints;
    private readonly ICodePointState? _Parent;

    public CodePointState() {
        this.CodePoints= new List<ActualPolymorphCodePoint>();
        this._Parent= null;
    }
    public CodePointState(ICodePointState parent) {
        this.CodePoints = new List<ActualPolymorphCodePoint>();
        this._Parent = parent;
    }

    public ICodePointState CreateChild(string childName, string childKey) {
        ConcurrentDictionary<string, ICodePointState>? dictChildKey;
        var this_Child = this._Child;
        if (this_Child is null) {
            this_Child = new Dictionary<string, ConcurrentDictionary<string, ICodePointState>>();
            dictChildKey = new ConcurrentDictionary<string, ICodePointState>();
            dictChildKey.TryAdd(childKey, new CodePointState(this));
            this_Child.Add(childName, dictChildKey);
            this._Child = this_Child;
        }
        while (!this_Child.TryGetValue(childName, out dictChildKey)) {
            dictChildKey = new ConcurrentDictionary<string, ICodePointState>();
            var dictChildName = new Dictionary<string, ConcurrentDictionary<string, ICodePointState>>(this_Child);
            dictChildName.Add(childName, dictChildKey);
            if (ReferenceEquals(
                System.Threading.Interlocked.CompareExchange(ref this._Child, dictChildName, this_Child),
                this._Child)) {
                break;
            }
        }
        while (true) {
            ICodePointState? result;
            if (dictChildKey.TryGetValue(childKey, out result)) {
                return result;
            } else {
                result = new CodePointState(this);
                if (dictChildKey.TryAdd(childKey, result)) {
                    return result;
                }
            }
        }
    }

    public ICodePointState? GetChild(string childName, string childKey) {
        if (this._Child is null) {
            return default;
        }
        if (this._Child.TryGetValue(childName, out var dictChildKey)) {
            if (dictChildKey.TryGetValue(childKey, out var result)) {
                return result;
            }
        }
        return default;
    }

    public ICodePointState? RemoveChild(string childName, string childKey) {
        if (this._Child is null) {
            return default;
        }
        if (this._Child.TryGetValue(childName, out var dictChildKey)) {
            if (dictChildKey.TryRemove(childKey, out var result)) {
                return result;
            }
        }
        return default;
    }

    public object? GetValue(string name) {
        if (this._Values is not null) {
            if (this._Values.TryGetValue(name, out var result)) {
                return result;
            }
        }
        return default;
    }

    public void SetValue(string name, object? value) {
        var values = this._Values ??= new Dictionary<string, object>();
        if (value is null) {
            values.Remove(name);
        } else {
            values[name] = value;
        }
    }

    public void Add(ActualPolymorphCodePoint codePoint) { 
        this.CodePoints.Add(codePoint);
    }
}