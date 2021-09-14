namespace Brimborium.CodeFlow.FluentIL.Builders {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a <see cref="MethodInfo"/> builder.
    /// </summary>
    public class MethodInfoBuilder {
        /// <summary>
        /// A list of matching methods.
        /// </summary>
        private IEnumerable<MethodInfo> _Methods;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInfoBuilder"/> class.
        /// </summary>
        /// <param name="type">The type that the method belongs to.</param>
        /// <param name="methodName">The name of the method to find.</param>
        public MethodInfoBuilder(Type type, string methodName) {
            this._Methods = type
                .GetMethods()
                .Where(m => m.Name == methodName);
        }

        /// <summary>
        /// Looks forgeneric definition methods.
        /// </summary>
        /// <returns>The <see cref="MethodInfoBuilder"/> instance.</returns>
        public MethodInfoBuilder IsGenericDefinition() {
            this._Methods = this._Methods.Where(m => m.IsGenericMethodDefinition);
            return this;
        }

        /// <summary>
        /// Lokks for methods with matching parameter types.
        /// </summary>
        /// <param name="types">A list of parameter types.</param>
        /// <returns>The <see cref="MethodInfoBuilder"/> instance.</returns>
        public MethodInfoBuilder HasParameterTypes(params Type[] types) {
            this._Methods = this._Methods
                .Select(m => new {
                    Method = m,
                    Parms = m.GetParameters()
                })
                .Where(m =>
                    m.Parms.Length == types.Length &&
                    this.ParameterTypesMatch(m.Parms, types))
                .Select(m => m.Method);

            return this;
        }

        /// <summary>
        /// Looks for methods with a given metadata token.
        /// </summary>
        /// <param name="metadataToken">THe metadata token.</param>
        /// <returns>The <see cref="MethodInfoBuilder"/> instance.</returns>
        public MethodInfoBuilder HasMetadataToken(int metadataToken) {
            this._Methods = this._Methods
                .Where(m => m.MetadataToken == metadataToken);

            return this;
        }

        /// <summary>
        /// Returns the first method found or null if there are no matching methods.
        /// </summary>
        /// <returns>A <see cref="MethodInfo"/> instance; otherwise null.</returns>
        public MethodInfo FirstOrDefault() {
            return this._Methods.FirstOrDefault();
        }

        /// <summary>
        /// Returns all matching methods.
        /// </summary>
        /// <returns>A list of matching <see cref="MethodInfo"/> instances.</returns>
        public IEnumerable<MethodInfo> All() {
            return this._Methods;
        }

        /// <summary>
        /// Checks if the parameters match the types.
        /// </summary>
        /// <param name="parms">The methods parameters.</param>
        /// <param name="types">The type list to match with.</param>
        /// <returns>True or false.</returns>
        private bool ParameterTypesMatch(ParameterInfo[] parms, Type[] types) {
            if (parms.Length < types.Length) {
                return false;
            }

            for (int i = 0; i < types.Length; i++) {
                if (types[i] != null) {
                    Type parmType = parms[i].ParameterType;
                    if (types[i].IsGenericTypeDefinition == true) {
                        parmType = parmType.GetGenericTypeDefinition();
                    }

                    if (parmType != types[i]) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}