using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.CodeBlocks.Library {
    public class CBTemplateProvider {
        public const string CSharp = "CSharp";
        public const string Expression = "Expression";

        public Dictionary<Type, List<CBRegisterTemplate>> Templates { get; }

        public CBTemplateProvider() {
            this.Templates = new Dictionary<Type, List<CBRegisterTemplate>>();
        }

        public void AddTemplateFromFunction<T>(Action<T, CBRenderContext> render, string? name = default) {
            var template = new CBTemplateFunction<T>(render);
            this.AddTemplate(template, name);
        }

        public void AddTemplate(CBTemplate template, string? language = default, string? name = default) {
            var renderType = template.GetRenderType();
            if (!this.Templates.TryGetValue(renderType, out var lstTemplate)) {
                lstTemplate = new List<CBRegisterTemplate>();
                this.Templates.Add(renderType, lstTemplate);
            }
            lstTemplate.Add(new CBRegisterTemplate(language ?? string.Empty, name ?? string.Empty, template));
        }

        public void AddTemplate(CBRegisterTemplate namedTemplate) {
            var renderType = namedTemplate.Template.GetRenderType();
            if (!this.Templates.TryGetValue(renderType, out var lstTemplate)) {
                lstTemplate = new List<CBRegisterTemplate>();
                this.Templates.Add(renderType, lstTemplate);
            }
            lstTemplate.Add(namedTemplate);
        }

        public CBTemplate GetTemplate(Type currentType, Type staticType, string name) {
            Type renderType = currentType;
            do {
                if (this.TryGetTemplate(renderType, name, out var result)) {
                    return result;
                }
                if (renderType.BaseType is not null) {
                    renderType = renderType.BaseType;
                } else {
                    break;
                }
            } while (!renderType.Equals(staticType) && renderType.Equals(typeof(object)));
            {
                throw new InvalidCastException($"Template for {currentType.FullName} - {staticType.FullName} '{name}' not found.");
            }
        }

        public CBTemplate<T> GetTemplate<T>(string? name = default) => (CBTemplate<T>)this.GetTemplate(typeof(T), name);

        public CBTemplate GetTemplate(Type renderType, string? name = default) {
            if (this.TryGetTemplate(renderType, name ?? string.Empty, out var result)) {
                return result;
            } else {
                throw new InvalidCastException($"Template for {renderType.FullName} '{name}' not found.");
            }
        }

        public bool TryGetTemplate(Type renderType, string name, [MaybeNullWhen(false)] out CBTemplate result) {
            {
                if (this.Templates.TryGetValue(renderType, out var lstTemplate)) {
                    //CBTemplate? withNoName = null;
                    foreach (var namedTemplate in lstTemplate) {
                        if (namedTemplate.CanRenderType(renderType, name)) {
                            result = namedTemplate.Template;
                            return true;
                        }
                        //if (string.IsNullOrEmpty(namedTemplate.Name)) {
                        //    withNoName = namedTemplate.Template;
                        //}
                    }
                    //if (withNoName is not null) {
                    //    result = withNoName;
                    //    return true;
                    //}
                }
            }
            {
                if (this.Templates.TryGetValue(typeof(object), out var lstTemplate)) {
                    //CBTemplate? withNoName = null;
                    foreach (var namedTemplate in lstTemplate) {
                        if (namedTemplate.CanRenderType(renderType, name)) {
                            result = namedTemplate.Template;
                            return true;
                        }
                        //if (string.IsNullOrEmpty(namedTemplate.Name)) {
                        //    withNoName = namedTemplate.Template;
                        //}
                    }
                    //if (withNoName is not null) {
                    //    result = withNoName;
                    //    return true;
                    //}
                }
            }
            result = null;
            return false;
        }

        public CBTemplateProvider GetTemplateByLanguage(string language) {
            var result = new CBTemplateProvider();
            foreach (var kv in this.Templates) {
                foreach (var namedTemplate in kv.Value) {
                    if (string.Equals(namedTemplate.Language, language, StringComparison.OrdinalIgnoreCase)) {
                        result.AddTemplate(namedTemplate);
                    }
                }
            }
            return result;
        }
    }
}
