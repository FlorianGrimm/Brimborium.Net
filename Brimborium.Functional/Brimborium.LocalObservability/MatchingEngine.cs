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
    private readonly List<IMatchingRule> _ListMatchingRule;
    private readonly List<IStateTransition> _ListStateTransition;
    private readonly CodePointState _CodePointState;

    public MatchingEngine(
        MatchingEngineOptions options
        ) {
        this._CodePointState = new CodePointState();
        this._ListMatchingRule = new List<IMatchingRule>(options.ListMatchingRule);
        this._ListStateTransition = new List<IStateTransition>(options.ListStateTransition);
    }

    public void Match(IMatchingEntry entry) {
        // TODO Dictionary to speed up
        ICodePointState codePointState = this._CodePointState;
        bool done = false;
        foreach (var matchingRule in this._ListMatchingRule) {
            if (done) { break; }
            var actualCodePoint = matchingRule.DoesConditionMatch(entry);
            if (actualCodePoint is not null) {
                {
                    if (matchingRule is IStateTransition stateTransition) {
                        if (stateTransition.DoesActualCodePointMatch(actualCodePoint)) {
                            (codePointState, done) = stateTransition.Execute(actualCodePoint, codePointState);
                            if (done) {
                                continue;
                            }
                        }
                    }
                }
                // TODO Dictionary to speed up
                foreach (var stateTransition in this._ListStateTransition) {
                    if (stateTransition.DoesActualCodePointMatch(actualCodePoint)) {
                        (codePointState, done) = stateTransition.Execute(actualCodePoint, codePointState);
                        break;
                    }
                }
            }
        }
    }

    // ??
    public ICodePointState GetChild(string childName, string childKey)
        => this._CodePointState.GetChild(childName, childKey);
}


public class CodePointState : ICodePointState {
    public CodePointState() {
    }

    public ICodePointState GetChild(string childName, string childKey) {
        throw new NotImplementedException();
    }
}