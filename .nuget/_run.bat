"%~dp0NuGet.exe" pack "..\DNTScheduler\DNTScheduler.csproj" -Prop Configuration=Release
copy "%~dp0*.nupkg" "%localappdata%\NuGet\Cache"

pause