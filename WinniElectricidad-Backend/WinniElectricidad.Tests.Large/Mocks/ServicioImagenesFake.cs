using Microsoft.AspNetCore.Http;
using WinniElectricidad.Api.Servicios;

namespace WinniElectricidad.Tests.Large.Mocks;

public class ServicioImagenesFake : IServicioImagenes
{
    public Task<string> GuardarAsync(IFormFile archivo, string carpeta)
    {
        return Task.FromResult(
            $"https://fake.blob/{carpeta}/{Guid.NewGuid()}.jpg"
        );
    }
}