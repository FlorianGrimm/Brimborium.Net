echo %~dp0

cd %~dp0Brimborium.Registrator
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0Brimborium.TypedSql
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0Brimborium.RowVersion
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0Brimborium.Extensions.Logging.LocalFile
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0Brimborium.CodeFlow
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0Brimborium.WebDavServer
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0Brimborium.WebFlow
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0Brimborium.WebSocket
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

goto :eof
:fail
@REM cd %~dp0

@echo "Failed"
