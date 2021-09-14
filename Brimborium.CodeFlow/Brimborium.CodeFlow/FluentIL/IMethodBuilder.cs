namespace Brimborium.CodeFlow.FluentIL
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Defines the method builder interface.
    /// </summary>
    public interface IMethodBuilder
    {
        /// <summary>
        /// Gets or sets the methods attributes.
        /// </summary>
        MethodAttributes Attributes { get; set; }

        /// <summary>
        /// Gets the methods body.
        /// </summary>
        /// <returns>A <see cref="IEmitter"/>.</returns>
        IEmitter Body();

        /// <summary>
        /// Provides access to the method body.
        /// </summary>
        /// <param name="action">An action to emit the body IL.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Body(Action<IEmitter> action);

        /// <summary>
        /// Sets the methods atrributes.
        /// </summary>
        /// <param name="attributes">The method attributes.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder MethodAttributes(MethodAttributes attributes);

        /// <summary>
        /// Sets the calling convention.
        /// </summary>
        /// <param name="convention">The calling convention.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder CallingConvention(CallingConventions convention);

        /// <summary>
        /// Sets the methods return type.
        /// </summary>
        /// <typeparam name="TReturn">The return type.</typeparam>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Returns<TReturn>();

        /// <summary>
        /// Sets the methods return type.
        /// </summary>
        /// <param name="returnType">The return type.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Returns(Type returnType);

        /// <summary>
        /// Sets the methods return type.
        /// </summary>
        /// <param name="genericType">The return type.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Returns(IGenericParameterBuilder genericType);

        /// <summary>
        /// Sets the methods return type.
        /// </summary>
        /// <param name="genericTypeDefinition">A generic type definition.</param>
        /// <param name="genericTypes">A list of generic type parameters.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Returns(Type genericTypeDefinition, params IGenericParameterBuilder[] genericTypes);

        /// <summary>
        /// Adds a parameter to the method.
        /// </summary>
        /// <typeparam name="TParam">The parameters type.</typeparam>
        /// <param name="parameterName">The parameters name.</param>
        /// <param name="attrs">The parameters attributes.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instsnce.</returns>
        IMethodBuilder Param<TParam>(string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        /// <summary>
        /// Adds a parameter to the method.
        /// </summary>
        /// <param name="parameterType">The parameters type.</param>
        /// <param name="parameterName">The parameters name.</param>
        /// <param name="attrs">The parameters attribute.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Param(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        /// <summary>
        /// Adds a parameter to the method.
        /// </summary>
        /// <param name="action">A parameter builder action.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Param(Action<IParameterBuilder> action);

        /// <summary>
        /// Adds a parameter to the method.
        /// </summary>
        /// <param name="parameter">A parameter builder.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Param(IParameterBuilder parameter);

        /// <summary>
        /// Defines the methods parameters.
        /// </summary>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Params(params Type[] parameterTypes);

        /// <summary>
        /// Defines the methods parameters.
        /// </summary>
        /// <param name="parameters">A list of parameter builders.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder Params(params IParameterBuilder[] parameters);

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <typeparam name="TParam">The parameters type.</typeparam>
        /// <param name="parameterName">The parameters name.</param>
        /// <param name="attrs">The parameters attributes.</param>
        /// <returns>The <see cref="IParameterBuilder"/> instsnce.</returns>
        IParameterBuilder CreateParam<TParam>(string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <param name="parameterType">The parameters type.</param>
        /// <param name="parameterName">The parameters name.</param>
        /// <param name="attrs">The parameters attribute.</param>
        /// <returns>The <see cref="IParameterBuilder"/> instance.</returns>
        IParameterBuilder CreateParam(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None);

        /// <summary>
        /// Checks if the method has a named parameter.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>True if it has; otherwise false.</returns>
        bool HasParameter(string parameterName);

        /// <summary>
        /// Gets a named parameter.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>A <see cref="IParameterBuilder"/> if found; otherwise null.</returns>
        IParameterBuilder GetParameter(string parameterName);

        /// <summary>
        /// Defines a generic parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The <see cref="IGenericParameterBuilder"/> instance.</returns>
        IGenericParameterBuilder NewGenericParameter(string parameterName);

        /// <summary>
        /// Defines a generic parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="parameterBuilder">A generic parameter builder action. </param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder NewGenericParameter(string parameterName, Action<IGenericParameterBuilder> parameterBuilder);

        /// <summary>
        /// Defines generic parameters.
        /// </summary>
        /// <param name="parameterNames">The names of the parameters.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder NewGenericParameters(params string[] parameterNames);

        /// <summary>
        /// Defines generic parameters.
        /// </summary>
        /// <param name="parameterNames">The names of the parameters.</param>
        /// <param name="action">The action to update the parameters.</param>
        /// <returns>The <see cref="IMethodBuilder"/> instance.</returns>
        IMethodBuilder NewGenericParameters(string[] parameterNames, Action<IGenericParameterBuilder[]> action);

        /// <summary>
        /// Gets a generic parameter.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>A <se cref="GenericTypeParameterBuilder"/> instance if found; otherwise null.</returns>
        GenericTypeParameterBuilder GetGenericParameter(string parameterName);

        /// <summary>
        /// Sets a custom attribute.
        /// </summary>
        /// <param name="customAttribute">The custom attribute.</param>
        /// <returns>The <see cref="MethodInfo"/> instance.</returns>
        IMethodBuilder SetCustomAttribute(CustomAttributeBuilder customAttribute);

        /// <summary>
        /// Sets the methods implementation flags.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns>The <see cref="MethodInfo"/> instance.</returns>
        IMethodBuilder SetImplementationFlags(MethodImplAttributes attributes);

        /// <summary>
        /// Defines the method.
        /// </summary>
        /// <returns>The <see cref="MethodInfo"/> instance.</returns>
        MethodBuilder Define();
    }
}