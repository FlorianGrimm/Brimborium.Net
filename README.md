# Brimborium.Net
c# libs


# setup

dotnet new tool-manifest

https://github.com/dotnet/Nerdbank.GitVersioning/blob/master/doc/nbgv-cli.md

dotnet tool install --tool-path . nbgv

nbgv install

# helper

```
$csproj = Get-ChildItem -Path . -Filter "*.csproj" -Recurse
$csprojFullName = $csproj | %{ $_.FullName }

$found = Select-String -Path $csprojFullName -Pattern "package.targets"

$foundPath = $found | %{ $_.Path } 
$foundPath | % { [System.IO.Path]::GetDirectoryName($_) } | % { [System.IO.Path]::GetDirectoryName($_) } | % { [System.IO.Path]::GetDirectoryName($_) }
$foundPath | % {
    & dotnet build $_
}


Get-ChildItem -Path . -Filter "*.csproj" -Recurse | % {
    & dotnet build $_.FullName
}


```