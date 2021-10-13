using System.Collections.Generic;

namespace System.Linq {
    public static class BrimboriumEnumerableExtension {
        public static EnumerableSplit<M, I> SplitByTypeToList<I, M>(IEnumerable<I> items) {
            List<M> matches = new List<M>();
            List<I> others = new List<I>();
            foreach (I item in items) {
                if (item is M m) {
                    matches.Add(m);
                } else {
                    others.Add(item);
                }
            }
            return new EnumerableSplit<M, I>(matches, others);
        }
    }

    public sealed record EnumerableSplit<M, O>(List<M> Matches, List<O> Others);
}
