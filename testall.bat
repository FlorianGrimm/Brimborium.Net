cd %~dp0

dotnet test Brimborium.CodeBlocks\Brimborium.CodeBlocks.Library.Test\Brimborium.CodeBlocks.Library.Test.csproj
@IF ERRORLEVEL 1 GOTO :fail

dotnet test Brimborium.CodeBlocks\Brimborium.Typing.Test\Brimborium.Typing.Test.csproj
@IF ERRORLEVEL 1 GOTO :fail

dotnet test Brimborium.CodeFlow\Brimborium.CodeFlow.Test\Brimborium.CodeFlow.Test.csproj
@IF ERRORLEVEL 1 GOTO :fail

dotnet test Brimborium.Extensions.Logging.LocalFile\test\Brimborium.Extensions.Logging.LocalFile.Test.csproj
@IF ERRORLEVEL 1 GOTO :fail

dotnet test Brimborium.Functional\Brimborium.Functional.Test\Brimborium.Functional.Test.csproj
@IF ERRORLEVEL 1 GOTO :fail

dotnet test Brimborium.Registrator\Brimborium.Registrator.Test\Brimborium.Registrator.Test.csproj
@IF ERRORLEVEL 1 GOTO :fail

dotnet test Brimborium.RowVersion\Brimborium.RowVersion.Test\Brimborium.RowVersion.Test.csproj
@IF ERRORLEVEL 1 GOTO :fail

dotnet test Brimborium.TypedSql\Brimborium.Tracking.Extensions.Test\Brimborium.Tracking.Extensions.Test.csproj
@IF ERRORLEVEL 1 GOTO :fail

dotnet test Brimborium.TypedSql\Brimborium.Tracking.Test\Brimborium.Tracking.Test.csproj
@IF ERRORLEVEL 1 GOTO :fail

# dotnet test Brimborium.WebFlow\Brimborium.WebFlow.Test\Brimborium.WebFlow.Test.csproj
# dotnet test Brimborium.WebFlow\Demo.Logic.Test\Demo.Logic.Test.csproj
# dotnet test Brimborium.WebFlow\Demo.WebApplication.Test\Demo.WebApplication.Test.csproj

goto :eof
:fail
@REM cd %~dp0

@echo "Failed"
