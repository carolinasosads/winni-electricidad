using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Direcciones;

public record DireccionDto
{
        [Required(ErrorMessage = "La calle es obligatoria.")]
        [MaxLength(100, ErrorMessage = "La calle no puede superar los 100 caracteres.")]
        public string Calle { get; set; }
        
        [Required(ErrorMessage = "La esquina es obligatoria.")]
        [MaxLength(100, ErrorMessage = "La esquina no puede superar los 100 caracteres.")]
        public string Esquina { get; set; }
        
        [StringLength(10, ErrorMessage = "El número no puede superar los 10 caracteres.")]
        public string? Numero { get; set; }
        
        [StringLength(10, ErrorMessage = "El apartamento no puede superar los 10 caracteres.")]
        public string? Apto { get; set; }
}