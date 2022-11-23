namespace Brimborium.LocalObservability;

public class MatchingEngineOptions {
    public readonly List<IMatchingRule> LstRule;
    public MatchingEngineOptions() {
        this.LstRule = new List<IMatchingRule>();
    }
}

public class MatchingEngine 
    : IMatchingEngine
    , ICodePointState // ??
    {
    private readonly List<IMatchingRule> _LstRule;
    private readonly CodePointState _CodePointState;

    public MatchingEngine(
        MatchingEngineOptions options
        ) {
        this._CodePointState = new CodePointState();
        this._LstRule = new List<IMatchingRule>(options.LstRule);
    }


    public void Match(IMatchingEntry entry) {
        foreach (var rule in this._LstRule) {
            var actualCodePoint = rule.DoesConditionMatch(entry);
            if (actualCodePoint is not null) {

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