namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorio<T> where T : class
{
    public void Add(T obj);
    public T FindById(int id);
    public void Update(T obj);
    public void Delete(int id);
    public IEnumerable<T> FindAll();
} 