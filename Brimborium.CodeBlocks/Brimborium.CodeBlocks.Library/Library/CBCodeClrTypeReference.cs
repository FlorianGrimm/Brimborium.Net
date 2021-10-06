using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Brimborium.CodeBlocks.Library {
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed class CBCodeClrTypeReference : ICBCodeTypeReference, ICBCodeTypeName {
        private static Dictionary<Type, CBCodeClrTypeReference> _Cache = new Dictionary<Type, CBCodeClrTypeReference>();
        public static CBCodeClrTypeReference Create<T>() => CreateFrom(typeof(T));
        public static CBCodeClrTypeReference CreateFrom(Type type) {
            if (_Cache.TryGetValue(type, out var result)) {
                return result;
            } else {
                result = new CBCodeClrTypeReference(type);
                _Cache.Add(type, result);
                return result;
            }
        }

        private CBCodeNamespace? _Namespace;

        public CBCodeClrTypeReference(System.Type type) {
            this.Type = type;
        }

        public Type Type { get; }

        public CBCodeNamespace Namespace { get => _Namespace ??= new CBCodeNamespace(this.Type.Namespace ?? string.Empty); set => throw new System.NotSupportedException(); }

        private string? _TypeName;
        public string TypeName {
            get => _TypeName ??= this.Type.Name.Contains('`') ? this.Type.Name.Split('`')[0] : this.Type.Name;
            set => throw new System.NotSupportedException();
        }

        public string FullName => this.Type.FullName ?? string.Empty;

        public ICBCodeTypeName CodeTypeName {
            get => this.GetCBCodeTypeNameReference().CodeTypeName;
            set => throw new NotSupportedException();
        }

        private CBCodeTypeNameReference? _CodeTypeNameReference;
        public CBCodeTypeNameReference GetCBCodeTypeNameReference() {
            if (_CodeTypeNameReference is null) {
                CBCodeTypeName codeTypeName = new CBCodeTypeName(this.Namespace, this.TypeName);
                if (this.GetGenericTypeDefinition() is CBCodeClrTypeReference genericTypeDefinition) {
                    codeTypeName.GenericTypeDefinition = genericTypeDefinition.GetCBCodeTypeNameReference();
                }
                codeTypeName.GenericTypeArguments = this.GetGenericTypeArguments().Select(t => t.GetCBCodeTypeNameReference()).ToArray();
                return _CodeTypeNameReference = new CBCodeTypeNameReference(codeTypeName);
            }
            return _CodeTypeNameReference;
        }

        public ICBCodeTypeReference? GenericTypeDefinition { get => this.GetGenericTypeDefinition(); set => throw new NotSupportedException(); }

        private ICBCodeTypeReference[]? _GenericTypeArguments;
        public ICBCodeTypeReference[] GenericTypeArguments { get => this._GenericTypeArguments ??= this.GetGenericTypeArguments().Cast<ICBCodeTypeReference>().ToArray(); set => throw new NotSupportedException(); }

        public ICBCodeElement? Parent { get; set; }
        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];

        public List<CBCodeClrMemberInfo> GetMembers(BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public) {
            var result = new List<CBCodeClrMemberInfo>();
            foreach (var typeMember in this.Type.GetMembers(bindingAttr)) {
                if (typeMember is MethodInfo methodInfo) {
                    result.Add(new CBCodeClrMethodInfo(methodInfo));
                } else if (typeMember is PropertyInfo propertyInfo) {
                    result.Add(new CBCodeClrPropertyInfo(propertyInfo));
                } else {
                    result.Add(new CBCodeClrMemberInfo(typeMember));
                }
            }
            return result;
        }

        private CBCodeClrTypeReference[]? _Interfaces;
        public CBCodeClrTypeReference[] GetInterfaces() {
            if (_Interfaces is null) {
                var result = new List<CBCodeClrTypeReference>();
                foreach (var i in this.Type.GetInterfaces()) {
                    result.Add(CBCodeClrTypeReference.CreateFrom(i));
                }
                return _Interfaces = result.ToArray();
            } else {
                return _Interfaces;
            }
        }

        public bool TryGetGenericTypeArguments(Type type, [MaybeNullWhen(false)] out CBCodeClrTypeReference[] parameters) {
            if (this.Type.IsConstructedGenericType) {
                if (this.Type.GetGenericTypeDefinition() == type) {
                    parameters = this.GetGenericTypeArguments();
                    return true;
                }
            }
            parameters = default;
            return false;
        }

        public CBCodeClrTypeReference? GetBaseType() {
            if (this.Type == typeof(object)) {
                return default;
            }
            if (this.Type.BaseType is not null) {
                return CBCodeClrTypeReference.CreateFrom(this.Type.BaseType);
            }
            if (this.Type.IsConstructedGenericType) {
                return CBCodeClrTypeReference.CreateFrom(this.Type.GetGenericTypeDefinition());
            }
            return default;
        }

        private CBCodeClrTypeReference[]? _GetGenericTypeArguments;

        public CBCodeClrTypeReference[] GetGenericTypeArguments() {
            if (this.Type.IsConstructedGenericType) {
                return this._GetGenericTypeArguments ??= this.Type.GenericTypeArguments.Select(t => new CBCodeClrTypeReference(t)).ToArray();
            } else {
                return this._GetGenericTypeArguments ??= new CBCodeClrTypeReference[0];
            }
        }
        public CBCodeClrTypeReference? GetGenericTypeDefinition() {
            if (this.Type.IsConstructedGenericType) {
                return CBCodeClrTypeReference.CreateFrom(this.Type.GetGenericTypeDefinition());
            } else {
                return default;
            }
        }

        internal string? CacheCSharp;

        public override string ToString() {
            return this.Type.FullName ?? string.Empty;
        }

        private string GetDebuggerDisplay() {
            return this.Type.FullName ?? string.Empty;
        }
    }

    public sealed class CBTemplateCSharpClrTypeReferenceTypeName : CBNamedTemplate<CBCodeClrTypeReference> {
        public CBTemplateCSharpClrTypeReferenceTypeName()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.TypeName) {
        }

        public override void RenderT(CBCodeClrTypeReference value, CBRenderContext ctxt) {
            if (value.CacheCSharp is null) {
                var sb = new System.Text.StringBuilder();
                Convert(value, sb);
                value.CacheCSharp = sb.ToString();
            }
            ctxt.Write(value.CacheCSharp);
        }

        private void Convert(CBCodeClrTypeReference value, System.Text.StringBuilder sb) {
            var genericTypeDefinition = value.GetGenericTypeDefinition();
            if (genericTypeDefinition is null) {
                sb.Append(value.Type.FullName);
            } else {
                var genericTypeArguments = value.GetGenericTypeArguments();
                sb.Append(value.Namespace.Namespace)
                    .Append(".")
                    .Append(value.TypeName.Split('`')[0])
                    .Append("<");
                for (int idx = 0; idx < genericTypeArguments.Length; idx++) {
                    if (idx > 0) { sb.Append(", "); }
                    Convert(genericTypeArguments[idx], sb);
                }
                sb.Append(">");
            }
        }
    }
}