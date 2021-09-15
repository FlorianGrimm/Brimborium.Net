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
                throw new ArgumentNullException("instance");
            } else { 
                return instance;
            }
        }

        public static T EnsureNotNull<T>(this T? instance, string msg)
            where T : class {
            if (object.ReferenceEquals(instance, null)) {
                throw new ArgumentNullException(msg);
            } else {
                return instance;
            }
        }

        public static T EnsureNotNull<T>(this T? instance, Func<Exception> createException)
            where T : class {
            if (object.ReferenceEquals(instance, null)) {
                throw createException();
            } else {
                return instance;
            }
        }


    }
}
