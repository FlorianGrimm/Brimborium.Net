// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class MethodInfoWrapper : MethodInfo {
        private readonly IMethodSymbol _Method;
        private readonly MetadataLoadContext _MetadataLoadContext;

        public MethodInfoWrapper(IMethodSymbol method, MetadataLoadContext metadataLoadContext) {
            this._Method = method;
            this._MetadataLoadContext = metadataLoadContext;
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();

        private MethodAttributes? _attributes;

        public override MethodAttributes Attributes => this._attributes ??= this._Method.GetMethodAttributes();

        public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

        public override Type DeclaringType => this._Method.ContainingType.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();

        public override Type ReturnType => this._Method.ReturnType.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();

        public override string Name => this._Method.Name;

        public override bool IsGenericMethod => this._Method.IsGenericMethod;

        public bool IsInitOnly => this._Method.IsInitOnly;

        public override Type ReflectedType => throw new NotImplementedException();

        public override IList<CustomAttributeData> GetCustomAttributesData() {
            var attributes = new List<CustomAttributeData>();
            foreach (var a in this._Method.GetAttributes()) {
                attributes.Add(new CustomAttributeDataWrapper(a, this._MetadataLoadContext));
            }
            return attributes;
        }

        public override MethodInfo GetBaseDefinition() {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit) {
            throw new NotSupportedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            throw new NotSupportedException();
        }

        public override Type[] GetGenericArguments() {
            var typeArguments = new List<Type>();
            foreach (var t in this._Method.TypeArguments) {
                typeArguments.Add(
                    t.AsType(this._MetadataLoadContext)
                        ?? throw new InvalidOperationException()
                    );
            }
            return typeArguments.ToArray();
        }

        public override MethodImplAttributes GetMethodImplementationFlags() {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetParameters() {
            var parameters = new List<ParameterInfo>();
            foreach (var p in this._Method.Parameters) {
                parameters.Add(new ParameterInfoWrapper(p, this._MetadataLoadContext));
            }
            return parameters.ToArray();
        }

        public override object Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture) {
            throw new NotSupportedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }
    }
}
