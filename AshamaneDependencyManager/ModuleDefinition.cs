using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AshamaneDependencyManager
{
    [DataContract(Name = "ModuleDefinition")]
    class ModuleDefinition
    {
        [DataMember(Name = "title")]
        public String Title;
   
        [DataMember(Name = "version")]
        public String VersionString
        {
            get { return VersionString; }
            set
            {
                ModuleVersion = new Version(value);
            }
        }

        [DataMember(Name = "files")]
        public List<String> Files;

        [DataMember(Name = "sql")]
        public ModuleSqlFiles SqlFiles;

        [DataMember(Name = "AddSCs")]
        public List<String> AddSCs;


        public Version ModuleVersion;
    }

    [DataContract(Name = "ModuleSqlFiles")]
    class ModuleSqlFiles
    {
        [DataMember(Name = "onInstall")]
        public List<String> OnInstall;

        [DataMember(Name = "onRemove")]
        public List<String> OnRemove;
    }
}
