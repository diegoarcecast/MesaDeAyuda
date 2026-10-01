using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace INAMU.MesaAyuda.UI.Models.Validadores
{
    public class Usuario
    {
        [Required]
        public int IdUsuario { get; set; }

        [MaxLength(500)]
        public string NombreUsuario { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public int FkUnidad { get; set; }

        [MaxLength(50)]
        public string Nombre { get; set; }

        [MaxLength(100)]
        public string PrimerApellido { get; set; }

        [MaxLength(100)]
        public string SegundoApellido { get; set; }

        [MaxLength(50)]
        public string Correo { get; set; }

        [MaxLength(1000)]
        public string MiClave { get; set; }

        [MaxLength(100)]
        public byte[] ImSalt1 { get; set; }

        [MaxLength(100)]
        public byte[] ImSalt2 { get; set; }

        [MaxLength(200)]
        public string Roles { get; set; }
    }
}