using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeConstructor : ICBCodeTypeMember {
        public static CBCodeConstructor FromClr(ConstructorInfo constructorInfo) {
            var result = new CBCodeConstructor();
            result.ConstructorInfo = constructorInfo;
            // TODO Parameter
            return result;
        }

        public CBCodeConstructor() {
            this.Parameters = new CBList<CBCodeParameter>(this);
        }

        public ConstructorInfo? ConstructorInfo;

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; }

        public bool IsStatic { get; set; }

        public CBList<CBCodeParameter> Parameters { get; }

        public string Name {
            get => this.Parent switch { CBCodeType typeDefinition => typeDefinition.Name, _ => string.Empty };
            set => throw new System.NotSupportedException();
        }

        public ICBCodeElement? Code { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() {
            var result = new List<ICBCodeElement>();
            result.AddRange(this.Parameters);
            if (this.Code is not null) {
                result.Add(this.Code);
            }
            return result;
        }
    }


    public static class CBCodeDefinitionConstructorExtenstion {
        public static CBCodeField AddParameterAssignToField(
            this CBCodeConstructor that,
            string name,
            CBCodeType type
            ) {
            var parameter = new CBCodeParameter(name, type);
            that.Parameters.Add(parameter);
            var field = new CBCodeField() {
                Name = "_" + name.Substring(0, 1).ToUpperInvariant() + name.Substring(1),
                Type = type,
                AccessibilityLevel = CBCodeAccessibilityLevel.Private
            };
            if (that.Parent is CBCodeType typeDefinition) {
                typeDefinition.Members.Add(field);
            }
            if (that.Code is null) { that.Code = new CBCodeBlock(); }
            if (that.Code is CBCodeBlock block) {
                block.Statements.Add(
                    new CBCodeAssignment(
                        new CBCodeAccessor(
                            field
                            ) { This = true },
                        parameter
                        )
                    );
                //block.Statements.Add(new CBCodeAssignment("this.", ));
            }
            return field;
        }
    }

    public class CBTemplateCSharpConstructorDeclaration : CBNamedTemplate<CBCodeConstructor> {
        public CBTemplateCSharpConstructorDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeConstructor value, CBRenderContext ctxt) {
            ctxt.CallTemplateDynamic(value.AccessibilityLevel)
                .Write(value.Name).Write("(").IndentIncr()
                .Foreach(
                    value.Parameters,
                    (i, ctxt) => {
                        ctxt.WriteLine()
                            .CallTemplateDynamic(i.Value)
                            .If(
                                i.IsLast,
                                Then: ctxt => {
                                    //ctxt.WriteLine();
                                },
                                Else: ctxt => {
                                    ctxt.Write(",");
                                });
                    })
                .IndentDecr().Write(") {").WriteLine().IndentIncr();
            if (value.Code is not null) {
                ctxt.CallTemplateDynamic(value.Code, CBTemplateProvider.Expression);
            }
            ctxt.IndentDecr().Write("}").WriteLine()
            .WriteLine()
            ;
        }
    }

}