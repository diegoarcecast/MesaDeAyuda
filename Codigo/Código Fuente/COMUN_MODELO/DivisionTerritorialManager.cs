using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMUN_MODELO
{
    public class DivisionTerritorialManager
    {
        /// <summary>
        /// Método de listar provincias
        /// </summary>
        /// <returns></returns>
        public List<CAT_PROVINCIA> listarProvincias()
        {
            List<CAT_PROVINCIA> listaResultado = new List<CAT_PROVINCIA>();

            try
            {
                using (INAMU_COMUNEntities context = new INAMU_COMUNEntities())
                {
                    List<CAT_PROVINCIA> lista = context.CAT_PROVINCIA.ToList();
                    listaResultado = lista;
                }
                return listaResultado;
            }
            catch (Exception ex)
            {
                //Aqui va la bitacora
            }
            return listaResultado;
        }

        /// <summary>
        /// Trae el objeto de una provincia a partir del Id
        /// </summary>
        /// <param name="idElemento"></param>
        /// <returns></returns>
        public CAT_PROVINCIA obtenerProvincia(int idElemento)
        {
            CAT_PROVINCIA elemento = new CAT_PROVINCIA();
            try
            {
                using (INAMU_COMUNEntities context = new INAMU_COMUNEntities())
                {
                    elemento = context.CAT_PROVINCIA.Where(w => w.I_IDPROVINCIA == idElemento).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return elemento;
        }

        /// <summary>
        /// Método de listar cantones por provicia
        /// </summary>
        /// <returns></returns>
        public List<CAT_CANTON> listarCantones(int idProvincia)
        {
            List<CAT_CANTON> listaResultado = new List<CAT_CANTON>();

            try
            {
                using (INAMU_COMUNEntities context = new INAMU_COMUNEntities())
                {
                    List<CAT_CANTON> lista = context.CAT_CANTON.OrderBy(c=>c.VC_DESCRIPCION).Where(c => c.I_IDPROVINCIA == idProvincia).ToList();
                    listaResultado = lista;
                }
                return listaResultado;
            }
            catch (Exception ex)
            {
                //Aqui va la bitacora
            }
            return listaResultado;
        }

        /// <summary>
        /// Trae el objeto de un canton a partir de su id y la provincia a la que pertenece
        /// </summary>
        /// <param name="idElemento"></param>
        /// <returns></returns>
        public CAT_CANTON obtenerCanton(int idProvincia, int idCanton)
        {
            CAT_CANTON elemento = new CAT_CANTON();
            try
            {
                using (INAMU_COMUNEntities context = new INAMU_COMUNEntities())
                {
                    elemento = context.CAT_CANTON.Where(w => w.I_IDPROVINCIA == idProvincia && w.I_IDCANTON == idCanton).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return elemento;
        }

        /// <summary>
        /// Método de listar distritos por canton
        /// /summary>
        /// <returns></returns>
        public List<CAT_DISTRITO> listarDistritos(int idCanton, int idProvincia)
        {
            //int result = 0;
            List<CAT_DISTRITO> listaResultado = new List<CAT_DISTRITO>();

            try
            {
                using (INAMU_COMUNEntities context = new INAMU_COMUNEntities())
                {
                    List<CAT_DISTRITO> lista = context.CAT_DISTRITO.OrderBy(d=>d.VC_DESCRIPCION).Where(c => c.I_IDCANTON == idCanton && c.I_IDPROVINCIA == idProvincia).ToList();
                    listaResultado = lista;
                }
                return listaResultado;
            }
            catch (Exception ex)
            {
                //Aqui va la bitacora
            }
            return listaResultado;
        }

        /// <summary>
        /// Trae un objeto a partir del canton al que pertenece
        /// </summary>
        /// <param name="idDistrito"></param>
        /// <param name="idCanton"></param>
        /// <param name="idProv"></param>
        /// <returns></returns>
        public CAT_DISTRITO obtenerDistrito(int idDistrito, int idCanton, int idProv)
        {
            using (INAMU_COMUNEntities context = new INAMU_COMUNEntities())
            {
                CAT_DISTRITO distrito = context.CAT_DISTRITO.FirstOrDefault(p => p.I_IDDISTRITO == idDistrito && p.I_IDPROVINCIA == idProv && p.I_IDCANTON == idCanton);
                return distrito;
            }
        }

    }
}
