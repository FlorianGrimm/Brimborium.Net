using System.Threading;
using System.Threading.Channels;

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
    LogLevel GetLogLevel(object data);
    Exception? GetException (object data);
    DateTimeOffset GetTimeStamp (object data);
    string GetLine(object data);
    //List<object>? Scopes { get; }
    //object? GetState();
    IEnumerable<KeyValuePair<string, object>> GetScopeValues(object data);
    IEnumerable<KeyValuePair<string, object>> GetStateValues(object data);
    IEnumerable<KeyValuePair<string, object>> GetValues(object data);
}

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

    /// <summary>
    /// If this rule matches the polymorphCodePoint.EntryData 
    /// it's stores the CodePoint the given polymorphCodePoint.MatchedActualCodePoint
    /// </summary>
    /// <param name="polymorphCodePoint">the item to match</param>
    /// <returns>true to skip this and stop matching</returns>
    bool Match(LogEntryData entryData, ActualPolymorphCodePoint polymorphCodePoint);
}

/// <summary>
/// The matching engine is used to match a log entry to a <see cref="IActualCodePoint"/>  
/// and process the <see cref="IActualCodePoint"/> in a IStateTransition.
/// </summary>
public interface IMatchingEngine {
    void Match(LogEntryData entry);
}

public interface IIncidentProcessingEngine1 {
 
    // external
    void SetReport(IActualCodePoint acp, string message);
    void SetReport(ActualPolymorphCodePoint actualPolymorphCodePoint, string message);
    void SetTerminating(IActualCodePoint acp, ICodePointState codePointState);
}
public interface IIncidentProcessingEngine2: IIncidentProcessingEngine1 {
    // internal
    ChannelWriter<ActualPolymorphCodePoint> GetChannelWriter();
    Task Execute(CancellationToken stoppingToken);

}
//public interface IProxyStateLogEntry : IEnumerable<KeyValuePair<string, object>> {
//}
