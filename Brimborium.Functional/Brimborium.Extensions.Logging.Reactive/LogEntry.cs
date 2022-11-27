namespace Brimborium.Extensions.Logging.Reactive;


public interface ILogEntry {
    string CategoryName { get; }
    LogLevel LogLevel { get; }
    EventId EventId { get; }
    //TState state { get; }
    Exception? Exception { get; }
    DateTimeOffset TimeStamp { get; }
    string Line { get; }
    List<object>? Scopes { get; }
    object? GetState();
}

public record LogEntry<TState>(
    string CategoryName,
    LogLevel LogLevel,
    EventId EventId,
    TState State,
    Exception? Exception,
    DateTimeOffset TimeStamp,
    List<object>? Scopes,
    string Line
    ) : ILogEntry {
    public object? GetState() => this.State;
}