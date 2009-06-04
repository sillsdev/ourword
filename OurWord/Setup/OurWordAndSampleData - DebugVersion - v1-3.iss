[Files]
Source: ..\bin\Debug\OurWord.exe; DestDir: {app}
Source: ..\bin\Debug\JWTools.dll; DestDir: {app}
Source: ..\bin\Debug\JWdb.dll; DestDir: {app}
Source: ..\..\Help\OurWordHelp.chm; DestDir: {app}\Help
Source: SampleData\*.*; DestDir: {userdocs}\OurWordSample; Flags: recursesubdirs createallsubdirs
Source: ..\Loc\*.*; DestDir: {commonappdata}\OurWord\Loc
Source: ..\..\lib\Palaso.Services.dll; DestDir: {app}
Source: ..\..\lib\PalasoUIWindowsForms.dll; DestDir: {app}
Source: ..\..\lib\PalasoReporting.dll; DestDir: {app}
Source: ..\..\lib\KeymanLink.dll; DestDir: {app}
Source: ..\bin\Debug\Chorus.exe; DestDir: {app}
Source: ..\bin\Debug\ChorusMerge.exe; DestDir: {app}
[Registry]
Root: HKCU; Subkey: Software\The Seed Company\Our Word!; ValueType: string; ValueName: HelpFile; ValueData: {app}\Help\OurWordHelp.chm; Flags: uninsdeletevalue
Root: HKCU; Subkey: Software\The Seed Company\Our Word!\Backup; ValueType: string; ValueName: Folder; ValueData: {userdocs}\OurWordSample\BackupFiles; Flags: uninsdeletevalue
Root: HKCU; Subkey: Software\The Seed Company\Our Word!\Backup; ValueType: string; ValueName: Enabled; ValueData: true; Flags: uninsdeletevalue
Root: HKCU; Subkey: Software\The Seed Company\Our Word!\MRU; ValueType: string; ValueData: 1; Flags: uninsdeletevalue createvalueifdoesntexist
Root: HKCU; Subkey: Software\The Seed Company\Our Word!\MRU; ValueType: string; ValueData: {userdocs}\OurWordSample\Settings\OurWordSample.owp; Flags: uninsdeletevalue createvalueifdoesntexist; ValueName: 001
Root: HKCU; Subkey: Software\The Seed Company\Our Word!; ValueName: AppDir; ValueData: {app}; ValueType: string
Root: HKCU; Subkey: Software\The Seed Company\Our Word!\Options\; ValueType: string; ValueData: {userdocs}\OurWordSample\Pictures; Flags: createvalueifdoesntexist uninsdeletekey; ValueName: PictureSearchPath
[Dirs]
Name: {app}\Help
[Setup]
AppName=Our Word!
AppVerName=1.3
DefaultDirName={pf}\TSC\Our Word
AllowUNCPath=false
AllowNoIcons=true
DefaultGroupName=Our Word!
ShowLanguageDialog=yes
PrivilegesRequired=admin
AlwaysShowDirOnReadyPage=true
AlwaysShowGroupOnReadyPage=true
AppVersion=1.3
UninstallDisplayIcon=
UninstallDisplayName=Our Word!
WindowVisible=true
WindowResizable=true
OutputBaseFilename=SetupOurWord - Debug Version
ShowTasksTreeLines=true
AppID={{B5CC113B-597C-4E1C-9C15-CDF7912E8BC2}
AppendDefaultDirName=true
[Icons]
Name: {group}\Our Word; Filename: {app}\OurWord.exe; WorkingDir: {app}; IconFilename: {app}\OurWord.exe; IconIndex: 0
Name: {userdesktop}\Our Word; Filename: {app}\OurWord.exe; WorkingDir: {app}; IconFilename: {app}\OurWord.exe; IconIndex: 0
Name: {group}\Our Word Help; Filename: {app}\Help\OurWordHelp.chm; WorkingDir: {app}\help; IconFilename: {app}\Help\OurWordHelp.chm
Name: {group}\Uninstall Our Word!; Filename: {uninstallexe}; WorkingDir: {app}; IconFilename: {{uninstallexe}
Name: {group}\Send an email to support; Filename: mailto:John_Wimbish@tsco.org
