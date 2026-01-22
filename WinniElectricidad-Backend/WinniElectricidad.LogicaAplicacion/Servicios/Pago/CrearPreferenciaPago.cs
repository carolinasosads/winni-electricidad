using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Pago;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Pago;

public class CrearPreferenciaPago:ICrearPreferenciaPago
{
    private readonly IRepositorioPago _repoPago;

    public CrearPreferenciaPago(IRepositorioPago repoPago)
    {
        _repoPago = repoPago;
    }

    public async Task<PagoPendienteDevueltoDto> Ejecutar(PagoPendienteDto pagoPendienteDto, int idUsuario, CancellationToken ct = default)
    {
        var request = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Title = "PagoTest",
                    Quantity = 1,
                    CurrencyId = "UYU",
                    UnitPrice = pagoPendienteDto.Monto
                }
            },
            
            BackUrls =  new PreferenceBackUrlsRequest
            {
                Success = "https://icy-flower-09db15f0f.3.azurestaticapps.net/success",
                Failure = "https://icy-flower-09db15f0f.3.azurestaticapps.net/failure",
                Pending = "https://icy-flower-09db15f0f.3.azurestaticapps.net/pending"
            },
            
            AutoReturn = "approved",
        };

        var client = new PreferenceClient();
        Preference preference = await client.CreateAsync(request);
        
        var pago = PagoMapper.MapearAPagoPendiente(pagoPendienteDto,idUsuario);
        await _repoPago.Add(pago, ct);

        var pagoDevuelto = PagoMapper.MapearPagoAPagoDevuelto(preference.Id, pago);
        
        return pagoDevuelto;
    }
}