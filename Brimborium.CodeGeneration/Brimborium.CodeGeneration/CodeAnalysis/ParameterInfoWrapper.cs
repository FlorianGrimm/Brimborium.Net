// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class ParameterInfoWrapper : ParameterInfo {
        private readonly IParameterSymbol _Parameter;

        private readonly MetadataLoadContext _MetadataLoadContext;

        public ParameterInfoWrapper(IParameterSymbol parameter, MetadataLoadContext metadataLoadContext) {
            this._Parameter = parameter;
            this._MetadataLoadContext = metadataLoadContext;
        }

        public override Type ParameterType => this._Parameter.Type.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();

        public override string Name => this._Parameter.Name;

        public override bool HasDefaultValue => this._Parameter.HasExplicitDefaultValue;

        public override object? DefaultValue => this.HasDefaultValue ? this._Parameter.ExplicitDefaultValue : null;

        public override int Position => this._Parameter.Ordinal;

        public override IList<CustomAttributeData> GetCustomAttributesData() {
            var attributes = new List<CustomAttributeData>();
            foreach (var a in this._Parameter.GetAttributes()) {
                attributes.Add(new CustomAttributeDataWrapper(a, this._MetadataLoadContext));
            }
            return attributes;
        }
    }
}
