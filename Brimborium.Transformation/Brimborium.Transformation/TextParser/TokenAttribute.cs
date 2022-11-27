namespace Brimborium.Transformation.TextParser;

[System.AttributeUsage(AttributeTargets.Class)]
public class TokenAttribute : System.Attribute {
    public string? Name { get; set; }

    public int? TokenId { get; set; }

    public string? Text { get; set; }

    public string? RegExpression { get; set; }
    
    public TokenAttribute() { }

    public TokenAttribute(string? name, int? tokenId, string? text, string? regExpression) {
        this.Name = name;
        this.TokenId = tokenId;
        this.Text = text;
        this.RegExpression = regExpression;
    }
}
