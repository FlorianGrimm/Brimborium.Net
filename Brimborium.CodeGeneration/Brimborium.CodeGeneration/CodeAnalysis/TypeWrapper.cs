// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class TypeWrapper : Type {
        private const string GlobalNamespaceValue = "<global namespace>";

        private readonly ITypeSymbol _TypeSymbol;

        private readonly MetadataLoadContext _MetadataLoadContext;

        private INamedTypeSymbol? _NamedTypeSymbol;

        private IArrayTypeSymbol? _ArrayTypeSymbol;

        private Type? _ElementType;

        public TypeWrapper(ITypeSymbol namedTypeSymbol, MetadataLoadContext metadataLoadContext) {
            this._TypeSymbol = namedTypeSymbol;
            this._MetadataLoadContext = metadataLoadContext;
            this._NamedTypeSymbol = this._TypeSymbol as INamedTypeSymbol;
            this._ArrayTypeSymbol = this._TypeSymbol as IArrayTypeSymbol;
        }

        public override Assembly Assembly => new AssemblyWrapper(this._TypeSymbol.ContainingAssembly, this._MetadataLoadContext);

        private string? _AssemblyQualifiedName;

        public override string AssemblyQualifiedName {
            get {
                if (this._AssemblyQualifiedName == null) {
                    StringBuilder sb = new();

                    AssemblyIdentity identity;

                    if (this._ArrayTypeSymbol == null) {
                        identity = this._TypeSymbol.ContainingAssembly.Identity;
                        sb.Append(this.FullName);
                    } else {
                        var currentType = this;
                        var nestCount = 1;

                        while (true) {
                            currentType = (TypeWrapper)currentType.GetElementType();

                            if (!currentType.IsArray) {
                                break;
                            }

                            nestCount++;
                        }

                        identity = currentType._TypeSymbol.ContainingAssembly.Identity;
                        sb.Append(currentType.FullName);

                        for (var i = 0; i < nestCount; i++) {
                            sb.Append("[]");
                        }
                    }

                    sb.Append(", ");
                    sb.Append(identity.Name);

                    sb.Append(", Version=");
                    sb.Append(identity.Version);

                    if (string.IsNullOrWhiteSpace(identity.CultureName)) {
                        sb.Append(", Culture=neutral");
                    }

                    sb.Append(", PublicKeyToken=");
                    var publicKeyToken = identity.PublicKeyToken;
                    if (publicKeyToken.Length > 0) {
                        foreach (var b in publicKeyToken) {
                            sb.Append(b.ToString("x2"));
                        }
                    } else {
                        sb.Append("null");
                    }

                    this._AssemblyQualifiedName = sb.ToString();
                }

                return this._AssemblyQualifiedName;
            }
        }

        public override Type BaseType => this._TypeSymbol.BaseType!.AsType(this._MetadataLoadContext)!;

        private string? _FullName;

        public override string FullName {
            get {
                if (this._FullName == null) {
                    StringBuilder sb = new();

                    if (this.IsNullableValueType(out var underlyingType)) {
                        sb.Append("System.Nullable`1[[");
                        sb.Append(underlyingType.AssemblyQualifiedName);
                        sb.Append("]]");
                    } else if (this.IsArray) {
                        sb.Append(this.GetElementType().FullName + "[]");
                    } else {
                        sb.Append(this.Name);

                        for (var currentSymbol = this._TypeSymbol.ContainingSymbol; currentSymbol != null && currentSymbol.Kind != SymbolKind.Namespace; currentSymbol = currentSymbol.ContainingSymbol) {
                            sb.Insert(0, $"{currentSymbol.Name}+");
                        }

                        if (!string.IsNullOrWhiteSpace(this.Namespace) && this.Namespace != GlobalNamespaceValue) {
                            sb.Insert(0, $"{this.Namespace}.");
                        }

                        if (this.IsGenericType && !this.ContainsGenericParameters) {
                            sb.Append('[');

                            foreach (var genericArg in this.GetGenericArguments()) {
                                sb.Append('[');
                                sb.Append(genericArg.AssemblyQualifiedName);
                                sb.Append(']');
                            }

                            sb.Append(']');
                        }
                    }

                    this._FullName = sb.ToString();
                }

                return this._FullName;
            }
        }

        public override Guid GUID => Guid.Empty;

        public override Module Module => throw new NotImplementedException();

        public override string Namespace =>
            this.IsArray ?
            this.GetElementType().Namespace ?? throw new TypeAccessException("Namespace is null") :
            this._TypeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining))!;

        public override Type UnderlyingSystemType => this;

        public override string Name {
            get {
                if (this._ArrayTypeSymbol == null) {
                    return this._TypeSymbol.MetadataName;
                }

                var elementType = this.GetElementType();
                return elementType.Name + "[]";
            }
        }

        public string SimpleName => this._TypeSymbol.Name;

        private Type? _EnumType;

        public override bool IsEnum {
            get {
                this._EnumType ??= this._MetadataLoadContext.Resolve(typeof(Enum));
                if (this._EnumType is Type enumType) {
                    return this.IsSubclassOf(enumType);
                }
                return false;
            }
        }

        public override bool IsGenericType => this._NamedTypeSymbol?.IsGenericType == true;

        public override bool ContainsGenericParameters => this._NamedTypeSymbol?.IsUnboundGenericType == true;

        public override bool IsGenericTypeDefinition => base.IsGenericTypeDefinition;

        public INamespaceSymbol GetNamespaceSymbol => this._TypeSymbol.ContainingNamespace;

        public override Type[] GetGenericArguments() {
            var args = new List<Type>();
            if (this._NamedTypeSymbol is INamedTypeSymbol namedTypeSymbol) {
                foreach (var item in namedTypeSymbol.TypeArguments) {
                    args.Add(item.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException());
                }
            }
            return args.ToArray();
        }

        public override Type GetGenericTypeDefinition() {
            if (this._NamedTypeSymbol is INamedTypeSymbol namedTypeSymbol) {
                return namedTypeSymbol.ConstructedFrom.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();
            }
            throw new InvalidOperationException("namedTypeSymbol is null");
        }

        public override IList<CustomAttributeData> GetCustomAttributesData() {
            var attributes = new List<CustomAttributeData>();
            foreach (var a in this._TypeSymbol.GetAttributes()) {
                attributes.Add(new CustomAttributeDataWrapper(a, this._MetadataLoadContext));
            }
            return attributes;
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) {
            if (this._NamedTypeSymbol == null) {
                return Array.Empty<ConstructorInfo>();
            }

            List<ConstructorInfo> ctors = new();

            foreach (var c in this._NamedTypeSymbol.Constructors) {
                if (c.IsImplicitlyDeclared && this.IsValueType) {
                    continue;
                }

                if ((BindingFlags.Public & bindingAttr) != 0 && c.DeclaredAccessibility == Accessibility.Public ||
                    (BindingFlags.NonPublic & bindingAttr) != 0 && c.DeclaredAccessibility != Accessibility.Public) {
                    ctors.Add(new ConstructorInfoWrapper(c, this._MetadataLoadContext));
                }
            }

            return ctors.ToArray();
        }

        public override object[] GetCustomAttributes(bool inherit) {
            throw new NotSupportedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            throw new NotSupportedException();
        }

        public override Type GetElementType() {
            this._ElementType ??= this._ArrayTypeSymbol?.ElementType.AsType(this._MetadataLoadContext)!;
            return this._ElementType;
        }

        public override Type MakeArrayType() {
            return this._MetadataLoadContext.Compilation.CreateArrayTypeSymbol(this._TypeSymbol).AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr) {
            List<FieldInfo> fields = new();

            foreach (var item in this._TypeSymbol.GetMembers()) {
                if (item is IFieldSymbol fieldSymbol) {
                    // Skip if:
                    if (
                        // this is a backing field
                        fieldSymbol.AssociatedSymbol != null ||
                        // we want a static field and this is not static
                        (BindingFlags.Static & bindingAttr) != 0 && !fieldSymbol.IsStatic ||
                        // we want an instance field and this is static or a constant
                        (BindingFlags.Instance & bindingAttr) != 0 && (fieldSymbol.IsStatic || fieldSymbol.IsConst) ||
                        // symbol represents an explicitly named tuple element
                        fieldSymbol.IsExplicitlyNamedTupleElement) {
                        continue;
                    }

                    if ((BindingFlags.Public & bindingAttr) != 0 && item.DeclaredAccessibility == Accessibility.Public ||
                        (BindingFlags.NonPublic & bindingAttr) != 0) {
                        fields.Add(new FieldInfoWrapper(fieldSymbol, this._MetadataLoadContext));
                    }
                }
            }

            return fields.ToArray();
        }

        public override Type GetInterface(string name, bool ignoreCase) {
            throw new NotImplementedException();
        }

        public override Type[] GetInterfaces() {
            var interfaces = new List<Type>();
            foreach (var i in this._TypeSymbol.AllInterfaces) {
                interfaces.Add(i.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException());
            }
            return interfaces.ToArray();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr) {
            var members = new List<MemberInfo>();
            foreach (var m in this._TypeSymbol.GetMembers()) {
                members.Add(new MemberInfoWrapper(m, this._MetadataLoadContext));
            }
            return members.ToArray();
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr) {
            var methods = new List<MethodInfo>();
            foreach (var m in this._TypeSymbol.GetMembers()) {
                // TODO: Efficiency
                if (m is IMethodSymbol method
                    //&& !this._namedTypeSymbol.Constructors.Contains(method)
                    && this._NamedTypeSymbol is INamedTypeSymbol namedTypeSymbol
                    && namedTypeSymbol.Constructors.Contains(method)
                    ) {
                    methods.Add(method.AsMethodInfo(this._MetadataLoadContext));
                }
            }
            return methods.ToArray();
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr) {
            var nestedTypes = new List<Type>();
            foreach (var type in this._TypeSymbol.GetTypeMembers()) {
                nestedTypes.Add(type.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException());
            }
            return nestedTypes.ToArray();
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) {
            List<PropertyInfo> properties = new();

            foreach (var item in this._TypeSymbol.GetMembers()) {
                if (item is IPropertySymbol propertySymbol) {
                    // Skip if:
                    if (
                        // we want a static property and this is not static
                        (BindingFlags.Static & bindingAttr) != 0 && !propertySymbol.IsStatic ||
                        // we want an instance property and this is static
                        (BindingFlags.Instance & bindingAttr) != 0 && propertySymbol.IsStatic) {
                        continue;
                    }

                    if ((BindingFlags.Public & bindingAttr) != 0 && item.DeclaredAccessibility == Accessibility.Public ||
                        (BindingFlags.NonPublic & bindingAttr) != 0) {
                        properties.Add(new PropertyInfoWrapper(propertySymbol, this._MetadataLoadContext));
                    }
                }
            }

            return properties.ToArray();
        }

        public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters) {
            throw new NotSupportedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        private TypeAttributes? _typeAttributes;

        protected override TypeAttributes GetAttributeFlagsImpl() {
            if (!this._typeAttributes.HasValue) {
                this._typeAttributes = default(TypeAttributes);

                if (this._TypeSymbol.IsAbstract) {
                    this._typeAttributes |= TypeAttributes.Abstract;
                }

                if (this._TypeSymbol.TypeKind == TypeKind.Interface) {
                    this._typeAttributes |= TypeAttributes.Interface;
                }

                if (this._TypeSymbol.ContainingType != null && this._TypeSymbol.DeclaredAccessibility == Accessibility.Private) {
                    this._typeAttributes |= TypeAttributes.NestedPrivate;
                }
            }

            return this._typeAttributes.Value;
        }

        protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers) {
            foreach (var constructor in this.GetConstructors(bindingAttr)) {
                var parameters = constructor.GetParameters();

                if (parameters.Length == types.Length) {
                    var mismatched = false;
                    for (var i = 0; i < parameters.Length; i++) {
                        if (parameters[i].ParameterType != types[i]) {
                            mismatched = true;
                            break;
                        }
                    }

                    if (!mismatched) {
                        return constructor;
                    }
                }
            }

            return null;
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers) {
            throw new NotImplementedException();
        }

        protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers) {
            // TODO: peformance; caching; honor bindingAttr
            foreach (var propertyInfo in this.GetProperties(bindingAttr)) {
                if (propertyInfo.Name == name) {
                    return propertyInfo;
                }
            }

            return null;
        }

        protected override bool HasElementTypeImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsArrayImpl() {
            return this._ArrayTypeSymbol != null;
        }

        private Type? _ValueType; // was Type!

        protected override bool IsValueTypeImpl() {
            this._ValueType ??= this._MetadataLoadContext.Resolve(typeof(ValueType));
            if (this._ValueType is Type valueType) {
                return this.IsSubclassOf(valueType);
            } else {
                return false;
            }
        }

        protected override bool IsByRefImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsCOMObjectImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsPointerImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsPrimitiveImpl() {
            throw new NotImplementedException();
        }

        public override bool IsAssignableFrom(Type? c) {
            if (c is null) { return false; }
            if (c is TypeWrapper tr) {
                return tr._TypeSymbol.AllInterfaces.Contains(this._TypeSymbol, SymbolEqualityComparer.Default) ||
                    tr._NamedTypeSymbol != null && tr._NamedTypeSymbol.BaseTypes().Contains(this._TypeSymbol, SymbolEqualityComparer.Default);
            } else if (this._MetadataLoadContext.Resolve(c) is TypeWrapper trr) {
                return trr._TypeSymbol.AllInterfaces.Contains(this._TypeSymbol, SymbolEqualityComparer.Default) ||
                    trr._NamedTypeSymbol != null && trr._NamedTypeSymbol.BaseTypes().Contains(this._TypeSymbol, SymbolEqualityComparer.Default);
            }
            return false;
        }

