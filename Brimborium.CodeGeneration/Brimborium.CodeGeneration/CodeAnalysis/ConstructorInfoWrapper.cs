// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class ConstructorInfoWrapper : ConstructorInfo {
        private readonly IMethodSymbol _ctor;
        private readonly MetadataLoadContext _metadataLoadContext;

        public ConstructorInfoWrapper(IMethodSymbol ctor, MetadataLoadContext metadataLoadContext) {
            Debug.Assert(ctor != null);
            this._ctor = ctor;
            this._metadataLoadContext = metadataLoadContext;
        }

        public override Type DeclaringType => this._ctor.ContainingType.AsType(this._metadataLoadContext) ?? throw new InvalidOperationException();

        private MethodAttributes? _Attributes;

        public override MethodAttributes Attributes => this._Attributes ??= this._ctor.GetMethodAttributes();

        public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

        public override string Name => this._ctor.Name;

        public override Type ReflectedType => throw new NotImplementedException();

        public override bool IsGenericMethod => this._ctor.IsGenericMethod;

        public override Type[] GetGenericArguments() {
            var typeArguments = new List<Type>();
            foreach (var t in this._ctor.TypeArguments) {
                typeArguments.Add(t.AsType(this._metadataLoadContext) ?? throw new InvalidOperationException());
            }
            return typeArguments.ToArray();
        }

        public override IList<CustomAttributeData> GetCustomAttributesData() {
            var attributes = new List<CustomAttributeData>();
            foreach (var a in this._ctor.GetAttributes()) {
                attributes.Add(new CustomAttributeDataWrapper(a, this._metadataLoadContext));
            }
            return attributes;
        }

        public override object[] GetCustomAttributes(bool inherit) {
            throw new NotSupportedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            throw new NotSupportedException();
        }

        public override MethodImplAttributes GetMethodImplementationFlags() {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetParameters() {
            var parameters = new List<ParameterInfo>();
            foreach (var p in this._ctor.Parameters) {
                parameters.Add(new ParameterInfoWrapper(p, this._metadataLoadContext));
            }
            return parameters.ToArray();
        }

        public override object Invoke(BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture) {
            throw new NotSupportedException();
        }

        public override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture) {
            throw new NotSupportedException();
        }

        //  public override object Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture) {
        //      throw new NotSupportedException();
        //  }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

    }
}
