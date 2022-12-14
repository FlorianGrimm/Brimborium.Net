namespace Brimborium.LocalObservability;

/// <summary>
/// Combines the (log entry) data with an uniformed access to the data.
/// </summary>
public record struct LogEntryData(
    object Data,
    ILogEntryDataAccessor DataAccessor
    ) {
    public string CategoryName => this.DataAccessor.GetCategoryName(this.Data);
    public EventId EventId => this.DataAccessor.GetEventId(this.Data);
    public IEnumerable<KeyValuePair<string, object>> GetValues() => this.DataAccessor.GetValues(this.Data);
}

/// <summary>
/// Access to the data of a log entry.
/// </summary>
public interface ILogEntryDataAccessor {
    string GetCategoryName(object data);
    EventId GetEventId(object data);
    IEnumerable<KeyValuePair<string, object>> GetValues(object data);
}

// public interface IEnumerableIEnumerable : IEnumerable<KeyValuePair<string, object>> {}

public enum MatchingKind {
    Start,
    Intercept,
    Normal,
    Stop
}

/// <summary>
/// A matching rule is used to match a log entry to a code point.
/// </summary>
public interface IMatchingRule {
    MatchingKind Kind { get; }
    int Priority { get; }
    string Name { get; }
    bool Match(ActualPolymorphCodePoint polymorphCodePoint);
}

/// <summary>
/// The matching engine is used to match a log entry to a <see cref="IActualCodePoint"/>  
/// and process the <see cref="IActualCodePoint"/> in a IStateTransition.
/// </summary>
public interface IMatchingEngine {
    void Match(LogEntryData entry);
}

public interface IProxyStateLogEntry : IEnumerable<KeyValuePair<string, object>> {
}

public interface IStateTransition {
    bool DoesActualCodePointMatch(IActualCodePoint actualCodePoint);
    ICodePointState Execute(IActualCodePoint actualCodePoint, ICodePointState codePointState);
}

public interface ICodePointState {
    ICodePointState CreateChild(string childName, string childKey);
    ICodePointState? GetChild(string childName, string childKey);
    ICodePointState? RemoveChild(string childName, string childKey);
    object? GetValue(string name);
    void SetValue(string name, object? value);
}

public interface IStatefullEngine {
    //ICodePointState CreateState();
    //void Match(IMatchingRule matchingRule, IMatchingEngine matchingEngine);
    //void Match(IMatchingRule matchingRule, IMatchingEngine matchingEngine);
}
