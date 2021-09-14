namespace Brimborium.CodeFlow.FluentIL.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// An implementation of the <see cref="IParameterBuilder"/> interface.
    /// </summary>
    internal class FluentParameterBuilder
        : IParameterBuilder
    {
        /// <summary>
        /// The parameters type.
        /// </summary>
        private Type parameterType;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentParameterBuilder"/> class.
        /// </summary>
        public FluentParameterBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentParameterBuilder"/> class.
        /// </summary>
        /// <param name="parameterType">The parameters type.</param>
        /// <param name="parameterName">The parameters name.</param>
        /// <param name="attrs">The parameters attributes.</param>
        public FluentParameterBuilder(Type parameterType, string parameterName, ParameterAttributes attrs)
        {
            this.parameterType = parameterType;
            this.ParameterName = parameterName;
            this.Attributes = attrs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentParameterBuilder"/> class.
        /// </summary>
        /// <param name="parameterType">The parameters type.</param>
        /// <param name="parameterName">The parameters name.</param>
        /// <param name="attrs">The parameters attributes.</param>
        public FluentParameterBuilder(IGenericParameterBuilder parameterType, string parameterName, ParameterAttributes attrs)
        {
            this.GenericParameterType = parameterType;
            this.ParameterName = parameterName;
            this.Attributes = attrs;
        }

        /// <summary>
        /// Gets the generic type parameter.
        /// </summary>
        internal IGenericParameterBuilder GenericParameterType { get; }

        /// <summary>
        /// Gets the parameters type.
        /// </summary>
        internal Type ParameterType
        {
            get
            {
                return this.parameterType ?? this.GenericParameterType.AsType();
            }
        }

        /// <summary>
        /// Gets the parameters name.
        /// </summary>
        internal string ParameterName { get; private set; }

        /// <summary>
        /// Gets the parameters attributes.
        /// </summary>
        internal ParameterAttributes Attributes { get; private set; } = ParameterAttributes.None;

        /// <summary>
        /// Gets the parameters custom attributes.
        /// </summary>
        internal List<CustomAttributeBuilder> CustomAttributes { get; private set; }

        /// <inheritdoc/>
        public IParameterBuilder Type<T>()
        {
            return this.Type(typeof(T));
        }

        /// <inheritdoc/>
        public IParameterBuilder Type(Type parameterType)
        {
            this.parameterType = parameterType;
            return this;
        }

        /// <inheritdoc/>
        public IParameterBuilder Name(string parameterName)
        {
            this.ParameterName = parameterName;
            return this;
        }

        /// <inheritdoc/>
        public IParameterBuilder HasDefault()
        {
            this.Attributes |= ParameterAttributes.HasDefault;
            return this;
        }

        /// <inheritdoc/>
        public IParameterBuilder HasFieldMarshal()
        {
            this.Attributes |= ParameterAttributes.HasFieldMarshal;
            return this;
        }

        /// <inheritdoc/>
        public IParameterBuilder In()
        {
            this.Attributes |= ParameterAttributes.In;
            return this;
        }

        /// <inheritdoc/>
        public IParameterBuilder Lcid()
        {
            this.Attributes |= ParameterAttributes.Lcid;
            return this;
        }

        /// <inheritdoc/>
        public IParameterBuilder Optional()
        {
            this.Attributes |= ParameterAttributes.Optional;
            return this;
        }

        /// <inheritdoc/>
        public IParameterBuilder Out()
        {
            this.Attributes |= ParameterAttributes.Out;
            return this;
        }

        /// <inheritdoc/>
        public IParameterBuilder Retval()
        {
            this.Attributes |= ParameterAttributes.Retval;
            return this;
        }

        /// <inheritdoc/>
        public IParameterBuilder SetCustomAttribute(CustomAttributeBuilder attributeBuilder)
        {
            this.CustomAttributes = this.CustomAttributes ?? new List<CustomAttributeBuilder>();
            this.CustomAttributes.Add(attributeBuilder);
            return this;
        }
    }
}