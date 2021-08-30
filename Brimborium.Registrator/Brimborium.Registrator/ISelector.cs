using Microsoft.Extensions.DependencyInjection;

using System.Collections.Generic;

namespace Brimborium.Registrator {
    public interface ISelector {
        void Populate(RegistrationStrategy registrationStrategy, ISelectorTarget selectorTarget);
    }

    public interface ISelectorTarget {
        List<ServicePopulation> Items { get; }
        void AddServicePopulation(ServicePopulation value);
    }

}
