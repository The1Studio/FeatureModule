@echo off
cd /d %~dp0
for %%a in ("%~dp0\.") do set "parent=%%~nxa"
set UnityProjectName=Unity%parent%

title TheOneStudio Setup Script
echo Welcome to TheOneStudio setting up pipeline!!
echo This script will:
echo 1. Change unity project name to %UnityProjectName%
echo 2. Update UITemplate Submodule and GDK submodule
pause
setlocal
@REM Remove old submodule
git rm UnityHyperCasualTemplate\Packages\com.gdk.core\
git rm UnityHyperCasualTemplate\Assets\UITemplate\
git rm UnityHyperCasualTemplate\Packages\com.gdk.3rd\
@REM Rename Unity Project
ren UnityHyperCasualTemplate %UnityProjectName%
git remote add template git@github.com:The1Studio/HyperCasualTemplate.git
@REM RE-Add submodule
git submodule add git@github.com:GameDevelopmentKit/GameFoundation.git %UnityProjectName%/Packages/com.gdk.core
git submodule add git@github.com:The1Studio/UITemplate.git %UnityProjectName%/Assets/UITemplate
git submodule add git@github.com:GameDevelopmentKit/ThirdPartyServices.git %UnityProjectName%/Packages/com.gdk.3rd
git submodule foreach git switch NativeGame
pause
