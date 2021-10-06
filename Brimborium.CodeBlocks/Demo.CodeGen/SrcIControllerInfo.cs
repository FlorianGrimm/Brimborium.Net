using Brimborium.CodeBlocks.Library;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Demo.CodeGen {
    public sealed class SrcIControllerInfo {
        public static SrcIControllerInfo Create(CBCodeClrTypeReference typeIController) {
            var r = new Regex("^I(.*)Controller$", RegexOptions.Singleline);
            var m = r.Match(typeIController.TypeName);
            if (!m.Success) {
                throw new System.ArgumentException($"{typeIController.TypeName} must match the pattern I...Controller.");
            }
            var result = new SrcIControllerInfo(typeIController, m.Groups[1].Value);
            result.Methods.AddRange(
                    typeIController.GetMembers().OfType<CBCodeClrMethodInfo>().Select(mi => new SrcIControllerMethodInfo(mi))
                );
            return result;
        }

        public SrcIControllerInfo(CBCodeClrTypeReference typeIController, string shortName) {
            this.TypeIController = typeIController;
            this.ShortName = shortName;
            this.Methods = new List<SrcIControllerMethodInfo>();
        }

        public CBCodeClrTypeReference TypeIController { get; }

        public string ShortName { get; }

        public List<SrcIControllerMethodInfo> Methods { get; }
    }

    public sealed class SrcIControllerMethodInfo : CBCodeDefinitionCustomMember {
        public SrcIControllerMethodInfo(CBCodeClrMethodInfo methodInfo) {
            this.MethodInfo = methodInfo;
            this.Name = methodInfo.Name;
            this.Parameters = new CBList<CBCodeParameter>(this);
            this.ReturnType = methodInfo.ReturnType.GetCBCodeTypeNameReference();
            foreach (var parameter in methodInfo.Parameters) {
                this.Parameters.Add(parameter.AsCBCodeParameter());
            }
        }

        public CBCodeClrMethodInfo MethodInfo { get; }

        public CBCodeTypeNameReference? ReturnType { get; set; }

        public CBList<CBCodeParameter> Parameters { get; }

    }
}
