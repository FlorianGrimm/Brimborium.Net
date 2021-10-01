using System;

namespace Brimborium.Typing {
    public class MetaNameBehaviour {
        public MetaNameBehaviour() {
            this.MetaNameComparer = new MetaNameComparer(StringComparer.Ordinal);
        }

        public MetaNameBehaviour(MetaNameComparer metaNameComparer) {
            this.MetaNameComparer = metaNameComparer;
        }

        public virtual MetaNameComparer MetaNameComparer { get; set; }

        public virtual string GetLanguage() => string.Empty;

        public virtual MetaName GetMetaName3(MetaType metaType) {
            MetaName metaName = new MetaName(
                metaType.ContainerName,
                metaType.FullName,
                metaType.Name
                );
            return metaName;
        }

        public virtual MetaName GetMetaName2(MetaType metaType) {
            MetaName metaName = new MetaName(
                string.Empty,
                metaType.FullName,
                metaType.Name
                );
            return metaName;
        }

        public virtual string ToString2(IMetaName metaName) {
            if (string.IsNullOrEmpty(metaName.Namespace)) {
                return Escape(metaName.Name);
            } else {
                return $"{Escape(metaName.Namespace)}.{Escape(metaName.Name)}";
            }
        }

        public virtual string ToString3(IMetaName metaName) {
            if (string.IsNullOrEmpty(metaName.ContainerName)) {
                if (string.IsNullOrEmpty(metaName.Namespace)) {
                    return Escape(metaName.Name);
                } else {
                    return $"{Escape(metaName.Namespace)}.{Escape(metaName.Name)}";
                }
            } else {
                return $"{Escape(metaName.ContainerName)}.{Escape(metaName.Namespace)}.{Escape(metaName.Name)}";
            }
        }

        public virtual string Escape(string name) {
            return name;
        }
    }

    public sealed class DotNetMetaNameBehaviour : MetaNameBehaviour {
        public DotNetMetaNameBehaviour() :base(new MetaNameComparer(StringComparer.Ordinal)) {
        }

        public override string GetLanguage() => "DotNet";
    }


    public sealed class SQLMetaNameBehaviour : MetaNameBehaviour {
        public SQLMetaNameBehaviour() :base(new MetaNameComparer(StringComparer.OrdinalIgnoreCase)) {
        }

        public override string GetLanguage() => "T-SQL";

        public override string Escape(string name) {
            return $"[{name}]";
        }

    }
}
