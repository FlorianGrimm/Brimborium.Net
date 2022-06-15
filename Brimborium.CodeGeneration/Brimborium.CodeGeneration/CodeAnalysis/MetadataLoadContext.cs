// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class MetadataLoadContext {
        private readonly Compilation _Compilation;

        public MetadataLoadContext(Compilation compilation) {
            this._Compilation = compilation;
        }

        public Compilation Compilation => this._Compilation;

        public Type? Resolve(Type type) {
            Debug.Assert(!type.IsArray, "Resolution logic only capable of handling named types.");
            return this.Resolve(type.FullName!);
        }

        public Type? Resolve(string fullyQualifiedMetadataName) {
            var typeSymbol = this._Compilation.GetBestTypeByMetadataName(fullyQualifiedMetadataName);
            return typeSymbol.AsType(this);
        }

        public Type Resolve(SpecialType specialType) {
            var typeSymbol = this._Compilation.GetSpecialType(specialType);
            return typeSymbol.AsType(this) ?? throw new InvalidOperationException();
        }
    }
}
