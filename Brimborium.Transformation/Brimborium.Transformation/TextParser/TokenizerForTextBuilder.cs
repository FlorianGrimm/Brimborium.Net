namespace Brimborium.Transformation.TextParser;

public class TokenizerForTextBuilder {
    public TokenizerForTextBuilder() {
    }

    public TokenizerForTextBuilder ScanTypes(Assembly assembly, string? @namespace = default) {
        var types = assembly.GetTypes();
        if (@namespace is not null) {
            types = types.Where(t =>
                    (t.Namespace is not null)
                    && (t.Namespace.StartsWith(@namespace, StringComparison.Ordinal))
                    && (t.GetCustomAttribute<TokenAttribute>() is not null)
                ).ToArray();
        } else {
            types = types.Where(t =>
                    (t.GetCustomAttribute<TokenAttribute>() is not null)
                ).ToArray();
        }
        return this.ScanTypes(types);
    }

    public TokenizerForTextBuilder ScanTypes(IEnumerable<Type> types) {
        foreach (var type in types) {
            var tokenAttribute = type.GetCustomAttribute<TokenAttribute>();
            if (tokenAttribute is null) { continue; }
            //tokenAttribute.Name
            //tokenAttribute.TokenId
            //tokenAttribute.Text
            //tokenAttribute.RegExpression
        }
        return this;
    }

    public List<TokenForTextBuilder> ScanType(Type type) {
        var result = new List<TokenForTextBuilder>();
        {
            var tokenAttribute = type.GetCustomAttribute<TokenAttribute>();
            if (tokenAttribute is not null) {
                var tokenForTextBuilder = new TokenForTextBuilder(tokenAttribute);
                tokenForTextBuilder.UseClass(type);
                result.Add(tokenForTextBuilder);
            }
        }
        foreach (var ctor in type.GetConstructors()) {
            var tokenAttributes = ctor.GetCustomAttributes<TokenAttribute>();
            if (tokenAttributes.Any()) {
                foreach (var tokenAttribute in tokenAttributes) {
                    var tokenForTextBuilder = new TokenForTextBuilder(tokenAttribute);
                    tokenForTextBuilder.UseConstructor(type, ctor);
                    result.Add(tokenForTextBuilder);
                }
            }
        }
        foreach (var method in type.GetMethods()) {
            var tokenAttributes = method.GetCustomAttributes<TokenAttribute>();
            if (tokenAttributes.Any()) {
                foreach (var tokenAttribute in tokenAttributes) {
                    var tokenForTextBuilder = new TokenForTextBuilder(tokenAttribute);
                    tokenForTextBuilder.UseMethod(method);
                    result.Add(tokenForTextBuilder);
                }
            }
        }

        //tokenAttribute.Name
        //tokenAttribute.TokenId
        //tokenAttribute.Text
        //tokenAttribute.RegExpression
        return result;
    }

    public TokenizerForTextBuilder AddToken(TokenDefinition tokenDefinition) {
        return this;
    }

    public TokenizerForText Build() {
        var result = new TokenizerForText();
        return result;
    }
}

public class TokenForTextBuilder {
    public TokenAttribute TokenAttribute { get; }

    public TokenForTextBuilder(TokenAttribute tokenAttribute) {
        this.TokenAttribute = tokenAttribute;
    }

    public void UseClass(Type type) {
        throw new NotImplementedException();
    }

    public void UseConstructor(Type type, ConstructorInfo ctor) {
        throw new NotImplementedException();
    }

    public void UseMethod(MethodInfo method) {
        throw new NotImplementedException();
    }
    public TokenDefinition Build(){
        var result = new TokenDefinition();
        return result;
    }
}

public class TokenDefinition {

}
