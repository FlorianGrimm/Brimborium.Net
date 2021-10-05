using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeTypeReference : ICBCodeElement { }

    public sealed class CBCodeTypeNameReference : ICBCodeTypeReference {
        public CBCodeTypeNameReference(ICBCodeTypeName codeTypeName) {
            this.CodeTypeName = codeTypeName;
        }

        public ICBCodeTypeName CodeTypeName { get; set; }
        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[] { this.CodeTypeName };
    }

    public sealed class CBTemplateCSharpTypeNameReference : CBNamedTemplate<CBCodeTypeNameReference> {
        public CBTemplateCSharpTypeNameReference()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeTypeNameReference value, CBRenderContext ctxt) {
            ctxt.Write(value.CodeTypeName.FullName);
        }
    }

    public sealed class CBTemplateCSharpAttributeTypeNameReference : CBNamedTemplate<CBCodeTypeNameReference> {
        public CBTemplateCSharpAttributeTypeNameReference()
            : base(CBTemplateProvider.CSharp, "Attribute") {
        }

        public override void RenderT(CBCodeTypeNameReference value, CBRenderContext ctxt) {
            ctxt.Write(CSharpUtility.WihoutAttribute(value.CodeTypeName));
        }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed class CBCodeClrTypeReference : ICBCodeTypeReference, ICBCodeTypeName {
        private static Dictionary<Type, CBCodeClrTypeReference> _Cache = new Dictionary<Type, CBCodeClrTypeReference>();
        public static CBCodeClrTypeReference Create<T>() => Create(typeof(T));
        public static CBCodeClrTypeReference Create(Type type) {
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

        public string TypeName { get => this.Type.Name; set => throw new System.NotSupportedException(); }

        public string FullName => this.Type.FullName ?? string.Empty;

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
                    result.Add(CBCodeClrTypeReference.Create(i));
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
                return CBCodeClrTypeReference.Create(this.Type.BaseType);
            }
            if (this.Type.IsConstructedGenericType) {
                return CBCodeClrTypeReference.Create(this.Type.GetGenericTypeDefinition());
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
                return CBCodeClrTypeReference.Create(this.Type.GetGenericTypeDefinition());
            } else {
                return default;
            }
        }

        private string GetDebuggerDisplay() {
            return this.Type.FullName ?? string.Empty;
        }
    }

    public sealed class CBTemplateCSharpClrTypeReference : CBNamedTemplate<CBCodeClrTypeReference> {
        public CBTemplateCSharpClrTypeReference()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeClrTypeReference value, CBRenderContext ctxt) {
            ctxt.Write(value.FullName);
        }
    }

    public sealed class CBTemplateCSharpAttributeClrTypeReference : CBNamedTemplate<CBCodeClrTypeReference> {
        public CBTemplateCSharpAttributeClrTypeReference()
            : base(CBTemplateProvider.CSharp, "Attribute") {
        }

        public override void RenderT(CBCodeClrTypeReference value, CBRenderContext ctxt) {
            ctxt.Write(CSharpUtility.WihoutAttribute(value));
        }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CBCodeClrMemberInfo {
        public CBCodeClrMemberInfo(MemberInfo memberInfo) {
            this.MemberInfo = memberInfo;
        }

        public MemberInfo MemberInfo { get; }

        public string Name => this.MemberInfo.Name;

        private string GetDebuggerDisplay() {
            return this.MemberInfo.Name;
        }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CBCodeClrMethodInfo : CBCodeClrMemberInfo {
        public CBCodeClrMethodInfo(MethodInfo methodInfo) : base(methodInfo) {
            this.MethodInfo = methodInfo;
        }

        public MethodInfo MethodInfo { get; }

        private CBCodeClrTypeReference? _ReturnType;
        public CBCodeClrTypeReference ReturnType => this._ReturnType ??= CBCodeClrTypeReference.Create(this.MethodInfo.ReturnType);

        CBCodeClrParameterInfo[]? _Parameters;
        public CBCodeClrParameterInfo[] Parameters {
            get {
                if (_Parameters is null) {
                    var parameters = new List<CBCodeClrParameterInfo>();
                    foreach (var parameterInfo in this.MethodInfo.GetParameters()) {
                        parameters.Add(new CBCodeClrParameterInfo(parameterInfo));
                    }
                    return this._Parameters = parameters.ToArray();
                } else {
                    return this._Parameters;
                }
            }
        }

        public string Name => this.MethodInfo.Name;

        public bool IsAsync() {
            return (this.MethodInfo.GetCustomAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>() is not null);
        }

        private string GetDebuggerDisplay() {
            return this.MemberInfo.Name;
        }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CBCodeClrParameterInfo {
        public CBCodeClrParameterInfo(ParameterInfo parameterInfo) {
            this.ParameterInfo = parameterInfo;
        }
        public CBCodeClrTypeReference ParameterType => CBCodeClrTypeReference.Create(this.ParameterInfo.ParameterType);

        public string Name => this.ParameterInfo.Name ?? string.Empty;

        public ParameterInfo ParameterInfo { get; }

        private string GetDebuggerDisplay() {
            return this.ParameterInfo.Name ?? string.Empty;
        }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CBCodeClrPropertyInfo : CBCodeClrMemberInfo {
        public CBCodeClrPropertyInfo(PropertyInfo propertyInfo) : base(propertyInfo) {
            this.PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; }

        private string GetDebuggerDisplay() {
            return this.PropertyInfo.Name;
        }
    }
}