#pragma warning disable RS1024 // Compare symbols correctly
        public override int GetHashCode() => this._TypeSymbol.GetHashCode();
#pragma warning restore RS1024 // Compare symbols correctly

        public override int GetArrayRank() {
            if (this._ArrayTypeSymbol == null) {
                throw new ArgumentException("Must be an array type.");
            }

            return this._ArrayTypeSymbol.Rank;
        }

        public override bool Equals(object? o) {
            if (o is TypeWrapper tw) {
                return this._TypeSymbol.Equals(tw._TypeSymbol, SymbolEqualityComparer.Default);
            } else if (o is Type t && this._MetadataLoadContext.Resolve(t) is TypeWrapper tww) {
                return this._TypeSymbol.Equals(tww._TypeSymbol, SymbolEqualityComparer.Default);
            }

            return base.Equals(o);
        }

        public override bool Equals(Type? o) {
            if (o is null) { return false; }
            if (o is TypeWrapper tw) {
                return this._TypeSymbol.Equals(tw._TypeSymbol, SymbolEqualityComparer.Default);
            } else if (this._MetadataLoadContext.Resolve(o) is TypeWrapper tww) {
                return this._TypeSymbol.Equals(tww._TypeSymbol, SymbolEqualityComparer.Default);
            }
            return base.Equals(o);
        }

        public Location? Location => this._TypeSymbol.Locations.Length > 0 ? this._TypeSymbol.Locations[0] : null;
    }
}
