using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Brimborium.CodeBlocks.Library {
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
    public sealed class CBCodeClrMethodInfo : CBCodeClrMemberInfo {
        public CBCodeClrMethodInfo(MethodInfo methodInfo) : base(methodInfo) {
            this.MethodInfo = methodInfo;
        }

        public MethodInfo MethodInfo { get; }

        private CBCodeClrTypeReference? _ReturnType;
        public CBCodeClrTypeReference ReturnType => this._ReturnType ??= CBCodeClrTypeReference.CreateFrom(this.MethodInfo.ReturnType);

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

        public bool IsAsync() {
            return (this.MethodInfo.GetCustomAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>() is not null);
        }

        private string GetDebuggerDisplay() {
            return this.MemberInfo.Name;
        }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed class CBCodeClrParameterInfo {
        public CBCodeClrParameterInfo(ParameterInfo parameterInfo) {
            this.ParameterInfo = parameterInfo;
        }
        public CBCodeClrTypeReference ParameterType => CBCodeClrTypeReference.CreateFrom(this.ParameterInfo.ParameterType);

        public string Name => this.ParameterInfo.Name ?? string.Empty;

        public ParameterInfo ParameterInfo { get; }

        public CBCodeParameter AsCBCodeParameter() {
            return new CBCodeParameter(this.Name, this.ParameterType.GetCBCodeTypeNameReference()) { 
                IsOut = this.ParameterInfo.IsOut
            };
        }

        private string GetDebuggerDisplay() {
            return this.ParameterInfo.Name ?? string.Empty;
        }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed class CBCodeClrPropertyInfo : CBCodeClrMemberInfo {
        public CBCodeClrPropertyInfo(PropertyInfo propertyInfo) : base(propertyInfo) {
            this.PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; }

        private string GetDebuggerDisplay() {
            return this.PropertyInfo.Name;
        }
    }
}