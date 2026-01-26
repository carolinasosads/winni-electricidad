using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Recordatorios;

public class RecordatorioDto
{
    [Required]
    public string TituloServicio { get; set; }
    [Required]
    public string Texto { get; set; }
    [Required]
    public List<string> EmailClientesParaEnviar { get; set; }
}