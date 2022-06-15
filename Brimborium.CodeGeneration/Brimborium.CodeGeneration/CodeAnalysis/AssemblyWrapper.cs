// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    public sealed class AssemblyWrapper : Assembly {
        private readonly MetadataLoadContext _MetadataLoadContext;

        public AssemblyWrapper(IAssemblySymbol assembly, MetadataLoadContext metadataLoadContext) {
            this.Symbol = assembly;
            this._MetadataLoadContext = metadataLoadContext;
        }

        public IAssemblySymbol Symbol { get; }

        public override string FullName => this.Symbol.Identity.Name;

        public override Type[] GetExportedTypes() {
            return this.GetTypes();
        }

        public override Type[] GetTypes() {
            var types = new List<Type>();
            var stack = new Stack<INamespaceSymbol>();
            stack.Push(this.Symbol.GlobalNamespace);
            while (stack.Count > 0) {
                var current = stack.Pop();

                foreach (var type in current.GetTypeMembers()) {
                    types.Add(type.AsType(this._MetadataLoadContext) ?? throw new InvalidOperationException());
                }

                foreach (var ns in current.GetNamespaceMembers()) {
                    stack.Push(ns);
                }
            }
            return types.ToArray();
        }

        public override Type GetType(string name) {
            return (
                    this.Symbol.GetTypeByMetadataName(name)
                    ?? throw new InvalidOperationException()
                ).AsType(this._MetadataLoadContext)
                ?? throw new InvalidOperationException()
                ;
        }
    }
}
