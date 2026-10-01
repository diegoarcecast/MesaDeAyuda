using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using INAMU.MesaAyuda.AccesoDatos;
using INAMU.MesaAyuda.UI.Models.Validadores;
using System.Transactions;

namespace INAMU.MesaAyuda.UI.Models
{
    public class Seguridad
    {
        public List<TBL_USUARIO> ListarUsuarios()
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                var listaUsuarios = entidades.TBL_USUARIO.ToList();
                foreach (var usuario in listaUsuarios)
                {
                    usuario.TBL_UNIDAD = usuario.TBL_UNIDAD;
                    usuario.TBL_ROLUSUARIO = usuario.TBL_ROLUSUARIO;
                    foreach (var rolusuario in usuario.TBL_ROLUSUARIO)
                    {
                        rolusuario.TBL_ROL = rolusuario.TBL_ROL;
                    }
                }
                return listaUsuarios;

            }
        }

        public List<TBL_ROL> ListarRoles()
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                var listaRoles = entidades.TBL_ROL.ToList();


                return listaRoles;

            }
        }

        public List<TBL_ACCION> ListarAcciones()
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                var listaAcciones = entidades.TBL_ACCION.ToList();

                return listaAcciones;

            }
        }

        public List<TBL_UNIDAD> ListarUnidades()
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                var listaUnidades = entidades.TBL_UNIDAD.ToList();

                return listaUnidades;

            }
        }

        public void RegistrarEventoBitacora(int idTipoEvento, int fkUsuario, string detalleEvento, string direccionIp)
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                entidades.TBL_BITACORA.Add(new TBL_BITACORA
                {
                    FK_TIPOEVENTO = idTipoEvento,
                    FK_USUARIO = fkUsuario,
                    VC_DETALLE = detalleEvento,
                    DT_FECHAEVENTO = DateTime.Now,
                    VC_DIRECCIONIP = Utils.ObtenerIP()
                });
                entidades.SaveChanges();
            }

        }

        public List<TBL_USUARIO> ConsultarUsuario(string usuario)
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                var listaUsuarios = entidades.TBL_USUARIO.Where(m => m.VC_USUARIO == usuario).ToList();
                foreach (var us in listaUsuarios)
                {
                    us.TBL_ROLUSUARIO = us.TBL_ROLUSUARIO;
                }

                return listaUsuarios.ToList();

            }
        }

        public TBL_USUARIO ConsultarUsuarioPorId(int pkUsuario)
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                var usuarioSeleccionado = entidades.TBL_USUARIO.FirstOrDefault(m => m.PK_USUARIO == pkUsuario);
                usuarioSeleccionado.TBL_ROLUSUARIO = usuarioSeleccionado.TBL_ROLUSUARIO;
                return usuarioSeleccionado;
            }
        }

        public bool ValidarClave(string clave, TBL_USUARIO usuario)
        {

            byte[] claveUsuario = Utils.GenerarClave(usuario.VC_USUARIO, clave, 2, usuario.IM_SALT1, usuario.IM_SALT2);
            try
            {
                for (int i = 0; i < claveUsuario.Length; i++)
                {
                    if (usuario.IM_CLAVE[i] != claveUsuario[i])
                        return false;
                }
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public bool ValidarPermisoAccion(string usuario, string controlador, string accion)
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                var usuarioObj = entidades.TBL_USUARIO.FirstOrDefault(m => m.VC_USUARIO == usuario);
                usuarioObj.TBL_ROLUSUARIO = usuarioObj.TBL_ROLUSUARIO;
                foreach (var asignacionRol in usuarioObj.TBL_ROLUSUARIO)
                {
                    asignacionRol.TBL_ROL = asignacionRol.TBL_ROL;
                    var listaAccionesRol = entidades.TBL_ROLACCION.Where(u => u.FK_ROL == asignacionRol.TBL_ROL.PK_ROL);
                    var listaAccionesCoinciden =
                        listaAccionesRol.Where(
                            i => i.TBL_ACCION.VC_CONTROLADOR == controlador && i.TBL_ACCION.VC_ACCION == accion);
                    if (listaAccionesCoinciden.Any())
                        return true;
                }


            }
            return false;
        }






        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////jose angel
        /// </summary>
        /// <param name="pkUsuario"></param>
        /// <returns></returns>
        [Authorize]
        public bool ConsultarExistenciaUsarioPorId(int pkUsuario)
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                if (pkUsuario == 0)
                {
                    return false;
                }
                else
                {
                    var listaResultados = entidades.TBL_USUARIO.Where(m => m.PK_USUARIO == pkUsuario);
                    return listaResultados.Any();
                }
            }
        }

        [Authorize]
        public Mensaje ActualizarUsuario(int pkUsuario, Usuario objSeguridadUsuarios,
            string usuarioRegistra)
        {
            try
            {
                using (var entidades = new INAMU_SOPORTEEntities())
                {
                    using (var scope = new TransactionScope())
                    {

                        var usuarioRegistrar = entidades.TBL_USUARIO.FirstOrDefault(m => m.PK_USUARIO == pkUsuario);



                        if (usuarioRegistrar != null)
                        {
                            usuarioRegistrar.VC_USUARIO = objSeguridadUsuarios.NombreUsuario;
                            usuarioRegistrar.DT_FECHAREGISTRO = objSeguridadUsuarios.FechaRegistro;
                            usuarioRegistrar.FK_UNIDAD = objSeguridadUsuarios.FkUnidad;
                            usuarioRegistrar.VC_NOMBRE = objSeguridadUsuarios.Nombre;
                            usuarioRegistrar.VC_APELLIDO1 = objSeguridadUsuarios.PrimerApellido;
                            usuarioRegistrar.VC_APELLIDO2 = objSeguridadUsuarios.SegundoApellido;
                            var salt1 = Guid.NewGuid().ToByteArray();
                            var salt2 = Guid.NewGuid().ToByteArray();
                            var clave = Utils.GenerarClave(usuarioRegistrar.VC_USUARIO, "sipamu2016", 2, salt1, salt2);
                            usuarioRegistrar.IM_CLAVE = clave;
                            usuarioRegistrar.IM_SALT1 = salt1;
                            usuarioRegistrar.IM_SALT2 = salt2;

                            entidades.SaveChanges();
                            scope.Complete();
                            return new Mensaje((int)Mensaje.CatTipoMensaje.Exitoso,
                                "Seguridad Usuario ingresado correctamente",
                                usuarioRegistrar.PK_USUARIO.ToString());
                        }
                        return new Mensaje((int)Mensaje.CatTipoMensaje.Error, "El usuario ya fué registrado", usuarioRegistrar.PK_USUARIO.ToString());
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                return new Mensaje((int)Mensaje.CatTipoMensaje.Error,
                    "Fallo al ingresar, contacte al administrador del sistema", ex.Message);
            }

            return null; //revisar 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objUsuarios"></param>
        /// <param name="usuarioRegistra"></param>
        /// <returns></returns>
        [Authorize]
        public Mensaje RegistrarUsuario(Models.Validadores.Usuario objUsuarios, bool externo)
        {
            try
            {
                using (var entidades = new INAMU_SOPORTEEntities())
                {
                    using (var scope = new TransactionScope())
                    {

                        var objUsuario = new TBL_USUARIO()
                        {
                            VC_USUARIO = objUsuarios.NombreUsuario,
                            DT_FECHAREGISTRO = DateTime.Now,
                            FK_UNIDAD = objUsuarios.FkUnidad,
                            VC_NOMBRE = objUsuarios.Nombre,
                            VC_APELLIDO1 = objUsuarios.PrimerApellido,
                            VC_APELLIDO2 = objUsuarios.SegundoApellido,
                            B_EXTERNO = false


                        };
                        if (externo)
                        {
                            objUsuario.B_EXTERNO = true;
                        }
                        var salt1 = Guid.NewGuid().ToByteArray();
                        var salt2 = Guid.NewGuid().ToByteArray();
                        var clave = Utils.GenerarClave(objUsuario.VC_USUARIO, objUsuarios.MiClave, 2, salt1, salt2);
                        objUsuario.IM_SALT1 = salt1;
                        objUsuario.IM_SALT2 = salt2;
                        objUsuario.IM_CLAVE = clave;
                        objUsuario.TBL_ROLUSUARIO.Add(new TBL_ROLUSUARIO() { FK_ROL = (int)Enumerados.Roles.PersonaUsuaria, });    
                        entidades.TBL_USUARIO.Add(objUsuario);
                        
                        entidades.SaveChanges();


                        scope.Complete();
                        return new Mensaje((int)Mensaje.CatTipoMensaje.Exitoso, "Usuario ingresado correctamente", objUsuario.PK_USUARIO.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return new Mensaje((int)Mensaje.CatTipoMensaje.Error,
                    "Fallo al registrar nueva persona usuaria, contacte al administrador del sistema", ex.Message);
            }
        }

        [Authorize]
        public Array ConsultarGridUsuarios(string draw, string start, string length, string sortColumn,
            string sortColumnDir, int pageSize, int skip, string terminoBusqueda, ref int recordsTotal,
            int idTipoInstrumento)
        {
            var arregloResultado = new ArrayList();
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                switch (idTipoInstrumento)
                {
                    #region Seguridad Usuario

                    case 1:
                        var listarNombreRoles = entidades.TBL_ROL.Where(j => j.VC_NOMBRE != null).Select(
                            r => new
                            {
                                NOMBREROL = r.VC_NOMBRE
                            }).AsEnumerable().ToList();

                        if (string.IsNullOrEmpty(terminoBusqueda))
                        {
                            var listaResultados = entidades.TBL_USUARIO.Where
                                (n => n.VC_USUARIO != null).Select(
                                    m => new
                                    {
                                        PKUSUARIO = m.VC_USUARIO,
                                        UNIDAD = m.TBL_UNIDAD.VC_DESCRIPCION,
                                        NOMBRE = m.VC_NOMBRE.Trim(),
                                        APELLIDO1 = m.VC_APELLIDO1.Trim(),
                                        APELLIDO2 = m.VC_APELLIDO2.Trim(),

                                        ROL = listarNombreRoles.Exists(m.PK_USUARIO.Equals)

                                    }).AsEnumerable().ToList();



                            switch (sortColumn)
                            {
                                case "PKUSUARIO":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.PKUSUARIO).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.PKUSUARIO).AsEnumerable().ToList();
                                    break;
                                case "UNIDAD":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.UNIDAD).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.UNIDAD).AsEnumerable().ToList();
                                    break;
                                case "NOMBRE":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.NOMBRE).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.NOMBRE).AsEnumerable().ToList();
                                    break;
                                case "APELLIDO1":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.APELLIDO1).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.APELLIDO1).AsEnumerable().ToList();
                                    break;
                                case "APELLIDO2":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.APELLIDO2).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.APELLIDO2).AsEnumerable().ToList();
                                    break;
                                case "ROL":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.ROL).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.ROL).AsEnumerable().ToList();
                                    if (sortColumnDir.Trim().ToUpper() == "ASC")
                                    {
                                        listaResultados =
                                            listaResultados.OrderBy(m => m.ROL).AsEnumerable().ToList();
                                    }
                                    else
                                    {
                                        listaResultados =
                                            listaResultados.OrderByDescending(m => m.ROL).AsEnumerable().ToList();
                                    }
                                    break;
                            }
                            recordsTotal = listaResultados.Count();
                            var listaFiltrada = listaResultados.Skip(skip).Take(pageSize).ToArray();
                            return listaFiltrada;
                        }
                        else
                        {
                            var listaResultados = entidades.TBL_USUARIO.Where
                                (n => n.VC_USUARIO != null).Select
                                (m => new
                                {
                                    PKUSUARIO = m.PK_USUARIO,
                                    UNIDAD = m.TBL_UNIDAD.VC_DESCRIPCION,
                                    NOMBRE = m.VC_NOMBRE.Trim(),
                                    APELLIDO1 = m.VC_APELLIDO1.Trim(),
                                    APELLIDO2 = m.VC_APELLIDO2.Trim(),
                                    ROL = listarNombreRoles.Exists(m.PK_USUARIO.Equals)
                                }).Where(x => x.UNIDAD.Trim() == terminoBusqueda.Trim() ||
                                              x.ROL.ToString() == terminoBusqueda.Trim() ||
                                              x.APELLIDO1.Trim() == terminoBusqueda.Trim() ||
                                              x.APELLIDO2.Trim() == terminoBusqueda.Trim() ||
                                              x.NOMBRE.ToUpper().Contains(terminoBusqueda.Trim().ToUpper()))


                                .AsEnumerable()
                                .ToList();
                            recordsTotal = listaResultados.Count();
                            switch (sortColumn)
                            {
                                case "PKUSUARIO":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.PKUSUARIO).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.PKUSUARIO).AsEnumerable().ToList();
                                    break;
                                case "UNIDAD":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.UNIDAD).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.UNIDAD).AsEnumerable().ToList();
                                    break;
                                case "NOMBRE":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.NOMBRE).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.NOMBRE).AsEnumerable().ToList();
                                    break;
                                case "APELLIDO1":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.APELLIDO1).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.APELLIDO1).AsEnumerable().ToList();
                                    break;
                                case "APELLIDO2":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.APELLIDO2).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.APELLIDO2).AsEnumerable().ToList();
                                    break;
                                case "ROL":
                                    listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
                                        ? listaResultados.OrderBy(m => m.ROL).AsEnumerable().ToList()
                                        : listaResultados.OrderByDescending(m => m.ROL).AsEnumerable().ToList();
                                    break;
                            }
                            var listaFiltrada = listaResultados.Skip(skip).Take(pageSize).ToArray();
                            return listaFiltrada;
                        }

                        #endregion

                }


            }
            return null;//revisar
        }

        [Authorize]
        public TBL_ROL ConsultarRolPorId(int pkRol)
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                var rolSeleccionado = entidades.TBL_ROL.FirstOrDefault(m => m.PK_ROL == pkRol);

                return rolSeleccionado;
            }
        }

        [Authorize]
        public bool ConsultarExistenciaRolPorId(int pkRol)
        {
            using (var entidades = new INAMU_SOPORTEEntities())
            {
                if (pkRol == 0)
                {
                    return false;
                }
                else
                {
                    var listaResultados = entidades.TBL_ROL.Where(m => m.PK_ROL == pkRol);
                    return listaResultados.Any();
                }
            }
        }


        //[Authorize]
        //public Mensaje RegistrarRoles(int pkFicha, Roles objrRoles,
        //    string usuarioRegistra)
        //{
        //    return null;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="objRoles"></param>
        ///// <param name="usuarioRegistra"></param>
        ///// <returns></returns>
        //[Authorize]
        //public Mensaje RegistrarAgregarRol(Models.Validadores.Roles objRoles, string usuarioRegistra)
        //{
        //    try
        //    {
        //        using (var entidades = new INAMU_SOPORTEEntities())
        //        {
        //            using (var scope = new TransactionScope())
        //            {
        //                var objRol = new TBLROL
        //                {
        //                    NOMBRE = objRoles.Nombre,
        //                    DESCRIPCION = objRoles.Descripcion
        //                };
        //                entidades.TBLROL.Add(objRol);
        //                entidades.SaveChanges();
        //                var ultimoRolIngresado = entidades.TBLROL.OrderByDescending(u => u.PKROL).FirstOrDefault();
        //                int valorId = ultimoRolIngresado.PKROL;

        //                var objRolAccion = new TBLROLACCION();

        //                foreach (var numAccion in objRoles.ChkAccion)
        //                {
        //                    objRolAccion.FKROL = valorId;
        //                    objRolAccion.FKACCION = Convert.ToInt32(numAccion);
        //                    entidades.TBLROLACCION.Add(objRolAccion);
        //                    entidades.SaveChanges();
        //                }
        //                scope.Complete();
        //                return new Mensaje((int)Mensaje.CatTipoMensaje.Exitoso, "Rol ingresado correctamente", objRol.PKROL.ToString());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Mensaje((int)Mensaje.CatTipoMensaje.Error,
        //            "Fallo al ingresar, contacte al administrador del sistema", ex.Message);
        //    }
        //}

        [Authorize]
        public Array ConsultaGridRoles(string draw, string start, string length, string sortColumn,
            string sortColumnDir, int pageSize, int skip, string terminoBusqueda, ref int recordsTotal,
            int idTipoInstrumento)
        {
            //var arregloResultado = new ArrayList();
            //using (var entidades = new SIPAMU_Entities())
            //{
            //    switch (idTipoInstrumento)
            //    {


            //        #region InstrumentoB

            //        case 2:
            //            if (string.IsNullOrEmpty(terminoBusqueda))
            //            {
            //                var listaResultados = entidades.TBLFICHA.Where
            //                    (n => n.INSTRUMENTOA && !n.INSTRUMENTOPLANVIDA && !n.INSTRUMENTOB && n.TBLGRUPO.ANNO == DateTime.Now.Year).Select
            //                    (m => new
            //                    {
            //                        PKFICHA = m.PKFICHA,
            //                        NOMBRE = m.NOMBRE.Trim() + " " + m.APELLIDO1.Trim() + " " + m.APELLIDO2.Trim(),
            //                        IDENTIFICACION = m.NUMEROIDENTIFICACION,
            //                        ANNOREGISTRO = m.FECHAREGISTRO.Year,
            //                        GRUPO = m.TBLGRUPO.NOMBRE + " - " + m.TBLGRUPO.HORARIO,
            //                        ENCARGADA = (m.TBLGRUPO.TBLUSUARIO.NOMBRE.Trim() + " " +
            //                                     m.TBLGRUPO.TBLUSUARIO.APELLIDO1.Trim() + " "
            //                                     + m.TBLGRUPO.TBLUSUARIO.APELLIDO2.Trim()).Trim()
            //                    }).AsEnumerable().ToList();
            //                switch (sortColumn)
            //                {
            //                    case "PKFICHA":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.PKFICHA).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.PKFICHA).AsEnumerable().ToList();
            //                        break;
            //                    case "NOMBRE":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.NOMBRE).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.NOMBRE).AsEnumerable().ToList();
            //                        break;
            //                    case "IDENTIFICACION":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.IDENTIFICACION).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.IDENTIFICACION).AsEnumerable().ToList();
            //                        break;
            //                    case "ANNOREGISTRO":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.ANNOREGISTRO).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.ANNOREGISTRO).AsEnumerable().ToList();
            //                        break;
            //                    case "GRUPO":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.GRUPO).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.GRUPO).AsEnumerable().ToList();
            //                        break;
            //                    case "ENCARGADA":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.ENCARGADA).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.ENCARGADA).AsEnumerable().ToList();
            //                        if (sortColumnDir.Trim().ToUpper() == "ASC")
            //                        {
            //                            listaResultados =
            //                                listaResultados.OrderBy(m => m.ENCARGADA).AsEnumerable().ToList();
            //                        }
            //                        else
            //                        {
            //                            listaResultados =
            //                                listaResultados.OrderByDescending(m => m.ENCARGADA).AsEnumerable().ToList();
            //                        }
            //                        break;
            //                }
            //                recordsTotal = listaResultados.Count();
            //                var listaFiltrada = listaResultados.Skip(skip).Take(pageSize).ToArray();
            //                return listaFiltrada;
            //            }
            //            else
            //            {
            //                var listaResultados = entidades.TBLFICHA.Where
            //                    (n => n.INSTRUMENTOA && !n.INSTRUMENTOPLANVIDA && !n.INSTRUMENTOB && n.TBLGRUPO.ANNO == DateTime.Now.Year).Select
            //                    (m => new
            //                    {
            //                        PKFICHA = m.PKFICHA,
            //                        NOMBRE = m.NOMBRE.Trim() + " " + m.APELLIDO1.Trim() + " " + m.APELLIDO2.Trim(),
            //                        IDENTIFICACION = m.NUMEROIDENTIFICACION,
            //                        ANNOREGISTRO = m.FECHAREGISTRO.Year,
            //                        GRUPO = m.TBLGRUPO.NOMBRE + " - " + m.TBLGRUPO.HORARIO,
            //                        ENCARGADA = (m.TBLGRUPO.TBLUSUARIO.NOMBRE.Trim() + " " +
            //                                     m.TBLGRUPO.TBLUSUARIO.APELLIDO1.Trim() + " "
            //                                     + m.TBLGRUPO.TBLUSUARIO.APELLIDO2.Trim()).Trim()
            //                    }).Where(x => x.IDENTIFICACION.Trim() == terminoBusqueda.Trim() ||
            //                                  x.GRUPO.Contains(terminoBusqueda.Trim()) ||
            //                                  x.ENCARGADA.Contains(terminoBusqueda.Trim()) ||
            //                                  x.NOMBRE.ToUpper().Contains(terminoBusqueda.Trim().ToUpper()))
            //                    .AsEnumerable()
            //                    .ToList();
            //                recordsTotal = listaResultados.Count();
            //                switch (sortColumn)
            //                {
            //                    case "PKFICHA":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.PKFICHA).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.PKFICHA).AsEnumerable().ToList();
            //                        break;
            //                    case "NOMBRE":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.NOMBRE).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.NOMBRE).AsEnumerable().ToList();
            //                        break;
            //                    case "IDENTIFICACION":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.IDENTIFICACION).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.IDENTIFICACION).AsEnumerable().ToList();
            //                        break;
            //                    case "ANNOREGISTRO":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.ANNOREGISTRO).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.ANNOREGISTRO).AsEnumerable().ToList();
            //                        break;
            //                    case "GRUPO":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.GRUPO).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.GRUPO).AsEnumerable().ToList();
            //                        break;
            //                    case "ENCARGADA":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.ENCARGADA).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.ENCARGADA).AsEnumerable().ToList();
            //                        break;
            //                }
            //                var listaFiltrada = listaResultados.Skip(skip).Take(pageSize).ToArray();
            //                return listaFiltrada;
            //            }
            //            break;

            //        #endregion



            //        #region PorDefecto

            //        default:
            //            if (string.IsNullOrEmpty(terminoBusqueda))
            //            {
            //                var listaResultados = entidades.TBLFICHA.Where(n => n.TBLGRUPO.ANNO == DateTime.Now.Year).Select
            //                    (m => new
            //                    {
            //                        PKFICHA = m.PKFICHA,
            //                        NOMBRE = m.NOMBRE.Trim() + " " + m.APELLIDO1.Trim() + " " + m.APELLIDO2.Trim(),
            //                        IDENTIFICACION = m.NUMEROIDENTIFICACION,
            //                        ANNOREGISTRO = m.FECHAREGISTRO.Year,
            //                        GRUPO = m.TBLGRUPO.NOMBRE + " - " + m.TBLGRUPO.HORARIO,
            //                        ENCARGADA = (m.TBLGRUPO.TBLUSUARIO.NOMBRE.Trim() + " " +
            //                                     m.TBLGRUPO.TBLUSUARIO.APELLIDO1.Trim() + " "
            //                                     + m.TBLGRUPO.TBLUSUARIO.APELLIDO2.Trim()).Trim()
            //                    }).AsEnumerable().ToList();
            //                switch (sortColumn)
            //                {
            //                    case "PKFICHA":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.PKFICHA).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.PKFICHA).AsEnumerable().ToList();
            //                        break;
            //                    case "NOMBRE":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.NOMBRE).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.NOMBRE).AsEnumerable().ToList();
            //                        break;
            //                    case "IDENTIFICACION":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.IDENTIFICACION).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.IDENTIFICACION).AsEnumerable().ToList();
            //                        break;
            //                    case "ANNOREGISTRO":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.ANNOREGISTRO).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.ANNOREGISTRO).AsEnumerable().ToList();
            //                        break;
            //                    case "GRUPO":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.GRUPO).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.GRUPO).AsEnumerable().ToList();
            //                        break;
            //                    case "ENCARGADA":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.ENCARGADA).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.ENCARGADA).AsEnumerable().ToList();
            //                        break;
            //                }
            //                recordsTotal = listaResultados.Count();
            //                var listaFiltrada = listaResultados.Skip(skip).Take(pageSize).ToArray();
            //                return listaFiltrada;
            //            }
            //            else
            //            {
            //                var listaResultados = entidades.TBLFICHA.Where(n => n.TBLGRUPO.ANNO == DateTime.Now.Year).Select
            //                    (m => new
            //                    {
            //                        PKFICHA = m.PKFICHA,
            //                        NOMBRE = m.NOMBRE.Trim() + " " + m.APELLIDO1.Trim() + " " + m.APELLIDO2.Trim(),
            //                        IDENTIFICACION = m.NUMEROIDENTIFICACION,
            //                        ANNOREGISTRO = m.FECHAREGISTRO.Year,
            //                        GRUPO = m.TBLGRUPO.NOMBRE + " - " + m.TBLGRUPO.HORARIO,
            //                        ENCARGADA = (m.TBLGRUPO.TBLUSUARIO.NOMBRE.Trim() + " " +
            //                                     m.TBLGRUPO.TBLUSUARIO.APELLIDO1.Trim() + " "
            //                                     + m.TBLGRUPO.TBLUSUARIO.APELLIDO2.Trim()).Trim()
            //                    }).Where(m => m.IDENTIFICACION.Trim() == terminoBusqueda.Trim() ||
            //                                  m.GRUPO.Contains(terminoBusqueda.Trim()) ||
            //                                  m.ENCARGADA.Contains(terminoBusqueda.Trim()) ||
            //                                  m.NOMBRE.ToUpper().Contains(terminoBusqueda.Trim().ToUpper()))
            //                    .AsEnumerable()
            //                    .ToList();
            //                recordsTotal = listaResultados.Count();
            //                switch (sortColumn)
            //                {
            //                    case "PKFICHA":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.PKFICHA).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.PKFICHA).AsEnumerable().ToList();
            //                        break;
            //                    case "NOMBRE":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.NOMBRE).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.NOMBRE).AsEnumerable().ToList();
            //                        break;
            //                    case "IDENTIFICACION":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.IDENTIFICACION).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.IDENTIFICACION).AsEnumerable().ToList();
            //                        break;
            //                    case "ANNOREGISTRO":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.ANNOREGISTRO).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.ANNOREGISTRO).AsEnumerable().ToList();
            //                        break;
            //                    case "GRUPO":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.GRUPO).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.GRUPO).AsEnumerable().ToList();
            //                        break;
            //                    case "ENCARGADA":
            //                        listaResultados = sortColumnDir.Trim().ToUpper() == "ASC"
            //                            ? listaResultados.OrderBy(m => m.ENCARGADA).AsEnumerable().ToList()
            //                            : listaResultados.OrderByDescending(m => m.ENCARGADA).AsEnumerable().ToList();
            //                        break;
            //                }

            //        #endregion

            //                var listaFiltrada = listaResultados.Skip(skip).Take(pageSize).ToArray();
            //                return listaFiltrada;
            //            }
            //            break;
            //    }
            //}
            return null;
        }
    
}
}