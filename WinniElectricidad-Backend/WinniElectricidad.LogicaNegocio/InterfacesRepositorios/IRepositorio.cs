namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorio<T> where T : class
{
    Task<T?> Add(T obj, CancellationToken ct = default);
    Task<T?> FindById(int id, CancellationToken ct = default);
    Task Update(T obj, CancellationToken ct = default);
    Task Delete(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> FindAll(CancellationToken ct = default);
} 