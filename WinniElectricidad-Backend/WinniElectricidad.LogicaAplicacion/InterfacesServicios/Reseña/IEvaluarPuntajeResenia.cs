namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;

public interface IEvaluarPuntajeResenia
{
    Task<int> CalcularPuntajeIa(string texto, int estrellas, CancellationToken ct = default);
}