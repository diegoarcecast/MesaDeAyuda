using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INAMU.MesaAyuda.UI
{
    public class Enumerados
    {
//        2	Jefatura de Unidad Informatica	Jefatura de Unidad Informatica	1
//8	Coordinación de Unidad Informática	Administración de Servidores	3
//11	Administración de Servidores y Servicios de TI	Administración de Servidores	5
//12	Desarrollo de Sistemas .NET	Desarrollo de Sistemas 	5
//15	Administración de Redes e Infraestructura	Administración de Redes e Infraestructura	7
//17	Soporte Técnico y Atención a Persona Usuaria	Soporte Técnico y Atención a Persona Usuaria	9
//18	Soporte Técnico y Atención a Sistema SARI	Soporte Técnico y Atención a Sistema SARI	9
//19	Desarrollo de Sistemas Sharepoint	Desarrollo de Sistemas Sharepoint	5
//21	Administración de Incidencias	Administración de Incidencias	3
//23	Persona usuaria	Persona usuaria del sistema	11
        public enum Roles
        {
            JefaturaUnidadInformatica = 2,
            CoordinacionTi = 8,
            Servidores = 11,
            Desarrollo = 12,
            Redes = 15,
            Soporte = 17,
            SoporteSari = 18,
            Sharepoint = 19,
            AdminIncidencias = 21,
            PersonaUsuaria = 22
        }
    }
}