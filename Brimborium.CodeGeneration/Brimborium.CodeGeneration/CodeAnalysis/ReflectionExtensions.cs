// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.CodeGeneration.CodeAnalysis {
    internal static partial class ReflectionExtensions {
        public static CustomAttributeData? GetCustomAttributeData(this MemberInfo memberInfo, Type type) {
            return memberInfo.CustomAttributes
                .FirstOrDefault(a => type.IsAssignableFrom(a.AttributeType))
                ;
        }

        public static TValue GetConstructorArgument<TValue>(this CustomAttributeData customAttributeData, int index) {
            return index < customAttributeData.ConstructorArguments.Count ? (TValue)customAttributeData.ConstructorArguments[index].Value! : default!;
        }

        public static bool IsInitOnly(this MethodInfo method) {
            if (method is null) {
                throw new ArgumentNullException(nameof(method));
            }

            var methodInfoWrapper = (MethodInfoWrapper)method;
            return methodInfoWrapper.IsInitOnly;
        }

        public static Location? GetDiagnosticLocation(this Type type) {
            Debug.Assert(type is TypeWrapper);
            return ((TypeWrapper)type).Location;
        }

        public static Location? GetDiagnosticLocation(this PropertyInfo propertyInfo) {
            Debug.Assert(propertyInfo is PropertyInfoWrapper);
            return ((PropertyInfoWrapper)propertyInfo).Location;
        }

        public static Location? GetDiagnosticLocation(this FieldInfo fieldInfo) {
            Debug.Assert(fieldInfo is FieldInfoWrapper);
            return ((FieldInfoWrapper)fieldInfo).Location;
        }
    }
}
