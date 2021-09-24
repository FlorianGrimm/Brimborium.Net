using System;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.CodeBlocks.Library {
    public class CBValue {
        private CBContext? _Context;

        public CBValue() {
            this.Name = string.Empty;
        }

        public CBValue(string name) {
            this.Name = name;
        }

        public string Name { get; set; }

        internal void SetContext(CBContext cbContext) {
            this._Context = cbContext;
        }

        public CBContext? GetContext() => this._Context;

        public virtual object? GetValue() => null;
    }

    public class CBExpressionValue<T> : CBValue {
        public CBExpressionValue() : base() {
        }

        public CBExpressionValue(string name) : base(name) {
        }

        public T Value { get => this.GetValueT(); set => this.SetValueT(value); }

        public virtual bool TryGetValueT([MaybeNullWhen(false)] out T value) {
            value = this.GetValueT();
            if (typeof(T).IsClass) {
                return (Value is not null);
            } else {
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
                return true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
            }
        }
        protected virtual T GetValueT() {
            throw new NotImplementedException();
        }

        protected virtual void SetValueT(T value) {
            throw new NotImplementedException();
        }

        public override object? GetValue() => this.GetValueT();
    }

    public class CBValue<T> : CBExpressionValue<T> {
        private T _Value;

        public CBValue(string name, T value) : base(name) {
            this._Value = value;
        }

        protected override T GetValueT() { return this._Value; }

        protected override void SetValueT(T value) { this._Value = value; }
    }

    /*
    public class CBValueFunction1<T> : CBValue<T>{
        public CBValueFunction1(string name, Func<T> func): base(name) {
        }
    }
    */

    //public class CBExpressionValue<T> : CBValue {
    //    public CBExpressionValue() :base() {
    //    }
    //    public CBExpressionValue(string name): base(name) {
    //    }
    //}
}
