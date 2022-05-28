﻿namespace Brimborium.CodeGeneration {
    public static class PrintContextExtensions {
        public static PrintContext AppendIndented<T>(this PrintContext ctxt, T data, Action<T, PrintContext> render, string addIndent = "    ") {
           var ctxtIndented = ctxt.GetIndented(addIndent);
           render(data, ctxtIndented);
           return ctxt;
        }

        public static PrintContext RenderTemplate<T>(this PrintContext ctxt, T data, RenderTemplate<T> template) {
            template.Render(data, ctxt);
            return ctxt;
        }

        public static PrintContext AppendList<T>(this PrintContext ctxt, IEnumerable<T> src, Action<T, PrintContext> render) {
            var list = src as List<T> ?? src.ToList();
            var cnt = list.Count;
            for (var idx = 0; idx < cnt; idx++) {
                render(list[idx], ctxt.SetListPosition(idx, cnt));
            }
            return ctxt;
        }
        public static PrintContext AppendList<T>(this PrintContext ctxt, IEnumerable<T> src, RenderTemplate<T> template) {
            var list = src as List<T> ?? src.ToList();
            var cnt = list.Count;
            for (var idx = 0; idx < cnt; idx++) {
                template.Render(list[idx], ctxt.SetListPosition(idx, cnt));
            }
            return ctxt;
        }


        public static PrintContext AppendCurlyBlock<T>(this PrintContext ctxt, T Data, Action<T, PrintContext>? FirstBefore, Action<T, PrintContext> Render, Action<T, PrintContext>? LastAfter) {
            if (FirstBefore is null) {
                ctxt.AppendLine("{");
            } else { 
                FirstBefore(Data, ctxt);
                ctxt.AppendLine(" {");
            }
            Render(Data, ctxt.GetIndented());
            if (LastAfter is null) {
                ctxt.AppendLine("}");
            } else {
                ctxt.Append("} ");
                LastAfter(Data, ctxt);

            }
            return ctxt;
        }

        public static PrintContext AppendCurlyBlock<T>(this PrintContext ctxt, T Data, RenderTemplate<T>? FirstBefore, RenderTemplate<T> Inner, RenderTemplate<T>? LastAfter) {
            if (FirstBefore is null) {
                ctxt.AppendLine("{");
            } else { 
                FirstBefore.Render(Data, ctxt);
                ctxt.AppendLine(" {");
            }
            Inner.Render(Data, ctxt.GetIndented());
            if (LastAfter is null) {
                ctxt.AppendLine("}");
            } else {
                ctxt.Append("} ");
                LastAfter.Render(Data, ctxt);
            }
            return ctxt;
        }

        public static string SwitchFirst(this PrintContext ctxt, string firstValue, string otherValue) {
            if (ctxt.IsFirst) {
                return firstValue;
            } else {
                return otherValue;
            }
        }
        public static string IfFirst(this PrintContext ctxt, string value, bool condition = true) {
            if (ctxt.IsFirst && condition) {
                return value;
            } else {
                return string.Empty;
            }
        }
        public static string IfNotFirst(this PrintContext ctxt, string value, bool condition = true) {
            if (ctxt.IsFirst && condition) {
                return string.Empty;
            } else {
                return value;
            }
        }
        public static string IfLast(this PrintContext ctxt, string value, bool condition = true) {
            if (ctxt.IsLast && condition) {
                return value;
            } else {
                return string.Empty;
            }
        }
        public static string IfNotLast(this PrintContext ctxt, string value, bool condition = true) {
            if (ctxt.IsLast && condition) {
                return string.Empty;
            } else {
                return value;
            }
        }
    }
}