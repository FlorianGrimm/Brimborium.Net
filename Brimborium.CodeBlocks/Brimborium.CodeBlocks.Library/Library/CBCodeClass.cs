using System.Collections.Generic;
using System.Linq;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeClass : CBTypeDefinition, ICBTypeDefinition {
        public CBCodeClass() : base() {
        }

        public CBCodeClass(CBCodeNamespace @namespace, string typeName) : base(@namespace, typeName) {
        }

        public bool IsSealed { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsPartial { get; set; }
    }


    public sealed class CBTemplateCSharpClass : CBNamedTemplate<CBCodeClass> {
        public CBTemplateCSharpClass()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeClass value, CBRenderContext ctxt) {
            ctxt.Foreach(
                items: value.Attributes,
                eachItem: (i, ctxt) => ctxt.CallTemplateDynamic(i.Value));
            ctxt.CallTemplate(value.AccessibilityLevel)
                .If(value.IsAbstract, (ctxt) => ctxt.Write("abstract "))
                .If(value.IsSealed, (ctxt) => ctxt.Write("sealed "))
                .If(value.IsPartial, (ctxt) => ctxt.Write("partial "))
                .Write("class ")
                .Write(value.TypeName)
                .CallTemplateDynamic(value, "BaseTypes")
                .Write(" {").WriteLine();
            foreach (var grp in value.Members
                .GroupBy(m => m switch { CBCodeDefinitionField => 0, CBCodeDefinitionConstructor => 1, CBCodeDefinitionProperty => 2, CBCodeDefinitionMethod => 3, _ => 4 })
                .OrderBy(kv => kv.Key)
                ) {
                foreach (var member in grp) {
                    ctxt.CallTemplateDynamic(member);
                }
                ctxt.WriteLine();
            }

            //foreach (var member in value.Members) {
            //    ctxt.CallTemplateDynamic(member);
            //}

            ctxt.IndentDecr().Write("}").WriteLine();
        }
    }

    public sealed class CBTemplateCSharpClassBaseTypes : CBNamedTemplate<CBCodeClass> {
        public CBTemplateCSharpClassBaseTypes()
            : base(CBTemplateProvider.CSharp, "BaseTypes") {
        }

        public override void RenderT(CBCodeClass value, CBRenderContext ctxt) {
            var lstBaseTypes = new List<ICBCodeTypeReference>();
            if (value.BaseType is not null) {
                lstBaseTypes.Add(value.BaseType);
            }
            lstBaseTypes.AddRange(value.Interfaces);

            ctxt.Foreach(
                items: lstBaseTypes,
                eachItem: (i, ctxt) => {
                    if (i.IsFirst) {
                        ctxt.WriteLine(indent: +1);
                        ctxt.Write(" : ");
                    } else {
                        ctxt.WriteLine();
                        ctxt.Write(", ");
                    }
                    ctxt.CallTemplateDynamic(i.Value);
                },
                isEmpty: (ctxt) => {
                    ctxt.IndentIncr();
                });
        }
    }
}