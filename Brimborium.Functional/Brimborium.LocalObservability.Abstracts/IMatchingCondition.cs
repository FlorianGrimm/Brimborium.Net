using Microsoft.Extensions.Logging;

namespace Brimborium.LocalObservability;

public record struct LogEntryData(
    object Data,
    ILogEntryDataAccessor DataAccessor
    ) {
    public string CategoryName => this.DataAccessor.GetCategoryName(this.Data);
    public EventId EventId => this.DataAccessor.GetEventId(this.Data);
    public IEnumerable<KeyValuePair<string, object>> GetValues() => this.DataAccessor.GetValues(this.Data);
}

public interface ILogEntryDataAccessor {
    string GetCategoryName(object data);
    EventId GetEventId(object data);
    IEnumerable<KeyValuePair<string, object>> GetValues(object data);
}

public interface IEnumerableIEnumerable : IEnumerable<KeyValuePair<string, object>> {
}

public enum MatchingKind {
    Start,
    Intercept,
    Normal,
    Stop
}

public interface IMatchingRule {
    MatchingKind Kind { get; }
    int Priority { get; }
    string Name { get; }
    IActualCodePoint? DoesConditionMatch(LogEntryData entry);
}


public interface IMatchingEngine {
    void Match(LogEntryData entry);
}

public interface IProxyStateLogEntry : IEnumerable<KeyValuePair<string, object>> {
}

public interface IStateTransition {
    bool DoesActualCodePointMatch(IActualCodePoint actualCodePoint);
    (ICodePointState CodePointState, bool Done) Execute(IActualCodePoint actualCodePoint, ICodePointState codePointState);
}

public interface ICodePointState {
    ICodePointState CreateChild(string childName, string childKey);
    ICodePointState? GetChild(string childName, string childKey);
    ICodePointState? RemoveChild(string childName, string childKey);
}

public interface IStatefullEngine {
    //ICodePointState CreateState();
    //void Match(IMatchingRule matchingRule, IMatchingEngine matchingEngine);
    //void Match(IMatchingRule matchingRule, IMatchingEngine matchingEngine);
}
