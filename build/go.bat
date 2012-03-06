@ECHO OFF

SET msbuild="%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

IF '%1'=='' (SET target="build") ELSE (SET target=%1)
IF '%2'=='' (SET buildConfig="Release") ELSE (SET buildConfig=%2)

IF '%target%'=='dist' (SET buildConfig="Nuget")

%msbuild% .\Build.proj /t:%target% /property:Configuration=%buildConfig%