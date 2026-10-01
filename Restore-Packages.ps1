[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$packagesPath = Join-Path $PSScriptRoot "Codigo\Código Fuente\packages"
if (-not (Test-Path $packagesPath)) { throw "No se encontró la carpeta local de paquetes: $packagesPath" }

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archives = Get-ChildItem -Path $packagesPath -Filter *.nupkg -Recurse
if (-not $archives) { throw "No se encontraron paquetes .nupkg para restaurar." }

foreach ($archive in $archives) {
    $destination = $archive.DirectoryName
    $temporary = Join-Path ([IO.Path]::GetTempPath()) ("inamu-nuget-" + [guid]::NewGuid().ToString("N"))
    try {
        [IO.Compression.ZipFile]::ExtractToDirectory($archive.FullName, $temporary)
        Get-ChildItem -Path $temporary -Force | ForEach-Object {
            Copy-Item $_.FullName -Destination $destination -Recurse -Force
        }
    }
    finally {
        if (Test-Path $temporary) { Remove-Item $temporary -Recurse -Force }
    }
}

Write-Host "Paquetes locales extraídos correctamente ($($archives.Count))." -ForegroundColor Green
