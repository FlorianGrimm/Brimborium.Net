namespace Brimborium.LocalObservability;

/// <summary>
/// The CodePath
/// </summary>
public class CodePath {
    public CodePath() {
    }

    public CodePath(
        string? name
    ) {
        this.Name = name;
    }

    public string? Name { get; set;  }
}

public class CodePathSequence: CodePath {
    public CodePathSequence(
        string? name,
        List<CodePath>? steps
    ):base(name) {
        this.Steps = steps ?? new List<CodePath>();
    }

    public List<CodePath> Steps { get; }
}

public class CodePathRepeat : CodePath {
    public CodePathRepeat(
        string? name,
        CodePath step
    ) : base(name) {
        this.Step = step;
    }

    public CodePath Step { get; set; }
}

public class CodePathStep:CodePath {
    public CodePathStep(
        string? name,
        CodePoint? codePoint
        ) :base(name) {
        this.CodePoint = codePoint;
    }

    public CodePoint? CodePoint { get; set; }
}