# AshamaneDependencyManager

Ashamane Dependency Manager (ADM) is a tool aiming to provide simplification in custom scripts integration into AshamaneCore

Using a single command, it allow to retrieve a module from any github repository, and install it in your src/server/scripts/custom directory. The ADM will also generate the required AddSC call for you, and download the required SQL files

## Usage

### Create your own Module

You can find a module skeleton here : https://github.com/AshamaneProject/CustomScriptModule

The most important part is the "module.json" containing all your module info, ADM will first get this file to figure out which script & SQL to download

### Install/Remove a Module

Download ADM (Release link coming soon, integration to Ashamanecore soon), then you can retrieve & instal a module using a simple command

You can also launch it as any other software without parameters, and a wizard will guide you throught the whole process

#### Install
```
   ./adm AshamaneProject/CustomScriptModule add [AshamaneCore root directory]
```

#### Remove
```
   ./adm AshamaneProject/CustomScriptModule remove [AshamaneCore root directory]
```

If no core directory provided, adm expect to be run from AshamaneCore/Contrib/adm
