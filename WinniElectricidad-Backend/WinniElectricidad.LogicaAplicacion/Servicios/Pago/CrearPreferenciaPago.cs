using MercadoPago.Client.Preference;
using MercadoPago.Error;
using MercadoPago.Resource.Preference;
using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Pago;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Pago;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Pago;

public class CrearPreferenciaPago : ICrearPreferenciaPago
{
    private readonly IRepositorioPago _repoPago;

    public CrearPreferenciaPago(IRepositorioPago repoPago)
    {
        _repoPago = repoPago;
    }

    public async Task<PagoPendienteDevueltoDto> Ejecutar(PagoPendienteDto pagoPendienteDto, int idUsuario, CancellationToken ct = default)
    {
        var montoPagado = pagoPendienteDto.MontoPagado ?? 0m;
        if (pagoPendienteDto.Monto > pagoPendienteDto.MontoTotal - montoPagado)
            throw new PagoException("El monto ingresado supera el monto restante a pagar.");

        if (pagoPendienteDto.MontoTotal - montoPagado == 0)
            throw new PagoException("El presupuesto seleccionado ya fue pagado por completo.");

        var servicios = pagoPendienteDto.NombresServicios switch
        {
            null or { Count: 0 } => "Servicios varios",
            { Count: 1 } => pagoPendienteDto.NombresServicios[0],
            _ => string.Join(", ", pagoPendienteDto.NombresServicios)
        };

        var pago = PagoMapper.MapearAPagoPendiente(pagoPendienteDto, idUsuario);
        await _repoPago.Add(pago, ct);
        var request = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Title = "Pago de presupuesto de " + servicios + " - Winni Electricidad",
                    Quantity = 1,
                    CurrencyId = "UYU",
                    UnitPrice = pagoPendienteDto.Monto
                }
            },
            
            BackUrls =  new PreferenceBackUrlsRequest
            {
                Success = "https://icy-flower-09db15f0f.3.azurestaticapps.net/cliente/success",
                Failure = "https://icy-flower-09db15f0f.3.azurestaticapps.net/cliente/failure",
                Pending = "https://icy-flower-09db15f0f.3.azurestaticapps.net/cliente/pending"
            },
            
            AutoReturn = "approved",
            ExternalReference = pago.IdPago.ToString(),
            NotificationUrl = "https://winnielectricidadbe-dev-adgqcbd7gvbgg7fy.eastus2-01.azurewebsites.net/WinniElectricidadApi/mercadopago/webhook"
        };

        try
        {
            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request, null, ct);

            return PagoMapper.MapearPagoAPagoDevuelto(preference.Id, pago);
        }
        catch (MercadoPagoApiException ex)
        {
            Console.WriteLine($"Error de MP: {ex.ApiError.Message}");
            throw;
        }
    }
}