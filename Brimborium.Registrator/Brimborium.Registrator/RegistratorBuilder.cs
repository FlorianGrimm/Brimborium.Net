using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.Registrator {
    public  class RegistratorBuilder {
        private IServiceCollection _Services;


        public RegistratorBuilder(IServiceCollection services) {
            this._Services = services;
        }

        public void Build() {
            this._Services.AddServicesWithRegistrator(
                (ITypeSourceSelector typeSourceSelector) => {
                    var implementationTypeSelector = typeSourceSelector.FromDependencyContext(DependencyContext.Default);
                    //implementationTypeSelector.AddClasses()
                },
                (ISelectorTarget selectorTarget) => { 
                }
                );
        }
    }
}
