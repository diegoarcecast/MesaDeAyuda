using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INAMU.MesaAyuda.UI.Controllers
{
    public class CatalogosController : Controller
    {
        Models.Catalogos _modelCatalogos = new Models.Catalogos();

        [AllowAnonymous]
        public JsonResult ListarUnidades()
        {
            try
            {
                return Json(_modelCatalogos.RetornarUnidadesFormatoCombo(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new ArrayList(), JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public JsonResult RetornarCatalogos()
        {
            try
            {
                return Json(_modelCatalogos.RetornarCatalogos(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("Error al procesar la solicitud: " + e.Message, JsonRequestBehavior.AllowGet);
            }

        }
    }
}