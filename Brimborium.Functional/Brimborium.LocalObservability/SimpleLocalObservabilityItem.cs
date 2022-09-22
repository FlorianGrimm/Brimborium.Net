//using System.Runtime.CompilerServices;
//[CallerMemberName]

namespace Brimborium.LocalObservability;

public readonly record struct SimpleLocalObservabilityItem(
    CodePoint Location,
    CodePoint? Direction,
    string? args
);