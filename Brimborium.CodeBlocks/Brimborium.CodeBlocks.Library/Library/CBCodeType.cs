using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Brimborium.CodeBlocks.Library {
    //public interface ICBCodeType : ICBCodeElement {
    //    CBCodeAccessibilityLevel AccessibilityLevel { get; }
    //    CBList<ICBCodeTypeDecoration> Attributes { get; }
    //    ICBCodeType? BaseType { get; set; }
    //    CBList<ICBCodeType> Interfaces { get; }
    //    CBList<ICBCodeDefinitionMember> Members { get; }
    //}


    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CBCodeType : ICBCodeElement {
        private static Dictionary<Type, CBCodeType> _Cache = new();

        public static IEnumerable<CBCodeType> FromClrList(IEnumerable<Type> types) {
            foreach (var type in types) {
                yield return FromClr(type);
            }
        }

        public static CBCodeType? FromCrlQ(Type? type) => (type is null) ? null : FromClr(type);

        public static CBCodeType FromClr(Type type) {
            if (_Cache.TryGetValue(type, out var result)) {
                return result;
            } else {
                _Cache.Add(type, result = new CBCodeType());
                var typeInfo = result.TypeInfo = type.GetTypeInfo();
                result.IsClass = type.IsClass;
                result.IsValueType = type.IsValueType;
                result.IsInterface = type.IsInterface;
                result.Namespace = new CBCodeNamespace(type.Namespace ?? string.Empty);
                if (type.IsGenericType) {
                    result.Name = type.Name.Split('`')[0];
                } else {
                    result.Name = type.Name;
                }

                if (type.IsPublic) {
                    result.AccessibilityLevel = CBCodeAccessibilityLevel.Public;
                }
                if (type.IsSealed) {
                    result.IsSealed = true;
                }

                result.BaseType = FromCrlQ(type.BaseType);
                //result.Interfaces.AddRange(FromTypes(type.GetInterfaces()));
                result.Interfaces.AddRange(FromClrList(typeInfo.ImplementedInterfaces));

                if (type.IsGenericType) {
                    if (ReferenceEquals(type, type.GetGenericTypeDefinition())) {
                    } else {
                        result.GenericTypeDefinition = FromClr(type.GetGenericTypeDefinition());
                    }
                    result.GenericTypeArguments.AddRange(FromClrList(type.GenericTypeArguments));
                } else {
                    System.Diagnostics.Debug.Assert(type.GenericTypeArguments.Length == 0);
                }
                //(type.GetTypeInfo().DeclaredConstructors())
                //result.Fields.AddRange()

                result.Fields.AddRange(typeInfo.DeclaredFields.Select(fieldInfo => CBCodeField.FromClr(fieldInfo)));
                result.Constructors.AddRange(typeInfo.DeclaredConstructors.Select(constructorInfo => CBCodeConstructor.FromClr(constructorInfo)));
                result.Properties.AddRange(typeInfo.DeclaredProperties.Select(propertyInfo => CBCodeProperty.FromClr(propertyInfo)));
                result.Methods.AddRange(typeInfo.DeclaredMethods.Select(methodInfo => CBCodeMethod.FromClr(methodInfo)));

                return result;
            }
        }

        public CBCodeType() {
            this.Namespace = new CBCodeNamespace();
            this.Name = string.Empty;
            this.Prefix = new CBList<ICBCodeElement>(this);
            this.Attributes = new CBList<ICBCodeTypeDecoration>(this);
            this.Interfaces = new CBList<CBCodeType>(this);
            this.GenericTypeArguments = new CBList<CBCodeType>(this);
            this.Fields = new CBList<CBCodeField>(this);
            this.Constructors = new CBList<CBCodeConstructor>(this);
            this.Properties = new CBList<CBCodeProperty>(this);
            this.Methods = new CBList<CBCodeMethod>(this);
            this.Members = new CBList<ICBCodeElement>(this);
        }

        public CBCodeType(
            CBCodeType genericTypeDefinition,
            IEnumerable<CBCodeType> genericTypeArguments
            ) : this() {
            this.GenericTypeDefinition = genericTypeDefinition;
            this.GenericTypeArguments.AddRange(genericTypeArguments);
            this.Namespace = this.GenericTypeDefinition.Namespace;
            this.Name = this.GenericTypeDefinition.Name;
        }

        public TypeInfo? TypeInfo { get; set; }
        public CBList<ICBCodeElement> Prefix { get; }



        public CBCodeNamespace Namespace { get; set; }

        public string Name { get; set; }

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; }

        public bool IsClass { get; set; }

        public bool IsRecord { get; set; }

        public bool IsValueType { get; set; }

        public bool IsInterface { get; set; }

        public bool IsSealed { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsPartial { get; set; }

        public CBList<ICBCodeTypeDecoration> Attributes { get; }

        public CBCodeType? GenericTypeDefinition { get; set; }

        public CBCodeType? BaseType { get; set; }

        public CBList<CBCodeType> Interfaces { get; }

        public CBList<CBCodeType> GenericTypeArguments { get; }

        public CBList<CBCodeField> Fields { get; }

        public CBList<CBCodeConstructor> Constructors { get; }

        public CBList<CBCodeProperty> Properties { get; }

        public CBList<CBCodeMethod> Methods { get; }

        public CBList<ICBCodeElement> Members { get; }

        public ICBCodeElement? Parent { get; set; }


        public IEnumerable<ICBCodeElement> GetChildren() {
            var result = new List<ICBCodeElement>();
            return result;
        }

        public override string ToString() {
            return $"{this.Namespace.Name}.{this.Name}";
        }

        private string GetDebuggerDisplay() {
            return $"{this.Namespace.Name}.{this.Name}";
        }
    }


    public sealed class CBTemplateCSharpTypeDeclaration : CBNamedTemplate<CBCodeType> {
        public CBTemplateCSharpTypeDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeType value, CBRenderContext ctxt) {
            ctxt.Foreach(
                items: value.Prefix,
                eachItem: (i, ctxt) => ctxt.CallTemplateDynamic(i.Value));
            //ctxt.Foreach(
            //    items: value.Attributes,
            //    eachItem: (i, ctxt) => ctxt.CallTemplateDynamic(i.Value, CBTemplateProvider.Attribute));
            if (value.IsRecord) {
                ctxt.CallTemplateStrict(value.AccessibilityLevel)
                    .Write("record ")
                    .Write("(")
                    .Foreach(value.Properties,
                    (i, ctxt) => {
                        ctxt.CallTemplateDynamic(i.Value.PropertyType, CBTemplateProvider.TypeName).Write(i.Value.Name);
                        if (i.IsLast) { } else { ctxt.Write(","); }
                    })
                    .Write(")");
            } else {
                ctxt.CallTemplateStrict(value.AccessibilityLevel)
                    .If(value.IsAbstract, (ctxt) => ctxt.Write("abstract "))
                    .If(value.IsSealed, (ctxt) => ctxt.Write("sealed "))
                    .If(value.IsPartial, (ctxt) => ctxt.Write("partial "))
                    .Write(value.IsClass ? "class " : value.IsValueType ? "struct " : value.IsInterface ? "interface " : "??? ")
                    .Write(value.Name)
                    .CallTemplateStrict<CBCodeType>(value, "BaseTypes")
                    .Write(" {").WriteLine();
                var lst = new List<ICBCodeElement>();
                lst.AddRange(value.Fields);
                lst.AddRange(value.Constructors);
                lst.AddRange(value.Properties);
                lst.AddRange(value.Methods);
                lst.AddRange(value.Members);
                foreach (var grp in lst
                    .GroupBy(m => m switch {
                        CBCodeField codeField => codeField.IsStatic ? 0 : 1,
                        CBCodeConstructor codeConstructor => codeConstructor.IsStatic ? 2 : 3,
                        CBCodeProperty => 4,
                        CBCodeMethod codeMethod => codeMethod.IsStatic ? 6 : 7,
                        _ => 8
                    })
                    .OrderBy(kv => kv.Key)
                    ) {
                    foreach (var member in grp) {
                        ctxt.CallTemplateDynamic(member);
                    }
                    ctxt.WriteLine();
                }

                ctxt.IndentDecr().Write("}").WriteLine();
            }
        }
    }
    public sealed class CBTemplateCSharpTypeBaseTypes : CBNamedTemplate<CBCodeType> {
        public CBTemplateCSharpTypeBaseTypes()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.BaseTypes) {
        }

        public override void RenderT(CBCodeType value, CBRenderContext ctxt) {
            var lstBaseTypes = new List<CBCodeType>();
            if (value.BaseType is not null) {
                lstBaseTypes.Add(value.BaseType);
            }
            lstBaseTypes.AddRange(value.Interfaces);

            ctxt.Foreach(
                items: lstBaseTypes,
                eachItem: (i, ctxt) => {
                    if (i.IsFirst) {
                        ctxt.WriteLine(indent: +1);
                        ctxt.Write(": ");
                    } else {
                        ctxt.WriteLine();
                        ctxt.Write(", ");
                    }
                    ctxt.CallTemplateDynamic(i.Value, CBTemplateProvider.TypeName);
                },
                isEmpty: (ctxt) => {
                    ctxt.IndentIncr();
                });
        }
    }

    public sealed class CBTemplateCSharpTypeTypeName : CBNamedTemplate<CBCodeType> {
        public CBTemplateCSharpTypeTypeName()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.TypeName) {
        }

        public override void RenderT(CBCodeType value, CBRenderContext ctxt) {
            if (value.GenericTypeDefinition is null) {
                ctxt.Write(value.Namespace.Name).Write(".").Write(value.Name);
            } else {
                ctxt.Write(value.Namespace.Name).Write(".").Write(value.Name);
                ctxt.Write("<");
                var genericTypeArguments = value.GenericTypeArguments;
                for (int idx = 0; idx < genericTypeArguments.Count; idx++) {
                    if (idx > 0) { ctxt.Write(", "); }
                    ctxt.CallTemplateDynamic(genericTypeArguments[idx], CBTemplateProvider.TypeName);
                }
                ctxt.Write(">");
            }
        }
    }

    public sealed class CBTemplateCSharpTypeAttribute : CBNamedTemplate<CBCodeType> {
        public CBTemplateCSharpTypeAttribute()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Attribute) {
        }

        public override void RenderT(CBCodeType value, CBRenderContext ctxt) {
            ctxt.Write(value.Namespace.Name);
            ctxt.Write(".");
            var tn = value.Name;
            tn = tn.EndsWith("Attribute") ? tn.Substring(0, tn.Length - 9) : tn;
            ctxt.Write(tn);
        }
    }
}