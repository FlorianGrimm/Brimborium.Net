using System;
using System.Reflection;

namespace Brimborium.Typing {
    public class DotNetTyping {
        public DotNetTyping(TypingRepository typingRepository) {
            this.TypingRepository = typingRepository;
        }

        public TypingRepository TypingRepository { get; }

        public void ScanAssembly(System.Reflection.Assembly assembly) {
            var assemblyName = assembly.GetName().Name;
            var assemblyTypes = assembly.GetTypes();
            foreach (var assemblyType in assemblyTypes) {
                this.ScanType(assemblyType);
            }
        }
        public MetaType ScanType(Type type) {
            this.TypingRepository
            // type.Namespace
            // type.Name
            var assemblyName = type.Assembly.GetName().Name;
            var result = new MetaType();
            
            return result;
        }
    }
}
