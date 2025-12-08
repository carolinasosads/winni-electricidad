using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Reseñas;

public class ReseñaACrearDto
{
    [Required(ErrorMessage = "Debes colocar una descripción a la reserva.")]
    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "Debes colocar una calificación a la reserva entre 1 y 5.")]
    [Range(1, 5, ErrorMessage = "La calificación de la reseña no es válida.")]
    public int Calificacion { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un servicio.")]
    public int IdServicio { get; set; }
}