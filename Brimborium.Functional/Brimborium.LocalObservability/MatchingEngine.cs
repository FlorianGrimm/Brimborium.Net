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
    , ICodePointState
    {
    private readonly List<IMatchingRule> _ListMatchingRule;
    private readonly List<IStateTransition> _ListStateTransition;
    private readonly CodePointState _CodePointState;

    public MatchingEngine(
        MatchingEngineOptions options
        ) {
        this._CodePointState = new CodePointState();
        this._ListMatchingRule = new List<IStateTransition>(options.ListMatchingRule);
        this._ListStateTransition = new List<IStateTransition>(options.ListStateTransition);
    }

    public void Match(IMatchingEntry entry) {
        // TODO Dictionary to speed up
        foreach (var matchingRule in this._ListMatchingRule) {
            var actualCodePoint = matchingRule.DoesConditionMatch(entry);
            if (actualCodePoint is not null) {
                { 
                    if (matchingRule is IStateTransition stateTransition) {
                        if (stateTransition.DoesActualCodePointMatch(actualCodePoint)) {
                            stateTransition.Execute(actualCodePoint);
                            continue;
                        }
                    }
                }
                // TODO Dictionary to speed up
                foreach (var stateTransition in this._ListStateTransition) {
                    if (stateTransition.DoesActualCodePointMatch(actualCodePoint)) {
                        stateTransition.Execute(actualCodePoint);
                        break;
                    }
                }
                //rule.Execute(entry, sideeffects)
                //this.StepState(entry, rule);
                //rule.StateTransition.Execute(entry);
            }
        }
        //var matches = new List<IMatchingRule>();
        //foreach (var rule in this._LstRule) {
        //    var doesMatch = rule.MatchingCondition.DoesMatch(entry);
        //    if (doesMatch) {
        //        matches.Add(rule);
        //    }
        //}
        //var lstExecute = new List<Action<IMatchingEntry>>();
        //foreach (var rule in matches) {
        //    var action=rule.StateTransition.Execute(entry);
        //    if (action is not null) {
        //        lstExecute.Add(action);
        //    }
        //}
        //foreach (var action in lstExecute) {
        //    action(entry);
        //}
    }

    // ??
    public ICodePointState GetChild(string childName, string childKey)
        => this._CodePointState.GetChild(childName, childKey);

    //private void StepState(IMatchingEntry entry, IMatchingRule rule) {
    //}

    //public ICodePointState CreateState() {
    //    throw new NotImplementedException();
    //}

    //public void Match(IMatchingRule matchingRule, IMatchingEngine matchingEngine) {
    //    throw new NotImplementedException();
    //}
}


public class CodePointState : ICodePointState {
    public CodePointState() {
    }

    public ICodePointState GetChild(string childName, string childKey) {
        throw new NotImplementedException();
    }
}