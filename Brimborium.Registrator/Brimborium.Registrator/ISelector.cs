using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Registrator
{
    public interface ISelector
    {
        void Populate(IServiceCollection services, RegistrationStrategy options);
    }
}
