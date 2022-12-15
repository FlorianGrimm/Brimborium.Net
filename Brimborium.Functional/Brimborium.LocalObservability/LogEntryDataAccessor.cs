namespace Brimborium.LocalObservability;

public class LogEntryDataAccessor
     : ILogEntryDataAccessor {
    private static LogEntryDataAccessor? _Instance;
    public static LogEntryDataAccessor GetInstance() => _Instance ??= new LogEntryDataAccessor();

    public LogEntryDataAccessor() {
    }

    public string GetCategoryName(object data) {
        if (data is ILogEntry logEntry) {
            return logEntry.CategoryName;
        } else {
            throw new ArgumentException(nameof(data));
        }
    }

    public LogLevel GetLogLevel(object data) {
        if (data is ILogEntry logEntry) {
            return logEntry.LogLevel;
        } else {
            throw new ArgumentException(nameof(data));
        }
    }

    public EventId GetEventId(object data) {
        if (data is ILogEntry logEntry) {
            return logEntry.EventId;
        } else {
            throw new ArgumentException(nameof(data));
        }
    }

    public Exception? GetException(object data) {
        if (data is ILogEntry logEntry) {
            return logEntry.Exception;
        } else {
            throw new ArgumentException(nameof(data));
        }
    }

    public DateTimeOffset GetTimeStamp(object data) {
        if (data is ILogEntry logEntry) {
            return logEntry.TimeStamp;
        } else {
            throw new ArgumentException(nameof(data));
        }
    }

    public string GetLine(object data) {
        if (data is ILogEntry logEntry) {
            return logEntry.Line;
        } else {
            throw new ArgumentException(nameof(data));
        }
    }

    public IEnumerable<KeyValuePair<string, object>> GetValues(object data) {
        if (data is ILogEntry logEntry) {
            var state = logEntry.GetState();
            var scopes = logEntry.Scopes;
            if (state is not null) {
                if (state is IEnumerable<KeyValuePair<string, object>> ekv) {
                    foreach (var item in ekv) { yield return item; }
                }
            }

            if (scopes is not null) {
                foreach (var scope in scopes) {
                    if (scope is IEnumerable<KeyValuePair<string, object>> ekv) {
                        foreach (var item in ekv) {
                            yield return item;
                        }
                    } else if (scope is IEnumerable scopeE) {
                        foreach (var item in scopeE) {
                            if (item is null) {
                                continue;
                            }
                            if (item is KeyValuePair<string, object> kv) {
                                yield return kv;
                            } else {
                                System.Console.WriteLine(item.GetType().FullName);
                            }
                        }
                    } else {
                        System.Console.WriteLine(scope.GetType().FullName);
                    }
                }
            }
        } else {
            throw new ArgumentException(nameof(data));
        }
    }


    public IEnumerable<KeyValuePair<string, object>> GetScopeValues(object data) {
        if (data is ILogEntry logEntry) {
            var scopes = logEntry.Scopes;
            if (scopes is not null) {
                foreach (var scope in scopes) {
                    if (scope is IEnumerable<KeyValuePair<string, object>> ekv) {
                        foreach (var item in ekv) {
                            yield return item;
                        }
                    } else if (scope is IEnumerable scopeE) {
                        foreach (var item in scopeE) {
                            if (item is null) {
                                continue;
                            }
                            if (item is KeyValuePair<string, object> kv) {
                                yield return kv;
                            } else {
                                System.Console.WriteLine(item.GetType().FullName);
                            }
                        }
                    } else {
                        System.Console.WriteLine(scope.GetType().FullName);
                    }
                }
            }
        } else {
            throw new ArgumentException(nameof(data));
        }
    }

    public IEnumerable<KeyValuePair<string, object>> GetStateValues(object data) {
        if (data is ILogEntry logEntry) {
            var state = logEntry.GetState();
            if (state is not null) {
                if (state is IEnumerable<KeyValuePair<string, object>> ekv) {
                    foreach (var item in ekv) { yield return item; }
                }
            }
        } else {
            throw new ArgumentException(nameof(data));
        }
    }
}
