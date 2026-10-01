using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INAMU.MesaAyuda.UI.Models
{
    public class Mensaje
    {
        public Mensaje(int tipoMensaje, string contenidoMensaje, string valorRetorno)
        {
            this.TipoMensaje = tipoMensaje;
            this.ContenidoMensaje = contenidoMensaje;
            this.ValorRetorno = valorRetorno;
        }

        public int TipoMensaje { get; set; }
        public string ContenidoMensaje { get; set; }
        public string ValorRetorno { get; set; }
        public enum CatTipoMensaje
        {
            Exitoso = 1,
            Error = 2,
            Informacion = 3,
            Advertencia = 4
        }
    }
}