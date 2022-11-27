namespace Brimborium.Transformation.TextParser;

public class RuleAttribute : System.Attribute {
    public string Definition { get; set; }

    public RuleAttribute() {
        this.Definition = string.Empty;
    }

    public RuleAttribute(string definition) {
        this.Definition = definition;
    }
}