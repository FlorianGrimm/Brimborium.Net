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

@REM cd %~dp0Brimborium.WebDavServer
@REM dotnet build
@REM @IF ERRORLEVEL 1 GOTO :fail
@REM 
@REM cd %~dp0Brimborium.WebFlow
@REM dotnet build
@REM @IF ERRORLEVEL 1 GOTO :fail
@REM 
@REM cd %~dp0Brimborium.WebSocket
@REM dotnet build
@REM @IF ERRORLEVEL 1 GOTO :fail

cd %~dp0
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

goto :eof
:fail
@REM cd %~dp0

@echo "Failed"
