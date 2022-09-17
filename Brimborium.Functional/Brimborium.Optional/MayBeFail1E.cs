using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Optional;

public class MayBeFail<E> : MayBe {
    public override bool Success => false;
    public override bool Fail => true;

    public E Error { get; init; }

    public MayBeFail(E error) {
        this.Error = error;
    }

    public bool TryGetFailureValue([MaybeNullWhen(false)] out E error) {
        error = this.Error;
        return true;
    }
}
