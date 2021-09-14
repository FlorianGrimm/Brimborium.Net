#pragma warning disable IDE0041

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.Extensions {
    public static class NullExtension {
        public static T EnsureNotNull<T>(this T? instance)
            where T:class
            {
            if (object.ReferenceEquals(instance, null)) {
                throw new ArgumentNullException();
            } else { 
                return instance;
            }
        }
    }
}
