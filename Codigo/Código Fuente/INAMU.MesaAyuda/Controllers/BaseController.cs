using INAMU.MesaAyuda.AccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace INAMU.MesaAyuda.UI.Controllers
{
    public class ControladorBase : Controller
    {
        public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
        {
            public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
            {
                return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
            }
        }
        public TBL_USUARIO RetornarUsuarioSesion()
        {
            var usuarioActual = (TBL_USUARIO)System.Web.HttpContext.Current.Session["usuarioActual"];
            if (usuarioActual == null)
            {
                FormsAuthentication.SignOut();
            }
            return usuarioActual;
        }
        public bool ComprobarPermisosAcccion(out bool estadoSesion)
        {
            try
            {
                var usuarioActual = (TBL_USUARIO)System.Web.HttpContext.Current.Session["usuarioActual"];
                if (usuarioActual == null)
                {
                    estadoSesion = false;
                    FormsAuthentication.SignOut();
                    return false;
                }
                estadoSesion = true;
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                var manejoSeguridad = new Models.Seguridad();
                return manejoSeguridad.ValidarPermisoAccion(usuarioActual.VC_NOMBRE,controllerName, actionName);
            }
            catch (Exception ex)
            {
                estadoSesion = true;
                return false;
            }
        }
    }
}