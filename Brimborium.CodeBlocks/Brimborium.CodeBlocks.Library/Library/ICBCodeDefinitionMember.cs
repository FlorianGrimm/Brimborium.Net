using System;
using System.Collections.Generic;
using System.Linq;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeDefinitionMember : ICBCodeElement {
        CBCodeAccessibilityLevel AccessibilityLevel { get; set; }
        
        bool IsStatic { get; set; }

        bool IsReadonly { get; set; }

        ICBCodeTypeReference? Type { get; set; }

        string Name { get; set; }
    }

    public class CBCodeDefinitionCustomMember : ICBCodeDefinitionMember {
        public CBCodeDefinitionCustomMember() {
            this.Name = string.Empty;
        }
        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; }
        
        public bool IsStatic { get; set; }

        public bool IsReadonly { get; set; }

        public virtual ICBCodeTypeReference? Type { get; set; }

        public string Name { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public virtual IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBCodeDefinitionField : ICBCodeDefinitionMember {
        public CBCodeDefinitionField() {
        }

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; } = CBCodeAccessibilityLevel.Public;

        public bool IsStatic { get; set; }

        public bool IsReadonly { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICBCodeTypeReference? Type { get; set; }
        
        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() {
            if (this.Type is not null) {
                return new ICBCodeElement[] { this.Type };
            } else {
                return new ICBCodeElement[0];
            }
        }

        public static implicit operator CBCodeFieldExpression(CBCodeDefinitionField that) {
            return new CBCodeFieldExpression(that);
        }
    }

    public sealed class CBTemplateCSharpDefinitionFieldDeclaration : CBNamedTemplate<CBCodeDefinitionField> {
        public CBTemplateCSharpDefinitionFieldDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeDefinitionField value, CBRenderContext ctxt) {
            ctxt.CallTemplateDynamic(value.AccessibilityLevel);
            if (value.IsStatic) {
                ctxt.Write("static ");
            }
            if (value.IsReadonly) {
                ctxt.Write("readonly ");
            }
            ctxt.CallTemplateDynamic(value.Type, CBTemplateProvider.TypeName).Write(" ")
                .Write(value.Name).Write(";")
                .WriteLine();
        }
    }

    public sealed class CBCodeDefinitionConstructor : ICBCodeDefinitionMember {
        public CBCodeDefinitionConstructor() {
            this.Parameters = new CBList<ICBCodeParameter>(this);
        }

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; } = CBCodeAccessibilityLevel.Public;

        public bool IsStatic { get; set; }

        public CBList<ICBCodeParameter> Parameters { get; }

        public string Name {
            get => this.Parent switch { CBCodeTypeDefinition typeDefinition => typeDefinition.TypeName, _ => string.Empty };
            set => throw new System.NotSupportedException();
        }

        public ICBCodeStatement? Code { get; set; }

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
        public static CBCodeDefinitionField AddParameterAssignToField(
            this CBCodeDefinitionConstructor that,
            string name,
            ICBCodeTypeReference type,
            Action<CBCodeDefinitionField>? configField =default
            ) {
            var parameter = new CBCodeParameter(name, type);
            that.Parameters.Add(parameter);
            var field = new CBCodeDefinitionField() { 
                Name = "_" + name.Substring(0, 1).ToUpperInvariant() + name.Substring(1), 
                Type = type,
                AccessibilityLevel = CBCodeAccessibilityLevel.Private };
            if (that.Parent is CBCodeTypeDefinition typeDefinition) {
                typeDefinition.Members.Add(field);
            }
            if (that.Code is null) { that.Code = new CBCodeBlock(); }
            if (that.Code is CBCodeBlock block) {
                block.Statements.Add(
                    new CBCodeAssignment(
                        new CBCodeAccessorExpression(new CBCodeCustomExpression("this"), new CBCodeFieldExpression(field)),
                        new CBCodeAccessorExpression(new CBCodeParameterExpression(parameter))
                        )
                    );


                    //new CBCodeSimpleExpression("this.", f), new CBCodeSimpleExpression("this.", f)); ;
            }
            if (configField is not null) {
                configField(field);
            }
            return field;
        }
    }

    public sealed class CBTemplateCSharpDefinitionConstructorDeclaration : CBNamedTemplate<CBCodeDefinitionConstructor> {
        public CBTemplateCSharpDefinitionConstructorDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeDefinitionConstructor value, CBRenderContext ctxt) {
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
                .IndentDecr().Write(") {").WriteLine().IndentIncr()
                    .CallTemplateDynamic(value.Code)
                .IndentDecr().Write("}").WriteLine()
                .WriteLine()
                ;
        }
    }

    public class CBCodeDefinitionMethod : ICBCodeDefinitionMember {
        public CBCodeDefinitionMethod() {
            this.Parameters = new CBList<ICBCodeParameter>(this);
        }

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; } = CBCodeAccessibilityLevel.Public;

        public bool IsStatic { get; set; }

        public bool IsAsync { get; set; }

        public bool IsNew { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsOverride { get; set; }

        public ICBCodeTypeReference? ReturnType { get; set; }

        public string Name { get; set; } = string.Empty;

        public CBList<ICBCodeParameter> Parameters { get; }

        public ICBCodeStatement? Code { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() {
            var result = new List<ICBCodeElement>();
            if (this.ReturnType is not null) {
                result.Add(this.ReturnType);
            }
            result.AddRange(this.Parameters);
            if (this.Code is not null) {
                result.Add(this.Code);
            }
            return result;
        }
    }

    public sealed class CBTemplateCSharpDefinitionMethodDeclaration : CBNamedTemplate<CBCodeDefinitionMethod> {
        public CBTemplateCSharpDefinitionMethodDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeDefinitionMethod value, CBRenderContext ctxt) {
            ctxt.CallTemplateDynamic(value.AccessibilityLevel).Write(" ")
                .CallTemplateDynamic(value.ReturnType).Write(" ")
                .Write(value.Name).Write("(").IndentIncr()
                .Foreach(
                    value.Parameters,
                    (i, ctxt) => {
                        ctxt.CallTemplateDynamic(i.Value)
                        .If(
                            i.IsLast,
                            Then: ctxt => {
                                ctxt.WriteLine();
                            },
                            Else: ctxt => {
                                ctxt.WriteLine(",");
                            });

                    })
                .IndentDecr().Write(")")
                .If(value.Code is null,
                    Then: ctxt => ctxt.Write(";").WriteLine(),
                    Else: ctxt => ctxt
                        .Write(" {").WriteLine().IndentIncr()
                        .CallTemplateDynamic(value.Code)
                        .IndentDecr().Write("}").WriteLine()
                        .WriteLine()
                    );
        }
    }

    public class CBCodeDefinitionProperty : ICBCodeDefinitionMember {
        public bool IsStatic { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public CBCodeAccessibilityLevel AccessibilityLevel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Name { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public ICBCodeElement? Parent { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public IEnumerable<ICBCodeElement> GetChildren() {
            throw new System.NotImplementedException();
        }
    }
}