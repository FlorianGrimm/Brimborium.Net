using Microsoft.Extensions.Logging;

using static System.Formats.Asn1.AsnWriter;

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

    public EventId GetEventId(object data) {
        if (data is ILogEntry logEntry) {
            return logEntry.EventId;
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


    //private readonly ILogEntry _LogEntry;

    //public ProxyILogEntry2MatchingEntry(ILogEntry logEntry) {
    //    this._LogEntry = logEntry;
    //}

    //ILogEntry? IMatchingEntry<ILogEntry>.GetLogEntry() => this._LogEntry;

    //public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
    //    foreach (var item in this._State) { yield return item; }
    //    if (this._Scopes is not null) {
    //        foreach (var scope in this._Scopes) {
    //            if (scope is IEnumerable<KeyValuePair<string, object>> scopeEKV) {
    //                foreach (var item in scopeEKV) {
    //                    yield return item;
    //                }
    //            } else if (scope is IEnumerable scopeE) {
    //                foreach (var item in scopeE) {
    //                    if (item is null) {
    //                        continue;
    //                    }
    //                    if (item is KeyValuePair<string, object> kv) {
    //                        yield return kv;
    //                    } else {
    //                        System.Console.WriteLine(item.GetType().FullName);
    //                    }
    //                }
    //            } else {
    //                System.Console.WriteLine(scope.GetType().FullName);
    //            }
    //        }
    //    }
    //}
}

//public class ProxyStateLogEntry: IProxyStateLogEntry {
//    private readonly IEnumerable<KeyValuePair<string, object>> _State;
//    private readonly List<object>? _Scopes;

//    public static ProxyStateLogEntry? Create(
//        object? values,
//        List<object>? scopes
//        ) {
//        if (values is IEnumerable<KeyValuePair<string, object>> e) { 
//            return new ProxyStateLogEntry(e,scopes);
//        }
//        return null;
//    }

//    public ProxyStateLogEntry(IEnumerable<KeyValuePair<string, object>> state, List<object>? scopes) {
//        this._State = state;
//        this._Scopes = scopes;
//    }


//    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
//        foreach(var item in this._State) { yield return item; }
//        if (this._Scopes is not null){
//            foreach(var scope in this._Scopes){
//                if (scope is IEnumerable<KeyValuePair<string,object>> scopeEKV){
//                    foreach(var item in scopeEKV){
//                        yield return item;
//                    }
//                } else if (scope is IEnumerable scopeE){
//                    foreach(var item in scopeE){
//                        if (item is null){
//                            continue;
//                        }
//                        if (item is KeyValuePair<string,object> kv){
//                            yield return kv;
//                        } else {
//                            System.Console.WriteLine(item.GetType().FullName);
//                        }
//                    }
//                } else {
//                    System.Console.WriteLine(scope.GetType().FullName);
//                }
//            }
//        }
//    }

//    IEnumerator IEnumerable.GetEnumerator() {
//        throw new NotImplementedException();
//    }

   
//    /*
//        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
//            return this._State.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator() {
//            return ((IEnumerable)this._State).GetEnumerator();
//        }
//    */
//}