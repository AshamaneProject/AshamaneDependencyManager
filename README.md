# AshamaneDependencyManager

Ashamane Dependency Manager (ADM) is a tool aiming to provide simplification in custom scripts integration into AshamaneCore

Using a single command, it allow to retrieve a module from any github repository, and install it in your src/server/scripts/custom directory. The ADM will also generate the required AddSC call for you, and download the required SQL files

## Usage

### Create your own Module

You can find a module skeleton here : https://github.com/AshamaneProject/CustomScriptModule

The most important part is the "module.json" containing all your module info, ADM will first get this file to figure out which script & SQL to download

### Install/Remove a Module

ADM is integrated in AshamaneCore (you can find it at contrib/adm), just launch adm.bat, then you can retrieve & install a module following a simple three step wizard

You can also use a single command

#### Install
```
   dotnet AshamaneDependencyManager.dll AshamaneProject/CustomScriptModule add [AshamaneCore root directory]
```

#### Remove
```
   dotnet AshamaneDependencyManager.dll AshamaneProject/CustomScriptModule remove [AshamaneCore root directory]
```

If no core directory provided, adm expect to be run from AshamaneCore/Contrib/adm
