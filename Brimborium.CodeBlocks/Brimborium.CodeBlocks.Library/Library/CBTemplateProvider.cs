using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.CodeBlocks.Library {
    public class CBTemplateProvider {
        public Dictionary<Type, List<CBNamedTemplate>> Templates { get; }

        public CBTemplateProvider() {
            this.Templates = new Dictionary<Type, List<CBNamedTemplate>>();
        }

        public void AddTemplateFromFunction<T>(Action<T, CBRenderContext> render, string? name = default) {
            var template = new CBTemplateFunction<T>(render);
            this.AddTemplate(template, name);
        }

        public void AddTemplate(CBTemplate template, string? name = default) {
            var renderType = template.GetRenderType();
            if (!this.Templates.TryGetValue(renderType, out var lstTemplate)) {
                lstTemplate = new List<CBNamedTemplate>();
                this.Templates.Add(renderType, lstTemplate);
            }
            lstTemplate.Add(new CBNamedTemplate(name ?? string.Empty, template));
        }

        public CBTemplate<T> GetTemplate<T>(string? name = default) => (CBTemplate<T>)this.GetTemplate(typeof(T), name);

        public CBTemplate GetTemplate(Type renderType, string? name = default) {
            if (this.TryGetTemplate(renderType, name ?? string.Empty, out var result)) {
                return result;
            } else {
                throw new InvalidCastException($"Template for {renderType.FullName} not found");
            }
        }

        public bool TryGetTemplate(Type renderType, string name, [MaybeNullWhen(false)] out CBTemplate result) {
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
    }
}
