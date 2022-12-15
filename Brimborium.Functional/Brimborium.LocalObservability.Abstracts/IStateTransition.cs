namespace Brimborium.LocalObservability;

public interface IStateTransition {
    void Apply(
        ActualPolymorphCodePoint polymorphCodePoint,
        ICodePointState codePointState,
        IIncidentProcessingEngine1 processingEngine);
}

public interface ICodePointState {
    ICodePointState CreateChild(string childName, string childKey);
    ICodePointState? GetChild(string childName, string childKey);
    ICodePointState? RemoveChild(string childName, string childKey);
    object? GetValue(string name);
    void SetValue(string name, object? value);
    void Add(ActualPolymorphCodePoint polymorphCodePoint);
}

public interface IStatefullEngine {
    //ICodePointState CreateState();
    //void Match(IMatchingRule matchingRule, IMatchingEngine matchingEngine);
    //void Match(IMatchingRule matchingRule, IMatchingEngine matchingEngine);
}
