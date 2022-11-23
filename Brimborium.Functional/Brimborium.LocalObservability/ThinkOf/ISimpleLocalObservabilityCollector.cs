//using System.Runtime.CompilerServices;
//[CallerMemberName]

namespace Brimborium.LocalObservability;

public interface ISimpleLocalObservabilityCollector {
    List<SimpleLocalObservabilityItem> Read();
}
