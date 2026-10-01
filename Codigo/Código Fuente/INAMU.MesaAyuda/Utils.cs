using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Threading;
//using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel;
using System.Data.Entity;
using System.Web.ModelBinding;
//using GB.INAMU.Formularios.Models;
//using Microsoft.Reporting.WebForms;
//using MUJERESEMPRESARIAS_MODELO.Modelo;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace INAMU.MesaAyuda.UI
{
    public class Utils
    {
        public static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, new CodeGeneratorOptions { IndentString = "\t" });
                    var literal = writer.ToString();
                    literal = literal.Replace(string.Format("\" +{0}\t\"", Environment.NewLine), "");
                    return literal;
                }
            }
        }
        public void sendemail(string texto, List<string> Archivos, List<string> recipients, string titulo, List<Attachment> ArchivosMemoria = null)
        {
            //string rutaServidorCorreo = ConfigurationManager.AppSettings["rutaServidorCorreo"].ToString();
            //service.Url = new Uri(rutaServidorCorreo);

            //string usuarioCorreo = ConfigurationManager.AppSettings["usuarioCorreo"].ToString();
            //string passwordCorreo = ConfigurationManager.AppSettings["passwordCorreo"].ToString();
            bool resultado = true;
            String userName = ConfigurationManager.AppSettings["usuarioCorreo"].ToString();
            String password = ConfigurationManager.AppSettings["passwordCorreo"].ToString();
            MailMessage msg = new MailMessage();
            if (recipients != null)
            {
                foreach (var destinatario in recipients)
                { msg.To.Add(new MailAddress(destinatario)); }

                //añadir esta linea para pruebas
                //msg.To.Add(new MailAddress("pruebacorreojon@inamu.go.cr"));
            }
            //{ msg.To.Add(new MailAddress(userName)); }
            if (Archivos != null)
            {
                for (int i = 0; i < Archivos.Count; i++)
                {
                    var archivo = new System.Net.Mail.Attachment(Archivos[i]);
                    msg.Attachments.Add(archivo);
                    //archivo.Dispose();
                    //using (System.Net.Mail.Attachment data =
                    //    new System.Net.Mail.Attachment(Archivos[i]))
                    //{
                    //    msg.Attachments.Add(data);
                    //}
                }
            }
            if (ArchivosMemoria != null)
            {
                for (int i = 0; i < ArchivosMemoria.Count; i++)
                {
                    msg.Attachments.Add(ArchivosMemoria[i]);
                }
            }
            //msg.From = new MailAddress(userName);
            msg.From = new MailAddress(userName);
            msg.Subject = titulo;
            msg.Body = texto;
            msg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Host = ConfigurationManager.AppSettings["HostSMTP"].ToString();
            client.Credentials = new System.Net.NetworkCredential(userName, password);
            client.Port = int.Parse(ConfigurationManager.AppSettings["PuertoSMTP"].ToString());
            client.EnableSsl = true;


            try
            {
                client.Send(msg);
                client.Dispose();
            }

            catch (Exception ex)
            {
                try
                {
                    client.EnableSsl = false;
                    client.Send(msg);

                }
                catch
                {
                    client.Dispose();
                    resultado = false;
                }

            }
            finally
            {
                msg.Attachments.Dispose();
            };
        }

        public string ConvertirDataTableJSON(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }

        public string ConsultarUsuarioAd(string usuarioRed)
        {
            dynamic resultado = new ExpandoObject();
            try
            {

                DirectoryEntry myLdapConnection = new DirectoryEntry(ConfigurationManager.AppSettings.Get("rutaAD"));
                DirectorySearcher search = new DirectorySearcher(myLdapConnection) { Filter = (string.Format("(&(objectcategory=user)({0}={1}))", "sAMAccountName", usuarioRed)) };
                search.CacheResults = true;
                SearchResultCollection allResults = search.FindAll();
                if (allResults.Count > 0)
                {
                    foreach (SearchResult searchResult in allResults)
                    {

                        resultado.UserId = searchResult.Properties["sAMAccountName"][0].ToString();
                        if (searchResult.Properties["mail"].Count > 0)
                        {
                            resultado.mail = searchResult.Properties["mail"][0].ToString();
                        }
                        else
                        {
                            resultado.mail = "";
                        }
                        if (searchResult.Properties["department"].Count > 0)
                        {
                            resultado.department = searchResult.Properties["department"][0].ToString();
                        }
                        else
                        {
                            resultado.department = "";
                        }
                        if (searchResult.Properties["displayName"].Count > 0)
                        {
                            resultado.displayName = searchResult.Properties["displayName"][0].ToString();
                        }
                        else
                        {
                            resultado.displayName = "";
                        }
                        if (searchResult.Properties["givenName"].Count > 0)
                        {
                            resultado.givenName = searchResult.Properties["givenName"][0].ToString();
                        }
                        else
                        {
                            resultado.givenName = "";
                        }
                        if (searchResult.Properties["sn"].Count > 0)
                        {
                            resultado.sn = searchResult.Properties["sn"][0].ToString();
                        }
                        else
                        {
                            resultado.sn = "";
                        }
                        if (searchResult.Properties["physicalDeliveryOfficeName"].Count > 0)
                        {
                            resultado.physicalDeliveryOfficeName =
                                searchResult.Properties["physicalDeliveryOfficeName"][0].ToString();
                        }
                        else
                        {
                            resultado.physicalDeliveryOfficeName = "";
                        }
                        if (searchResult.Properties["company"].Count > 0)
                        {
                            resultado.company = searchResult.Properties["company"][0].ToString();
                        }
                        else
                        {
                            resultado.company = "";
                        }



                    }
                    return Newtonsoft.Json.JsonConvert.SerializeObject(resultado);

                }
                else
                {
                    return String.Empty;
                }

                


            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool AutenticadoAd(string usr, string pwd)
        {

            try
            {
                DirectoryEntry entry = new DirectoryEntry(ConfigurationManager.AppSettings.Get("rutaAD"), usr, pwd, AuthenticationTypes.Secure);
                object nativeObject = entry.NativeObject;

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task SendEmail(string toEmailAddress, string emailSubject, string emailMessage)
        {
            var message = new MailMessage();
            message.To.Add(toEmailAddress);

            message.Subject = emailSubject;
            message.Body = emailMessage;

            using (var smtpClient = new SmtpClient())
            {

            }
        }

        public static byte[] GenerarClave(string usuario, string clave,
        int tipoConversion, byte[] salt1, byte[] salt2)
        {
            string tmpPassword = null;

            switch (tipoConversion)
            {
                case 2: // password + lots of salt
                    tmpPassword = Convert.ToBase64String(salt1)
                     + Convert.ToBase64String(salt2)
                     + usuario.ToLower() + clave;
                    break;
                case 1: // user name as salt
                    tmpPassword = usuario.ToLower() + clave;
                    break;
                case 0: // no salt
                default:
                    tmpPassword = clave;
                    break;
            }

            //Convert the password string into an Array of bytes.
            UTF8Encoding textConverter = new UTF8Encoding();
            byte[] passBytes = textConverter.GetBytes(tmpPassword);

            //Return the encrypted bytes
            if (tipoConversion == 2)
                return new SHA384Managed().ComputeHash(passBytes);
            else
                return new MD5CryptoServiceProvider().ComputeHash(passBytes);
        }


        public static string ObtenerIP()
        {
            string VisitorsIPAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            }
            return VisitorsIPAddr;
        }



    }
}