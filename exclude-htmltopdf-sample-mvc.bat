@echo off
REM ====================================================================
REM Batch file to exclude HTML to PDF sample from EJ2 MVC Sample Browser
REM Compatible with Windows 7, 8, 10, 11, and Windows Server editions
REM All-in-one script - no separate PowerShell file needed
REM ====================================================================
setlocal enabledelayedexpansion

echo.
echo ========================================================
echo Excluding HTML to PDF Sample from EJ2 MVC Sample Browser
echo ========================================================
echo.

REM Check if PowerShell is available
where powershell >nul 2>nul
if %errorlevel% neq 0 (
    echo [ERROR] PowerShell is required but not found. Please install PowerShell.
    echo.
    pause
    exit /b 1
)

REM Delete the physical files
echo Step 1: Deleting physical files...
if exist "Controllers\PDF\HtmltoPDFController.cs" (
    del /F /Q "Controllers\PDF\HtmltoPDFController.cs" 2>nul
    if !errorlevel! equ 0 (
        echo   [OK] Deleted: Controllers\PDF\HtmltoPDFController.cs
    ) else (
        echo   [WARN] Could not delete Controllers\PDF\HtmltoPDFController.cs
    )
) else (
    echo   [INFO] File not found: Controllers\PDF\HtmltoPDFController.cs
)

if exist "Views\PDF\HtmltoPDF.cshtml" (
    del /F /Q "Views\PDF\HtmltoPDF.cshtml" 2>nul
    if !errorlevel! equ 0 (
        echo   [OK] Deleted: Views\PDF\HtmltoPDF.cshtml
    ) else (
        echo   [WARN] Could not delete Views\PDF\HtmltoPDF.cshtml
    )
) else (
    echo   [INFO] File not found: Views\PDF\HtmltoPDF.cshtml
)

if exist "Content\PDF\HtmltoPDFController.txt" (
    del /F /Q "Content\PDF\HtmltoPDFController.txt" 2>nul
    if !errorlevel! equ 0 (
        echo   [OK] Deleted: Content\PDF\HtmltoPDFController.txt
    ) else (
        echo   [WARN] Could not delete Content\PDF\HtmltoPDFController.txt
    )
) else (
    echo   [INFO] File not found: Content\PDF\HtmltoPDFController.txt
)

if exist "Content\PDF\HtmltoPDFConversion.txt" (
    del /F /Q "Content\PDF\HtmltoPDFConversion.txt" 2>nul
    if !errorlevel! equ 0 (
        echo   [OK] Deleted: Content\PDF\HtmltoPDFConversion.txt
    ) else (
        echo   [WARN] Could not delete Content\PDF\HtmltoPDFConversion.txt
    )
) else (
    echo   [INFO] File not found: Content\PDF\HtmltoPDFConversion.txt
)
echo.

REM Update configuration files using PowerShell
echo Step 2: Updating configuration files...
echo.

REM Create temporary PowerShell script
set "TEMP_PS_SCRIPT=%TEMP%\exclude_htmltopdf_%RANDOM%_%RANDOM%.ps1"

(
echo # PowerShell script to remove HtmltoPDF entries from configuration files
echo # Auto-generated temporary script - Compatible with PowerShell 2.0+
echo.
echo $ErrorActionPreference = 'Stop'
echo.
echo try {
echo     Write-Host "  - Processing Scripts\samplelist.js..." -ForegroundColor Cyan
echo.    
echo     $sampleListPath = 'Scripts\samplelist.js'
echo     if ^(Test-Path $sampleListPath^) {
echo         $content = [System.IO.File]::ReadAllText^($sampleListPath, [System.Text.Encoding]::UTF8^)
echo         $pattern = ',\s*\{\s*"url":\s*"HtmltoPDF"[\s\S]*?"sourceFiles":\s*\[[\s\S]*?\]\s*\}'
echo         $content = $content -replace $pattern, ''
echo         [System.IO.File]::WriteAllText^($sampleListPath, $content, [System.Text.Encoding]::UTF8^)
echo         Write-Host "  [OK] Removed HtmltoPDF entry from samplelist.js" -ForegroundColor Green
echo     } else {
echo         Write-Host "  [ERROR] samplelist.js not found" -ForegroundColor Red
echo         exit 1
echo     }
echo.    
echo     Write-Host "  - Processing EJ2MVCSampleBrowser.csproj..." -ForegroundColor Cyan
echo.    
echo     $csprojPath = 'EJ2MVCSampleBrowser.csproj'
echo     if ^(Test-Path $csprojPath^) {
echo         $lines = [System.IO.File]::ReadAllLines^($csprojPath, [System.Text.Encoding]::UTF8^)
echo         $newLines = $lines ^| Where-Object { $_ -notmatch 'HtmltoPDF' }
echo         [System.IO.File]::WriteAllLines^($csprojPath, $newLines, [System.Text.Encoding]::UTF8^)
echo         Write-Host "  [OK] Removed all HtmltoPDF entries from project file" -ForegroundColor Green
echo     } else {
echo         Write-Host "  [ERROR] EJ2MVCSampleBrowser.csproj not found" -ForegroundColor Red
echo         exit 1
echo     }
echo.    
echo     exit 0
echo } catch {
echo     Write-Host "  [ERROR] $^($_.Exception.Message^)" -ForegroundColor Red
echo     exit 1
echo }
) > "%TEMP_PS_SCRIPT%"

REM Execute PowerShell script
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%TEMP_PS_SCRIPT%"
set PS_EXIT_CODE=%errorlevel%

REM Clean up temporary script
if exist "%TEMP_PS_SCRIPT%" del /F /Q "%TEMP_PS_SCRIPT%" 2>nul

:show_result
echo.
if %PS_EXIT_CODE% equ 0 (
    echo ========================================================
    echo SUCCESS: HTML to PDF sample has been successfully excluded!
    echo ========================================================
    echo.
    echo The following changes have been made:
    echo   1. Deleted Controllers\PDF\HtmltoPDFController.cs
    echo   2. Deleted Views\PDF\HtmltoPDF.cshtml
    echo   3. Deleted Content\PDF\HtmltoPDFController.txt
    echo   4. Deleted Content\PDF\HtmltoPDFConversion.txt
    echo   5. Removed entries from Scripts\samplelist.js
    echo   6. Removed entries from EJ2MVCSampleBrowser.csproj
) else (
    echo ========================================================
    echo ERROR: Some operations failed. Please check the output above.
    echo ========================================================
)
echo.