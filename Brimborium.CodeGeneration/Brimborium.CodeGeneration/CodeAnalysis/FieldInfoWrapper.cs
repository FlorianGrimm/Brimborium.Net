// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class FieldInfoWrapper : FieldInfo {
        private readonly IFieldSymbol _Field;
        private readonly MetadataLoadContext _MetadataLoadContext;
        public FieldInfoWrapper(IFieldSymbol parameter, MetadataLoadContext metadataLoadContext) {
            this._Field = parameter;
            this._MetadataLoadContext = metadataLoadContext;
        }

        private FieldAttributes? _attributes;

        public override FieldAttributes Attributes {
            get {
                if (!this._attributes.HasValue) {
                    this._attributes = default(FieldAttributes);

                    if (this._Field.IsStatic) {
                        this._attributes |= FieldAttributes.Static;
                    }

                    if (this._Field.IsReadOnly) {
                        this._attributes |= FieldAttributes.InitOnly;
                    }

                    switch (this._Field.DeclaredAccessibility) {
                        case Accessibility.Public:
                            this._attributes |= FieldAttributes.Public;
                            break;
                        case Accessibility.Private:
                            this._attributes |= FieldAttributes.Private;
                            break;
                        case Accessibility.Protected:
                            this._attributes |= FieldAttributes.Family;
                            break;
                    }
                }

                return this._attributes.Value;
            }
        }

        public override RuntimeFieldHandle FieldHandle => throw new NotImplementedException();

        public override Type FieldType => this._Field.Type.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();

        public override Type DeclaringType => this._Field.ContainingType.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();

        public override string Name => this._Field.Name;

        public override Type ReflectedType => throw new NotImplementedException();

        public override object[] GetCustomAttributes(bool inherit) {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        public override object GetValue(object? obj) {
            throw new NotImplementedException();
        }

        public override IList<CustomAttributeData> GetCustomAttributesData() {
            var attributes = new List<CustomAttributeData>();
            foreach (var a in this._Field.GetAttributes()) {
                attributes.Add(new CustomAttributeDataWrapper(a, this._MetadataLoadContext));
            }
            return attributes;
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture) {
            throw new NotImplementedException();
        }

        public Location? Location => this._Field.Locations.Length > 0 ? this._Field.Locations[0] : null;
    }
}
