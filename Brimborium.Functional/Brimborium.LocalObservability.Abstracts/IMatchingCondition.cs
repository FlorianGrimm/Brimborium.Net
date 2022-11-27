namespace Brimborium.LocalObservability;

public interface IMatchingRule {
    string Name { get; }
    IActualCodePoint? DoesConditionMatch(IMatchingEntry entry);
}


public interface IMatchingEngine {
    void Match(IMatchingEntry entry);
}

public interface IMatchingEntry {
}
public interface IMatchingEntry<T> {
    T? GetLogEntry();
    /*
    string? GetCategoryName();
    LogLevel? GetLogLevel()?
    EventId EventId { get; }
    //TState state { get; }
    Exception? Exception { get; }
    DateTimeOffset TimeStamp { get; }
    string Line { get; }
    List<object>? Scopes { get; }
    object? GetState();
     */
}

public interface IStateTransition {
    bool DoesActualCodePointMatch(IActualCodePoint actualCodePoint);
    (ICodePointState CodePointState, bool Done) Execute(IActualCodePoint actualCodePoint, ICodePointState codePointState);
}

public interface ICodePointState {
    ICodePointState GetChild(string childName, string childKey);
}

public interface IStatefullEngine {
    //ICodePointState CreateState();
    //void Match(IMatchingRule matchingRule, IMatchingEngine matchingEngine);
    //void Match(IMatchingRule matchingRule, IMatchingEngine matchingEngine);
}
