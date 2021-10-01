using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Brimborium.Typing {
    public sealed class TypingRepository {
        private readonly Dictionary<MetaName, MetaType> _MetaTypes;

        private MetaNameBehaviour _NameBehaviour;

        public TypingRepository() {
            this._NameBehaviour = new MetaNameBehaviour();
            this._MetaTypes = new Dictionary<MetaName, MetaType>(this._NameBehaviour.MetaNameComparer);
        }

        public TypingRepository(MetaNameBehaviour metaNameBehaviour) {
            this._NameBehaviour = metaNameBehaviour;
            this._MetaTypes = new Dictionary<MetaName, MetaType>(this._NameBehaviour.MetaNameComparer);
        }

        public TypingList GetTypingList() {
            var result = new TypingList();
            result.Language = this._NameBehaviour.GetLanguage();
            result.MetaTypes = this._MetaTypes.Values.ToList();
            return result;
        }

        public void Add(MetaType metaType) {
            MetaName metaName = this._NameBehaviour.GetMetaName3(metaType);
            this._MetaTypes.Add(metaName, metaType);
        }

        public bool TryGetMetaType(MetaName name, [MaybeNullWhen(false)] out MetaType metaType) {
            return this._MetaTypes.TryGetValue(name, out metaType);
        }
    }

    public sealed class TypingList {
        public TypingList() {
            this.Language = string.Empty;
            this.MetaTypes = new List<MetaType>();
        }

        public string Language { get; set; }

        public List<MetaType> MetaTypes { get; set; }
    }
}
