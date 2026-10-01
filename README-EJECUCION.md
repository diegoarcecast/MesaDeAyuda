# Mesa de Ayuda INAMU: compilación y publicación

La solución está preparada para **Visual Studio 2022**, **.NET Framework 4.8** e
IIS/IIS Express. El código no contiene contraseñas ni direcciones privadas de los
servidores del INAMU; esos valores se configuran en cada ambiente.

> La aplicación puede compilarse y abrirse en cualquier PC Windows compatible,
> pero para iniciar sesión y consultar tickets necesita acceso a las bases de datos
> SQL Server y al Active Directory del INAMU. Las bases de datos no forman parte
> de este repositorio.

## Requisitos

1. Windows 10/11 o Windows Server 2019 en adelante.
2. Visual Studio 2022 con la carga de trabajo **Desarrollo de ASP.NET y web**.
3. **.NET Framework 4.8 Developer Pack** y **.NET Framework 4.8 Runtime**.
4. SQL Server accesible con las bases `INAMU_MESA_AYUDA` e `INAMU_COMUN`.
5. Para IIS: habilitar **Internet Information Services**, **ASP.NET 4.8**, la
   consola de administración de IIS y autenticación según la política institucional.

## Ejecutar desde Visual Studio

1. Descargue o clone el repositorio.
2. Ejecute una vez `powershell -ExecutionPolicy Bypass -File .\Restore-Packages.ps1`.
   El repositorio incluye los paquetes `.nupkg`; este paso extrae localmente los
   DLL necesarios, incluso en una PC sin conexión a Internet.
3. Abra `Codigo/Código Fuente/INAMU.MesaAyuda.sln`. Si Visual Studio solicita
   restaurar paquetes adicionales, acepte la restauración.
4. Edite `Codigo/Código Fuente/INAMU.MesaAyuda/Web.config`:
   - `INAMU_SOPORTEEntities`: servidor y autenticación de la mesa de ayuda.
   - `INAMU_COMUNEntities`: servidor y autenticación de la base común.
   - `rutaAD`, `HostSMTP`, `usuarioCorreo` y `passwordCorreo`.
5. Establezca `INAMU.MesaAyuda.UI` como proyecto de inicio.
6. Compile con **Compilar > Recompilar solución** y ejecute con IIS Express.

De forma predeterminada se usa autenticación integrada de Windows para SQL
Server. En IIS debe autorizar en SQL Server la identidad del Application Pool, o
cambiar `Integrated Security=True` por credenciales administradas de forma segura.

## Publicar en IIS automáticamente

Abra PowerShell **como administrador** desde la raíz del repositorio:

```powershell
.\Deploy-IIS.ps1 `
  -SiteName "MesaAyudaINAMU" `
  -Port 8080 `
  -SupportSqlServer "SQL-SERVIDOR" `
  -CommonSqlServer "SQL-SERVIDOR"
```

El script encuentra MSBuild de Visual Studio, restaura NuGet, compila en Release,
publica los archivos en `C:\inetpub\MesaAyudaINAMU`, configura las conexiones SQL
y crea un Application Pool **.NET CLR v4.0 / Integrated**. La URL resultante será
`http://localhost:8080/`.

Parámetros opcionales importantes:

- `-PublishPath`: carpeta física de publicación.
- `-SupportDatabase` y `-CommonDatabase`: nombres de las bases.
- `-LdapPath`: ruta LDAP institucional.
- `-Force`: reemplaza un sitio IIS existente con el mismo nombre.

## Publicación manual

El perfil `1.pubxml` publica en `bin\Publish`, una ruta relativa y portable. En
Visual Studio seleccione **Publicar > 1 > Publicar** y copie el resultado al
directorio físico del sitio IIS. Configure un Application Pool con:

- Versión de CLR: **v4.0**.
- Modo de canalización: **Integrated**.
- Aplicaciones de 32 bits: **False**, salvo que un proveedor externo lo requiera.

Nunca confirme contraseñas reales en `Web.config`. Configure secretos solamente
en el archivo publicado y restrinja sus permisos a administradores y a la
identidad del Application Pool.
