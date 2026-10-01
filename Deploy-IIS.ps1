[CmdletBinding()]
param(
    [string]$SiteName = "MesaAyudaINAMU",
    [string]$PublishPath = "C:\inetpub\MesaAyudaINAMU",
    [ValidateRange(1, 65535)][int]$Port = 8080,
    [Parameter(Mandatory = $true)][string]$SupportSqlServer,
    [Parameter(Mandatory = $true)][string]$CommonSqlServer,
    [string]$SupportDatabase = "INAMU_MESA_AYUDA",
    [string]$CommonDatabase = "INAMU_COMUN",
    [string]$LdapPath = "LDAP://SERVIDOR/DC=dominio,DC=local",
    [switch]$Force
)

$ErrorActionPreference = "Stop"
$solution = Join-Path $PSScriptRoot "Codigo\Código Fuente\INAMU.MesaAyuda.sln"
$webProject = Join-Path $PSScriptRoot "Codigo\Código Fuente\INAMU.MesaAyuda\INAMU.MesaAyuda.UI.csproj"

if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
    [Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw "Ejecute este script desde PowerShell como administrador."
}

$vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vswhere)) {
    throw "No se encontró Visual Studio Installer (vswhere.exe). Instale Visual Studio 2022 con la carga de trabajo ASP.NET."
}

$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
if (-not $msbuild) { throw "No se encontró MSBuild en la instalación de Visual Studio." }

Write-Host "Restaurando paquetes NuGet..." -ForegroundColor Cyan
& (Join-Path $PSScriptRoot "Restore-Packages.ps1")
if ($LASTEXITCODE -ne 0) { throw "Falló la extracción de los paquetes locales." }
& $msbuild $solution /t:Restore /p:RestorePackagesConfig=true /m
if ($LASTEXITCODE -ne 0) { throw "Falló la restauración de paquetes NuGet." }

if (Test-Path $PublishPath) { Remove-Item (Join-Path $PublishPath "*") -Recurse -Force -ErrorAction SilentlyContinue }
New-Item -ItemType Directory -Path $PublishPath -Force | Out-Null

Write-Host "Compilando y publicando la aplicación..." -ForegroundColor Cyan
& $msbuild $webProject /t:Build /p:Configuration=Release /p:DeployOnBuild=true /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles=true "/p:PublishUrl=$PublishPath" /m
if ($LASTEXITCODE -ne 0) { throw "La compilación o publicación falló. Revise la salida de MSBuild." }

$webConfigPath = Join-Path $PublishPath "Web.config"
[xml]$config = Get-Content $webConfigPath
function Set-EntityConnection([string]$name, [string]$model, [string]$server, [string]$database) {
    $entry = $config.configuration.connectionStrings.add | Where-Object { $_.name -eq $name }
    if (-not $entry) { throw "No se encontró la conexión $name en Web.config." }
    $provider = "data source=$server;initial catalog=$database;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework"
    $entry.connectionString = "metadata=res://*/$model.csdl|res://*/$model.ssdl|res://*/$model.msl;provider=System.Data.SqlClient;provider connection string=`"$provider`""
}
Set-EntityConnection "INAMU_SOPORTEEntities" "INAMU_SOPORTE" $SupportSqlServer $SupportDatabase
Set-EntityConnection "INAMU_COMUNEntities" "ComunModel" $CommonSqlServer $CommonDatabase
($config.configuration.appSettings.add | Where-Object { $_.key -eq "rutaAD" }).value = $LdapPath
$config.Save($webConfigPath)

Import-Module WebAdministration
if (Test-Path "IIS:\Sites\$SiteName") {
    if (-not $Force) { throw "El sitio IIS '$SiteName' ya existe. Use -Force para reemplazarlo." }
    Remove-Website -Name $SiteName
}
if (-not (Test-Path "IIS:\AppPools\$SiteName")) { New-WebAppPool -Name $SiteName | Out-Null }
Set-ItemProperty "IIS:\AppPools\$SiteName" -Name managedRuntimeVersion -Value "v4.0"
Set-ItemProperty "IIS:\AppPools\$SiteName" -Name managedPipelineMode -Value "Integrated"
New-Website -Name $SiteName -Port $Port -PhysicalPath $PublishPath -ApplicationPool $SiteName | Out-Null

Write-Host "Publicación terminada: http://localhost:$Port/" -ForegroundColor Green
Write-Host "Autorice en SQL Server la identidad 'IIS AppPool\$SiteName'." -ForegroundColor Yellow
