using Brimborium.Registrator;
using Brimborium.Registrator.Internals;

using Microsoft.Extensions.DependencyModel;

using System;

namespace Microsoft.Extensions.DependencyInjection {
    public static partial class ServiceCollectionExtensions {
        /// <summary>
        /// Adds registrations to the <paramref name="services"/> collection using
        /// conventions specified using the <paramref name="actionAdd"/>.
        /// </summary>
        /// <param name="services">The services to add to.</param>
        /// <param name="actionAdd">The configuration action.</param>
        /// <exception cref="ArgumentNullException">If either the <paramref name="services"/>
        /// or <paramref name="actionAdd"/> arguments are <c>null</c>.</exception>
        public static IServiceCollection AddRegistrator(
            this IServiceCollection services,
            Action<ITypeSourceSelector>? actionAdd = default,
            Action<ISelectorTarget>? actionRevisit = default) {
            Preconditions.NotNull(services, nameof(services));
            
            var selector = new TypeSourceSelector();

            if (actionAdd is not null) {
                actionAdd(selector);
            } else {
                selector.FromApplicationDependencies(DependencyContext.Default)
                    .AddClasses()
                    .UsingAttributes();
            }

            var selectorTarget = new SelectorTarget();

            ((ISelector)selector).Populate(RegistrationStrategy.Append, selectorTarget);

            if (actionRevisit is not null) { 
                actionRevisit(selectorTarget);
            }

            selectorTarget.Build(services);

            return services;
        }
    }
}
