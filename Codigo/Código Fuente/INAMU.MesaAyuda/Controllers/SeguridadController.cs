using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using INAMU.MesaAyuda.UI.Models;

namespace INAMU.MesaAyuda.UI.Controllers
{
    public class SeguridadController : Controller
    {
        readonly Models.Seguridad _modelSeguridad = new Models.Seguridad();
        readonly Utils _utilitario = new Utils();
        [AllowAnonymous]
        public bool RegistrarEventoBitacora(int idTipoEvento, int fkUsuario, string contenido = "")
        {
            try
            {
                _modelSeguridad.RegistrarEventoBitacora(idTipoEvento, fkUsuario, contenido + HttpContext.Request.UserAgent.ToString() + " - " + HttpContext.Request.RawUrl, HttpContext.Request.UserHostAddress);
            }
            catch (Exception)
            {

                _modelSeguridad.RegistrarEventoBitacora(idTipoEvento, fkUsuario, HttpContext.Request.Browser.Browser + " - " + HttpContext.Request.RawUrl, "NA");
            }

            return true;
        }
        [AllowAnonymous]
        public JsonResult ComprobarUsuarioAd(string usuarioAd)
        {
            usuarioAd = usuarioAd.Replace("@inamu.go.cr", string.Empty);
            var utilitario = new Utils();
            return Json(utilitario.ConsultarUsuarioAd(usuarioAd), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult RegistrarUsuario(Models.Validadores.Usuario usuario)
        {
            if (!usuario.NombreUsuario.Contains("@inamu.go.cr"))
            {
                return Json(new Mensaje((int)Mensaje.CatTipoMensaje.Error, "Para proceder con el registro debe suministrar un correo de la institución", string.Empty));
            }
            else
            {
                if (_utilitario.AutenticadoAd(usuario.NombreUsuario, usuario.MiClave))
                {
                    if (_modelSeguridad.ConsultarUsuario(usuario.NombreUsuario).Any())
                    {
                        return
                            Json(new Mensaje((int) Mensaje.CatTipoMensaje.Error, "El usuario ya existe en el sistema",
                                string.Empty));
                    }
                    else
                    {
                        usuario.Roles = string.Format("{0}", (int)Enumerados.Roles.PersonaUsuaria);
                        return Json(_modelSeguridad.RegistrarUsuario(usuario, false), JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new Mensaje((int)Mensaje.CatTipoMensaje.Error, "La contraseña de su usuario de red es incorrecta", string.Empty));
                }


            }
            
            
        }

        [AllowAnonymous]
        public JsonResult AutenticarUsuario(string usuario, string clave)
        {
            var directorioAd = System.Configuration.ConfigurationManager.AppSettings.Get("rutaAD");
            var utilitario = new Utils();
            try
            {
                var usuarioConsulta = _modelSeguridad.ConsultarUsuario(usuario);
                if (usuarioConsulta.Any())
                {
                    var autenticadoAd = utilitario.AutenticadoAd(usuario, clave);
                    if (autenticadoAd)
                    {
                        FormsAuthentication.SetAuthCookie(usuarioConsulta.FirstOrDefault().VC_USUARIO, false);
                        HttpContext.Session.Add("usuarioActual", usuarioConsulta.FirstOrDefault());
                        return
                            Json(
                                new Mensaje((int)Mensaje.CatTipoMensaje.Exitoso, usuarioConsulta.FirstOrDefault().PK_USUARIO.ToString(), string.Empty),
                                JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (_modelSeguridad.ValidarClave(clave, usuarioConsulta.FirstOrDefault()))
                        {
                            FormsAuthentication.SetAuthCookie(usuarioConsulta.FirstOrDefault().VC_USUARIO, false);
                            HttpContext.Session.Add("usuarioActual", usuarioConsulta.FirstOrDefault());
                            return Json(new Mensaje((int)Mensaje.CatTipoMensaje.Exitoso, usuarioConsulta.FirstOrDefault().PK_USUARIO.ToString(), string.Empty), JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new Mensaje((int)Mensaje.CatTipoMensaje.Error, "Error al validar sus credenciales", string.Empty), JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(new Mensaje((int)Mensaje.CatTipoMensaje.Error, "Error, la persona usuaria ingresada no existe", string.Empty), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new Mensaje((int)Mensaje.CatTipoMensaje.Error, "Error al validar sus credenciales", string.Empty), JsonRequestBehavior.AllowGet);
            }
        }
    }
}