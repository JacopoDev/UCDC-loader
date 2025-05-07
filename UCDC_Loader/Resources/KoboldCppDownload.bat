
setlocal enabledelayedexpansion

:: Get the download URL line
curl -s https://api.github.com/repos/LostRuins/koboldcpp/releases/latest | findstr /i "browser_download_url" | findstr /i "koboldcpp.exe" > temp.txt

:: Extract and clean the URL
for /f "usebackq delims=" %%A in ("temp.txt") do (
    set "line=%%A"
)

:: Remove the leading ' "browser_download_url": "' and trailing quote
set "line=%line: =%"
set "url=%line:"browser_download_url":"="%"
set "url=%url:~1,-1%"

:: Show and download
echo Downloading from: %url%
curl -L -o ./KoboldCpp/koboldcpp.exe %url%


:: Clean up
del temp.txt
endlocal