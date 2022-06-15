// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class MemberInfoWrapper : MemberInfo {
        private readonly ISymbol _Member;
        private readonly MetadataLoadContext _MetadataLoadContext;

        public MemberInfoWrapper(ISymbol member, MetadataLoadContext metadataLoadContext) {
            this._Member = member;
            this._MetadataLoadContext = metadataLoadContext;
        }

        public override Type DeclaringType => this._Member.ContainingType.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException();

        public override MemberTypes MemberType => throw new NotImplementedException();

        public override string Name => this._Member.Name;

        public override Type ReflectedType => throw new NotImplementedException();

        public override IList<CustomAttributeData> GetCustomAttributesData() {
            var attributes = new List<CustomAttributeData>();
            foreach (var a in this._Member.GetAttributes()) {
                attributes.Add(new CustomAttributeDataWrapper(a, this._MetadataLoadContext));
            }
            return attributes;
        }

        public override object[] GetCustomAttributes(bool inherit) {
            throw new NotSupportedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            throw new NotSupportedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }
    }
}
