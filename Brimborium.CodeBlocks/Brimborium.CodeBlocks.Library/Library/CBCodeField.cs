using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeField : ICBCodeTypeMember {
        public static CBCodeField FromClr(FieldInfo fieldInfo) {
            var result = new CBCodeField();
            result.Name = fieldInfo.Name;
            result.IsStatic = fieldInfo.IsStatic;
            if (fieldInfo.IsPrivate) {
                result.AccessibilityLevel = CBCodeAccessibilityLevel.Private;
            } else if (fieldInfo.IsPublic) {
                result.AccessibilityLevel = CBCodeAccessibilityLevel.Public;
            } else if (fieldInfo.IsFamily) {
                result.AccessibilityLevel = CBCodeAccessibilityLevel.Protected;
            }
            return result;
        }

        public CBCodeField() {
            this.Name = string.Empty;
            this.Type = CBCodeType.FromClr(type: typeof(object));
        }

        public FieldInfo? FieldInfo;

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; }

        public bool IsStatic { get; set; }

        public string Name { get; set; } = string.Empty;

        public CBCodeType Type { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() {
            if (this.Type is not null) {
                return new ICBCodeElement[] { this.Type };
            } else {
                return new ICBCodeElement[0];
            }
        }
    }

    public sealed class CBTemplateCSharpFieldDeclaration : CBNamedTemplate<CBCodeField> {
        public CBTemplateCSharpFieldDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeField value, CBRenderContext ctxt) {
            ctxt.CallTemplateDynamic(value.AccessibilityLevel).Write(" ")
                .CallTemplateDynamic(value.Type).Write(" ")
                .Write(value.Name).Write(";")
                .WriteLine();
        }
    }
}