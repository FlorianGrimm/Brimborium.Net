// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class CustomAttributeDataWrapper : CustomAttributeData {
        public CustomAttributeDataWrapper(AttributeData a, MetadataLoadContext metadataLoadContext) {
            if (a.AttributeConstructor is null) {
                throw new InvalidOperationException();
            }

            var namedArguments = new List<CustomAttributeNamedArgument>();
            foreach (var na in a.NamedArguments) {
                var attributeClass = a.AttributeClass;
                if (attributeClass is null) {
                    throw new InvalidOperationException();
                }
                var member = attributeClass.BaseTypes().SelectMany(t => t.GetMembers(na.Key)).First();

                MemberInfo memberInfo = member is IPropertySymbol
                    ? new PropertyInfoWrapper((IPropertySymbol)member, metadataLoadContext)
                    : new FieldInfoWrapper((IFieldSymbol)member, metadataLoadContext);

                namedArguments.Add(new CustomAttributeNamedArgument(memberInfo, na.Value.Value));
            }

            var constructorArguments = new List<CustomAttributeTypedArgument>();

            foreach (var ca in a.ConstructorArguments) {
                if (ca.Kind == TypedConstantKind.Error) {
                    continue;
                }

                var value = ca.Kind == TypedConstantKind.Array ? ca.Values : ca.Value;
                var caType = ca.Type ?? throw new InvalidOperationException("ca.Type"); ;
                constructorArguments.Add(
                    new CustomAttributeTypedArgument(
                        caType.AsType(metadataLoadContext) ?? throw new InvalidOperationException(),
                        value));
            }

            this.Constructor = new ConstructorInfoWrapper(a.AttributeConstructor, metadataLoadContext);
            this.NamedArguments = namedArguments;
            this.ConstructorArguments = constructorArguments;
        }

        public override ConstructorInfo Constructor { get; }

        public override IList<CustomAttributeNamedArgument> NamedArguments { get; }

        public override IList<CustomAttributeTypedArgument> ConstructorArguments { get; }
    }
}
