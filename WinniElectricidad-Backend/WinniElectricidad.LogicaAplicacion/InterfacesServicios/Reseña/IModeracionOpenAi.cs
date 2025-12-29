namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;

public interface IModeracionOpenAi
{
    Task<bool> EsOfensiva(string texto);
}