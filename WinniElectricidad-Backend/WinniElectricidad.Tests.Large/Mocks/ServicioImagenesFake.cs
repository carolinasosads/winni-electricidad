using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using WinniElectricidad.Api.Servicios;

namespace WinniElectricidad.Tests.Large.Mocks;

public class ServicioImagenesFake : IServicioImagenes
{
    public Task<string> GuardarImagenAsync(IFormFile archivo, string carpeta)
    {
        return Task.FromResult(
            $"https://fake.blob/{carpeta}/{Guid.NewGuid()}.jpg"
        );
    }

    public Task<ICollection<string>> GuardarImagenesAsync(
        ICollection<IFormFile> archivos,
        string nombreServicio,
        string carpeta)
    {
        var urls = new Collection<string>();

        if (archivos != null)
        {
            var index = 1;
            foreach (var _ in archivos)
            {
                var suffix = index == 1 ? "" : index.ToString();
                urls.Add($"https://fake.blob/{carpeta}/{nombreServicio}{suffix}.jpg");
                index++;
            }
        }

        return Task.FromResult<ICollection<string>>(urls);
    }

    public Task EliminarImagenesAsync(IEnumerable<string> urls)
    {
        return Task.CompletedTask;
    }
}