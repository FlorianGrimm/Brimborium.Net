echo %~dp0

REM dir -Recurse *.sln | %{'cd %~dp0' + $_.Directory.Name}


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

cd %~dp0Brimborium.CodeGeneration
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0Brimborium.Configuration
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

@REM cd %~dp0Brimborium.Functional
@REM dotnet build
@REM @IF ERRORLEVEL 1 GOTO :fail

@REM cd %~dp0Brimborium.WebDavServer
@REM dotnet build
@REM @IF ERRORLEVEL 1 GOTO :fail

@REM cd %~dp0Brimborium.WebFlow
@REM dotnet build
@REM @IF ERRORLEVEL 1 GOTO :fail

@REM cd %~dp0Brimborium.WebSocket
@REM dotnet build
@REM @IF ERRORLEVEL 1 GOTO :fail

@REM cd %~dp0Brimborium.CodeBlocks
@REM dotnet build
@REM @IF ERRORLEVEL 1 GOTO :fail

cd %~dp0Brimborium.Transformation
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

cd %~dp0
dotnet build
@IF ERRORLEVEL 1 GOTO :fail

goto :eof
:fail
@REM cd %~dp0

@echo "Failed"
