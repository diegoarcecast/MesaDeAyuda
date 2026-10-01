using System;
using System.Web.Helpers;
using System.Web.Mvc;
using INAMU.MesaAyuda.UI.Models;
using INAMU.MesaAyuda.AccesoDatos;
using System.Collections.Generic;
using System.Linq;

namespace INAMU.MesaAyuda.UI.Controllers
{
    public class HomeController : ControladorBase
    {
        [Authorize]
        public ActionResult Index()
        {
            var usuarioActual = RetornarUsuarioSesion();
            if (usuarioActual != null)
            {
                if (usuarioActual.TBL_ROLUSUARIO.Any())
                {
                    if (usuarioActual.TBL_ROLUSUARIO.Any(m => m.FK_ROL != (int)Enumerados.Roles.PersonaUsuaria))
                    {
                        return View(viewName: "~/Views/Shared/Dashboard/Soporte.cshtml", model: usuarioActual);
                    }
                    else
                    {
                        return View(viewName: "~/Views/Shared/Dashboard/Funcionaria.cshtml", model: usuarioActual);
                    }
                }
                return View(viewName: "~/Views/Shared/Error/Permisos.cshtml");
            }
            return View(viewName: "~/Views/Shared/Error/Permisos.cshtml");
        }

        [AllowAnonymous]
        public ActionResult Ingreso()
        {
            return View();
        }


    }
}