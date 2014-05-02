@REM Store args
set _SolutionDir = %1%
set _ProjectDir = %2%
set _OutDir = %3%

@REM Do works
if not exist "%_SolutionDir%xiaowan" mkdir "%_SolutionDir%xiaowan" >nul

copy /y "%_ProjectDir%%_OutDir%xiaowan.exe" "%_SolutionDir%xiaowan\" >nul
copy /y "%_ProjectDir%%_OutDir%xiaowan.exe.Config" "%_SolutionDir%xiaowan\" >nul
copy /y "%_ProjectDir%%_OutDir%ControlExs.dll" "%_SolutionDir%xiaowan\" >nul
copy /y "%_ProjectDir%%_OutDir%AxInterop.WMPLib.dll" "%_SolutionDir%xiaowan\" >nul
copy /y "%_ProjectDir%%_OutDir%Interop.WMPLib.dll" "%_SolutionDir%xiaowan\" >nul
copy /y "%_ProjectDir%%_OutDir%Microsoft.WindowsAPICodePack.dll" "%_SolutionDir%xiaowan\" >nul
copy /y "%_ProjectDir%%_OutDir%Microsoft.WindowsAPICodePack.Shell.dll" "%_SolutionDir%xiaowan\" >nul

if not exist "%_SolutionDir%xiaowan\en" mkdir "%_SolutionDir%xiaowan\en" >nul
if not exist "%_SolutionDir%xiaowan\zh-Hant" mkdir "%_SolutionDir%xiaowan\zh-Hant" >nul
if not exist "%_SolutionDir%xiaowan\zh-TW" mkdir "%_SolutionDir%xiaowan\zh-TW" >nul
if not exist "%_SolutionDir%xiaowan\ja-JP" mkdir "%_SolutionDir%xiaowan\ja-JP" >nul

xcopy /y "%_ProjectDir%%_OutDir%en" "%_SolutionDir%xiaowan\en" >nul
xcopy /y "%_ProjectDir%%_OutDir%zh-Hant" "%_SolutionDir%xiaowan\zh-Hant" >nul
xcopy /y "%_ProjectDir%%_OutDir%zh-Hant" "%_SolutionDir%xiaowan\zh-TW" >nul
xcopy /y "%_ProjectDir%%_OutDir%ja-JP" "%_SolutionDir%xiaowan\ja-JP" >nul

echo PostBuildEvent Completed.
