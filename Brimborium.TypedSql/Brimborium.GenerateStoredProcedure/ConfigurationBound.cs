
using System.Collections.Generic;
using System.Linq;

namespace Brimborium.GenerateStoredProcedure {
    public class ConfigurationBound {
        public ConfigurationBound() {
            this.RenderBindings = new List<RenderBinding>();
            this.ReplacementBindings = new List<RenderBinding>();
        }
        public List<RenderBinding> RenderBindings { get; }
        public List<RenderBinding> ReplacementBindings { get; }

        public void AddRenderBindings(
            string comment,
            IEnumerable<RenderBinding> renderBindings
            ) {
            var lstRenderBindings = renderBindings.ToList();
            if (lstRenderBindings.Count == 0) {
                System.Console.Out.WriteLine($"Render {comment}: none");
            } else {
                System.Console.Out.WriteLine($"Render {comment}: {lstRenderBindings.Count}");
            }
            this.RenderBindings.AddRange(lstRenderBindings);
        }

        public void AddReplacementBindings(
            string comment,
            IEnumerable<RenderBinding> renderBindings
            ) {
            var lstRenderBindings = renderBindings.ToList();
            if (lstRenderBindings.Count == 0) {
                System.Console.Out.WriteLine($"Replacement {comment}: none");
            } else {
                System.Console.Out.WriteLine($"Replacement {comment}: {lstRenderBindings.Count}");
            }
            this.ReplacementBindings.AddRange(lstRenderBindings);
        }

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
