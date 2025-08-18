@echo off
setlocal enableextensions

REM Fix all .md files to be valid UTF-8 and normalize smart punctuation to ASCII.
REM Usage: double-click or run from a Developer Command Prompt in the repo root.

set "PS1=%~dp0fix-markdown-utf8.ps1"
if not exist "%PS1%" (
  echo PowerShell script not found: %PS1%
  exit /b 1
)

REM Remove trailing backslash to avoid CMD -> PowerShell quoting issues ("C:\path\")
set "ROOT=%~dp0"
if "%ROOT:~-1%"=="\" set "ROOT=%ROOT:~0,-1%"

echo ===== Normalizing all Markdown (*.md) files to UTF-8 (no BOM) =====
powershell -NoProfile -ExecutionPolicy Bypass -File "%PS1%" -Root "%ROOT%"
set ERR=%ERRORLEVEL%
if %ERR% NEQ 0 (
  echo Completed with errors. See messages above.
) else (
  echo Done. All Markdown files normalized to UTF-8.
)

endlocal
