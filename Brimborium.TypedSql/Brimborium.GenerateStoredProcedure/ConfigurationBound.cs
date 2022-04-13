
using System.Collections.Generic;

namespace Brimborium.GenerateStoredProcedure {
    public class ConfigurationBound {
        public ConfigurationBound() {
            this.RenderBindings = new List<RenderBinding>();
            this.ReplacementBindings = new List<RenderBinding>();
        }
        public List<RenderBinding> RenderBindings { get; }
        public List<RenderBinding> ReplacementBindings { get; }

        public static List<ReplacementBinding<T>> CreateReplacementBinding<T>(
            List<ReplacementTemplate<T>> lstReplacementTemplates,
            IEnumerable<T> lstData) {
            var result = new List<ReplacementBinding<T>>();
            foreach (var replacementTemplate in lstReplacementTemplates) {
                foreach (var data in lstData) {
                    var replacementBinding = new ReplacementBinding<T>(
                        Name: replacementTemplate.Name,
                        Data: data,
                        Template: replacementTemplate.Template);
                    result.Add(replacementBinding);
                }
            }
            return result;
        }
    }
}
