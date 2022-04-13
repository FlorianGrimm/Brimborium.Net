
using System;

namespace Brimborium.TypedStoredProcedure {
    public sealed record CSTypeDefinition(
            Type? Type,
            string? Name,
            Microsoft.SqlServer.Management.Smo.SqlDataType? SqlDataType = null,
            int? MaximumLength = null,
            bool IsList = false,
            bool IsScalar = false,
            bool CanBeEmpty = false,
            bool IsAsync = false,
            MemberDefinition[]? Members = null,
            ParameterConverterDefinition? ParameterConverter = null
        ) {
        public static CSTypeDefinition Void => new CSTypeDefinition(null, "void", null, null);
        public static CSTypeDefinition VoidAsync => new CSTypeDefinition(null, "void", null, null, IsAsync: true);
        public static CSTypeDefinition None => new CSTypeDefinition(null, null, null, null);
        public static CSTypeDefinition TypeOf<T>(
            Microsoft.SqlServer.Management.Smo.SqlDataType? sqlDataType = null,
            int? maximumLength = null,
            bool isList = false,
            bool isScalar = false,
            bool canBeEmpty = false,
            bool isAsync = false,
            MemberDefinition[]? members = null,
            ParameterConverterDefinition? ParameterConverter = null) {
            return new CSTypeDefinition(
                Type: typeof(T),
                Name: typeof(T).FullName,
                SqlDataType: sqlDataType,
                MaximumLength: maximumLength,
                IsList: isList,
                IsScalar: isScalar,
                CanBeEmpty: canBeEmpty,
                IsAsync: isAsync,
                Members: members,
                ParameterConverter: ParameterConverter);
        }
        public bool IsNone() => this.Name == null;
        public bool IsVoid() => this.Name == "void";
    }
}
