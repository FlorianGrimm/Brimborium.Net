using Brimborium.CodeBlocks.Library;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Demo.CodeGen {
    public sealed class SrcIControllerInfo {
        public static SrcIControllerInfo Create(CBCodeType typeIController) {
            var r = new Regex("^I(.*)Controller$", RegexOptions.Singleline);
            var m = r.Match(typeIController.Name);
            if (!m.Success) {
                throw new System.ArgumentException($"{typeIController.Name} must match the pattern I...Controller.");
            }
            var result = new SrcIControllerInfo(typeIController, m.Groups[1].Value);
            result.Methods.AddRange(
                    typeIController.Methods.Select(method => new SrcIControllerMethodInfo(method))
                );
            return result;
        }

        public SrcIControllerInfo(CBCodeType typeIController, string shortName) {
            this.TypeIController = typeIController;
            this.ShortName = shortName;
            this.Methods = new List<SrcIControllerMethodInfo>();
        }

        public CBCodeType TypeIController { get; }

        public string ShortName { get; }

        public List<SrcIControllerMethodInfo> Methods { get; }
    }

    public sealed class SrcIControllerMethodInfo : CBCodeCustomMember {
        public SrcIControllerMethodInfo(CBCodeMethod sourceMethod) {
            this.SourceMethod = sourceMethod;
            this.Name = sourceMethod.Name;
            this.Parameters = new CBList<CBCodeParameter>(this);
            this.ReturnType = sourceMethod.ReturnType;
            foreach (var parameter in sourceMethod.Parameters) {
                this.Parameters.Add(parameter);
            }
        }

        public CBCodeMethod SourceMethod { get; }

        public CBCodeType? ReturnType { get; set; }

        public CBList<CBCodeParameter> Parameters { get; }

    }
}
