using Brimborium.CodeFlow.Model;

namespace Demo.Logic {
    public class GnaModelPartial {
        private PartialValue<string> _Name;
        private PartialValue<bool> _Done;

        public GnaModelPartial() {
            this._Name = new PartialValue<string>();
            this._Done = new PartialValue<bool>();
        }
        public bool NameIsSet => this._Name.ValueIsSet;
        public string Name { get { return this._Name.Value!; } set { PartialValue.Set(ref this._Name, value); } }
        public bool DoneIsSet => this._Done.ValueIsSet;
        public bool Done { get { return this._Done.Value!; } set { PartialValue.Set(ref this._Done, value); } }
    }
}
