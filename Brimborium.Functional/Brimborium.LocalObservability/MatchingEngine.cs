namespace Brimborium.LocalObservability;

public class MatchingEngineOptions {
    public readonly List<IMatchingRule> ListMatchingRule;
    public readonly List<IStateTransition> ListStateTransition;

    public MatchingEngineOptions() {
        this.ListMatchingRule = new List<IMatchingRule>();
        this.ListStateTransition = new List<IStateTransition>();
    }
}

public class MatchingEngine
    : IMatchingEngine
    , ICodePointState {
    private readonly List<IMatchingRule> _ListMatchingRule1Start;
    private readonly List<IMatchingRule> _ListMatchingRule2Intercept;
    private readonly List<IMatchingRule> _ListMatchingRule3Normal;
    private readonly List<IMatchingRule> _ListMatchingRule4Stop;
    private readonly List<IStateTransition> _ListStateTransition;
    private readonly CodePointState _CodePointState;

    public MatchingEngine(
        MatchingEngineOptions options
        ) {
        this._CodePointState = new CodePointState();
        this._ListMatchingRule1Start = new List<IMatchingRule>();
        this._ListMatchingRule2Intercept = new List<IMatchingRule>();
        this._ListMatchingRule3Normal = new List<IMatchingRule>();
        this._ListMatchingRule4Stop = new List<IMatchingRule>();
        this._ListStateTransition = new List<IStateTransition>(options.ListStateTransition);

        foreach (var matchingRule in options.ListMatchingRule) {
            if (matchingRule.Kind == MatchingKind.Start) {
                this._ListMatchingRule1Start.Add(matchingRule);
            } else if (matchingRule.Kind == MatchingKind.Intercept) {
                this._ListMatchingRule2Intercept.Add(matchingRule);
            } else if (matchingRule.Kind == MatchingKind.Normal) {
                this._ListMatchingRule3Normal.Add(matchingRule);
            } else if (matchingRule.Kind == MatchingKind.Stop) {
                this._ListMatchingRule4Stop.Add(matchingRule);
            } else {
                this._ListMatchingRule3Normal.Add(matchingRule);
            }
        }

        // this._ListMatchingRule1Start.Sort()
    }

    public void Match(LogEntryData entry) {
        /*
         * 
 * , IProxyStateLogEntry proxyStateLogEntry
var proxyState = ProxyStateLogEntry.Create(logEntry.GetState(), logEntry.Scopes);
if (proxyState is null) { return null; }
*/
        //if (entry.GetLogEntry() is ILogEntry logEntry) {
        //    var proxyState = ProxyStateLogEntry.Create(logEntry.GetState(), logEntry.Scopes);
        //    if (proxyState is null) { return  }
        //}


        // TODO Dictionary to speed up
        ICodePointState codePointState = this._CodePointState;
        foreach(var listMatchingRule in new []{this._ListMatchingRule1Start, this._ListMatchingRule2Intercept,this._ListMatchingRule3Normal,this._ListMatchingRule4Stop}){
            foreach (var matchingRule in listMatchingRule) {
                var done = false;
                var actualCodePoint = matchingRule.DoesConditionMatch(entry);
                if (actualCodePoint is not null) {
                    {
                        if (matchingRule is IStateTransition stateTransition) {
                            if (stateTransition.DoesActualCodePointMatch(actualCodePoint)) {
                                (codePointState, done) = stateTransition.Execute(actualCodePoint, codePointState);
                                if (done) {
                                    break;
                                }
                                // continue;
                            }
                        }
                    }
                    // TODO Dictionary to speed up
                    foreach (var stateTransition in this._ListStateTransition) {
                        if (stateTransition.DoesActualCodePointMatch(actualCodePoint)) {
                            (codePointState, done) = stateTransition.Execute(actualCodePoint, codePointState);
                            if (done) {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public ICodePointState CreateChild(string childName, string childKey)
        => this._CodePointState.CreateChild(childName, childKey);

    public ICodePointState? GetChild(string childName, string childKey)
        => this._CodePointState.GetChild(childName, childKey);

    public ICodePointState? RemoveChild(string childName, string childKey)
        => this._CodePointState.RemoveChild(childName, childKey);
}
