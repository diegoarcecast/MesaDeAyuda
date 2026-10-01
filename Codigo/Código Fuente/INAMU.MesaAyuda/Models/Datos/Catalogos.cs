using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INAMU.MesaAyuda.AccesoDatos;
using System.Dynamic;

namespace INAMU.MesaAyuda.UI.Models
{
    public class Catalogos
    {
        public ArrayList RetornarUnidadesFormatoCombo()
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                var listaUnidades = new ArrayList();
                listaUnidades.AddRange(entidades.TBL_UNIDAD.Select(m =>
                    new
                    {
                        m.PK_UNIDAD,
                        DESCRIPCION = m.VC_DESCRIPCION,
                        CODIGO = m.VC_CODIGO

                    }).ToList());
                return listaUnidades;
            }
        }
        public dynamic RetornarCatalogos()
        {
            using (var modelo = new INAMU_SOPORTEEntities())
            {
                var listaCatalogos = modelo.TBL_CATALOGO.Select(n => new
                { n.VC_NOMBRECATALOGO, CATALOGO = n.TBL_VALORCATALOGO.Select(k => new { PKVALORCATALOGO = k.PK_VALORCATALOGO,VALOR1 = k.VC_VALOR1, VALOR2 = k.VC_VALOR2}).ToArray() });
                dynamic objCatalogos = new ExpandoObject();
                objCatalogos = listaCatalogos;

                return objCatalogos;
            }
        }

    }
}
