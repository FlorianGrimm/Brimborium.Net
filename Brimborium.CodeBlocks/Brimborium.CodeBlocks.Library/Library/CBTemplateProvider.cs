using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBTemplateProvider {
        public const string CSharp = "CSharp";
        public const string Common = "";
        public const string Attribute = "Attribute";
        public const string Declaration = "Declaration";
        public const string BaseTypes = "BaseTypes";
        public const string Expression = "Expression";
        public const string TypeName = "TypeName";

        public Dictionary<Type, List<CBRegisterTemplate>> Templates { get; }

        public CBTemplateProvider() {
            this.Templates = new Dictionary<Type, List<CBRegisterTemplate>>();
        }

        public void AddTemplateFromFunction<T>(Action<T, CBRenderContext> render, string? language, string? name) {
            var template = new CBTemplateFunction<T>(render);
            this.AddTemplate(template, language, name);
        }

        public void AddTemplate(CBTemplate template, string? language, string? name ) {
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

        public bool TryGetTemplate(Type currentType, Type staticType, string? name, bool throwIfNotFound, [MaybeNullWhen(false)] out CBTemplate result) {
            name ??= string.Empty;
            Type renderType = currentType;
            while (true) {
                if (this.TryGetTemplateSingle(renderType, name, out result)) {
                    return true;
                } else {
                    if (renderType.Equals(staticType)) {
                        break;
                    } else if (renderType.BaseType is not null) {
                        renderType = renderType.BaseType;
                        continue;
                    } else {
                        break;
                    }
                }
            }
            if (throwIfNotFound) {
                throw new InvalidCastException($"Template for {currentType.FullName} - {staticType.FullName} '{name}' not found.");
            }
            {
                result = default;
                return false;
            }
        }

        public bool TryGetTemplate<T>(string? name, bool throwIfNotFound, [MaybeNullWhen(false)] out CBTemplate result) {
            if (this.TryGetTemplate(typeof(T), name, throwIfNotFound, out var template)) {
                result = template;
                return true;
            } else {
                result = default;
                return false;
            }
        }

        public bool TryGetTemplate(Type renderType, string? name, bool throwIfNotFound, [MaybeNullWhen(false)] out CBTemplate result) {
            if (this.TryGetTemplateSingle(renderType, name ?? string.Empty, out result)) {
                return true;
            }
            if (throwIfNotFound) {
                throw new InvalidCastException($"Template for {renderType.FullName} '{name}' not found.");
            }
            {
                result = default;
                return false;
            }
        }

        public bool TryGetTemplateSingle(Type renderType, string name, [MaybeNullWhen(false)] out CBTemplate result) {
            {
                if (this.Templates.TryGetValue(renderType, out var lstTemplate)) {
                    CBTemplate? withNoName = null;
                    foreach (var namedTemplate in lstTemplate) {
                        if (namedTemplate.CanRenderType(renderType, name)) {
                            result = namedTemplate.Template;
                            return true;
                        }
                        if (string.IsNullOrEmpty(namedTemplate.Name)) {
                            withNoName = namedTemplate.Template;
                        }
                    }
                    if (withNoName is not null) {
                        result = withNoName;
                        return true;
                    }
                }
            }
            {
                if (this.Templates.TryGetValue(typeof(object), out var lstTemplate)) {
                    CBTemplate? withNoName = null;
                    foreach (var namedTemplate in lstTemplate) {
                        if (namedTemplate.CanRenderType(renderType, name)) {
                            result = namedTemplate.Template;
                            return true;
                        }
                        if (string.IsNullOrEmpty(namedTemplate.Name)) {
                            withNoName = namedTemplate.Template;
                        }
                    }
                    if (withNoName is not null) {
                        result = withNoName;
                        return true;
                    }
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
