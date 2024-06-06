cd /d %~dp0

@echo [prepare somethings]
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath`) do set "path=%path%;%%i\MSBuild\Current\Bin;%%i\Common7\IDE"

echo [build app using vs2022]
cd ..\src\
dotnet publish -c Release -p:PublishProfile=FolderProfile
cd /d %~dp0

del ..\src\bin\Release\net472\win7-x86\*.pdb
del ..\src\bin\Release\net472\win7-x86\*.config
"C:\Program Files\7-Zip\7z.exe" a ScreenCap.7z ..\src\bin\Release\net472\win7-x86\* -t7z -mx=5 -mf=BCJ2 -r -y

@pause
