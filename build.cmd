@echo off
cd /d "%~dp0"

if not exist webview2 (
  curl -sL -o webview2.zip https://www.nuget.org/api/v2/package/Microsoft.Web.WebView2/1.0.4258.31 || exit /b 1
  mkdir webview2
  tar -xf webview2.zip -C webview2 || exit /b 1
  del webview2.zip
)
copy /y webview2\lib\net462\Microsoft.Web.WebView2.Core.dll . >nul
copy /y webview2\lib\net462\Microsoft.Web.WebView2.WinForms.dll . >nul
copy /y webview2\runtimes\win-x64\native\WebView2Loader.dll . >nul

%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe /nologo /target:winexe /platform:x64 /win32icon:app.ico /out:"To-Do List.exe" ^
  /r:Microsoft.Web.WebView2.Core.dll /r:Microsoft.Web.WebView2.WinForms.dll app.cs
