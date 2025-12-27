using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;

namespace WinniElectricidad.Tests.Large.Mocks;

public class ModeracionOpenAiFake : IModeracionOpenAi
{
    public Task<bool> EsOfensiva(string texto)
    {
        return Task.FromResult(false);
    }
}