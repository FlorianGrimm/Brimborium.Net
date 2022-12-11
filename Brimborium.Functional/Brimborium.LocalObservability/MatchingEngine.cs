﻿using System.ComponentModel.DataAnnotations;

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
        this._ListMatchingRule1Start = new List<IMatchingRule>(options.ListMatchingRule.Where(matchingRule => matchingRule.Kind == MatchingKind.Start).OrderBy(matchingRule => matchingRule.Priority).ThenBy(matchingRule => matchingRule.Name));
        this._ListMatchingRule2Intercept = new List<IMatchingRule>(options.ListMatchingRule.Where(matchingRule => matchingRule.Kind == MatchingKind.Intercept).OrderBy(matchingRule => matchingRule.Priority).ThenBy(matchingRule => matchingRule.Name));
        this._ListMatchingRule3Normal = new List<IMatchingRule>(options.ListMatchingRule.Where(matchingRule => matchingRule.Kind == MatchingKind.Normal).OrderBy(matchingRule => matchingRule.Priority).ThenBy(matchingRule => matchingRule.Name));
        this._ListMatchingRule4Stop = new List<IMatchingRule>(options.ListMatchingRule.Where(matchingRule => matchingRule.Kind == MatchingKind.Stop).OrderBy(matchingRule => matchingRule.Priority).ThenBy(matchingRule => matchingRule.Name));
        this._ListStateTransition = new List<IStateTransition>(options.ListStateTransition);
    }

    public void Match(LogEntryData entry) {
        // TODO Dictionary to speed up
        ICodePointState codePointState = this._CodePointState;
        codePointState = this.matchForList(entry, codePointState, this._ListMatchingRule1Start);
        codePointState = this.matchForList(entry, codePointState, this._ListMatchingRule2Intercept);
        codePointState = this.matchForList(entry, codePointState, this._ListMatchingRule3Normal);
        this.matchForList(entry, codePointState, this._ListMatchingRule4Stop);
    }

    private ICodePointState matchForList(LogEntryData entry, ICodePointState codePointState, List<IMatchingRule> listMatchingRule) {
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
                        }
                    }
                }
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
        //
        return codePointState;
    }

    public ICodePointState CreateChild(string childName, string childKey)
        => this._CodePointState.CreateChild(childName, childKey);

    public ICodePointState? GetChild(string childName, string childKey)
        => this._CodePointState.GetChild(childName, childKey);

    public ICodePointState? RemoveChild(string childName, string childKey)
        => this._CodePointState.RemoveChild(childName, childKey);

    public object? GetValue(string name) {
        return this._CodePointState.GetValue(name);
    }

    public void SetValue(string name, object? value) {
        this._CodePointState.SetValue(name, value);
    }
}