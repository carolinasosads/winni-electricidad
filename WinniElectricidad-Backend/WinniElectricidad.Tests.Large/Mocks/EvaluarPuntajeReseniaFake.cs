using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;

namespace WinniElectricidad.Tests.Large.Mocks;

public class EvaluarPuntajeReseniaFake : IEvaluarPuntajeResenia
{
    public Task<int> CalcularPuntajeIa(string texto, int estrellas, CancellationToken ct = default)
    {
        return Task.FromResult(80);
    }
}