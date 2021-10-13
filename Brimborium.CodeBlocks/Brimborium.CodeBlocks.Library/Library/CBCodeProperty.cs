using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeProperty : ICBCodeTypeMember {
        public static CBCodeProperty FromClr(PropertyInfo propertyInfo) {
            var result = new CBCodeProperty();
            result.Name = propertyInfo.Name;

            //propertyInfo.CustomAttributes
            result.PropertyType = CBCodeType.FromClr(propertyInfo.PropertyType);
            result.CanRead = propertyInfo.CanRead;
            result.CanWrite = propertyInfo.CanWrite;
            // TODO
            // propertyInfo.GetGetMethod()
            // propertyInfo.GetSetMethod()
            return result;
        }

        public CBCodeProperty() {
            this.Name = string.Empty;
            this.PropertyType = CBCodeType.FromClr(typeof(object));
        }

        public PropertyInfo? PropertyInfo;

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; }
        
        public string Name { get; set; }

        public CBCodeType PropertyType { get; set; }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() {
            throw new System.NotImplementedException();
        }

    }
    public class CBTemplateCSharpPropertyDeclaration : CBNamedTemplate<CBCodeProperty> {
        public CBTemplateCSharpPropertyDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeProperty value, CBRenderContext ctxt) {
            //ctxt.CallTemplateDynamic(value.AccessibilityLevel).Write(" ")
            //    .Write(value.Name).Write("(").IndentIncr()
            //    .Foreach(
            //        value.Parameters,
            //        (i, ctxt) => {
            //            ctxt.WriteLine()
            //                .CallTemplateDynamic(i.Value)
            //                .If(
            //                    i.IsLast,
            //                    Then: ctxt => {
            //                        //ctxt.WriteLine();
            //                    },
            //                    Else: ctxt => {
            //                        ctxt.Write(",");
            //                    });
            //        })
            //    .IndentDecr().Write(") {").WriteLine().IndentIncr()
            //        .CallTemplateDynamic(value.Code)
            //    .IndentDecr().Write("}").WriteLine()
            //    .WriteLine()
            //    ;
        }
    }

}