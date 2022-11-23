//using System.Runtime.CompilerServices;
//[CallerMemberName]

namespace Brimborium.LocalObservability;


public interface ILocalObservabilitySink {
    void Visit(CodePoint location, CodePoint? direction=default, string? args=default);
}
