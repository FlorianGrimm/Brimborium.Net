// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class PropertyInfoWrapper : PropertyInfo {
        private readonly IPropertySymbol _Property;
        private readonly MetadataLoadContext _MetadataLoadContext;

        public PropertyInfoWrapper(IPropertySymbol property, MetadataLoadContext metadataLoadContext) {
            this._Property = property;
            this._MetadataLoadContext = metadataLoadContext;
        }

        public override PropertyAttributes Attributes => throw new NotImplementedException();

        public override bool CanRead => this._Property.GetMethod != null;

        public override bool CanWrite => this._Property.SetMethod != null;

        public override Type PropertyType => this._Property.Type.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();

        public override Type DeclaringType => this._Property.ContainingType.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();

        public override string Name => this._Property.Name;

        public override Type ReflectedType => throw new NotImplementedException();

        public override MethodInfo[] GetAccessors(bool nonPublic) {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit) {
            throw new NotSupportedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            throw new NotSupportedException();
        }

        public override IList<CustomAttributeData> GetCustomAttributesData() {
            var attributes = new List<CustomAttributeData>();
            foreach (var a in this._Property.GetAttributes()) {
                attributes.Add(new CustomAttributeDataWrapper(a, this._MetadataLoadContext));
            }
            return attributes;
        }

        public override MethodInfo GetGetMethod(bool nonPublic) {
            return this._Property.GetMethod!.AsMethodInfo(this._MetadataLoadContext);
        }

        public override ParameterInfo[] GetIndexParameters() {
            var parameters = new List<ParameterInfo>();
            foreach (var p in this._Property.Parameters) {
                parameters.Add(new ParameterInfoWrapper(p, this._MetadataLoadContext));
            }
            return parameters.ToArray();
        }

        public override MethodInfo GetSetMethod(bool nonPublic) {
            return this._Property.SetMethod!.AsMethodInfo(this._MetadataLoadContext);
        }

        public override object? GetValue(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture) {
            throw new NotSupportedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture) {
            throw new NotSupportedException();
        }

        public Location? Location => this._Property.Locations.Length > 0 ? this._Property.Locations[0] : null;
    }
}
