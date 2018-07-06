using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AshamaneDependencyManager
{
    class ScriptLoaderEditor
    {
        public static void EditAddSC(String ScriptLoaderPath, String AddSC, bool add)
        {
            if (!File.Exists(ScriptLoaderPath))
                throw new Exception("Invalid directory or custom_script_loader.cpp does not exist");

            List<String> scriptLoaderContent = new List<String>(File.ReadAllLines(ScriptLoaderPath));
            List<String> newFileContent = new List<String>();

            bool inDeclarationContext = false;
            bool addSCDeclarationFound = false;

            bool inCallContext = false;
            bool addSCCallFound = false;

            foreach (String line in scriptLoaderContent)
            {
                if (line.Contains("ADM declaration begin"))
                    inDeclarationContext = true;

                if (line.Contains("ADM call begin"))
                    inCallContext = true;

                if (line.Contains(AddSC))
                {
                    if (inDeclarationContext)
                    {
                        addSCDeclarationFound = true;

                        if (!add)
                            continue;
                    }

                    if (inCallContext)
                    {
                        addSCCallFound = true;

                        if (!add)
                            continue;
                    }
                }

                if (line.Contains("ADM declaration end"))
                {
                    if (!addSCDeclarationFound && add)
                        newFileContent.Add("void " + AddSC + "();");

                    inDeclarationContext = false;
                }

                if (line.Contains("ADM call end"))
                {
                    if (!addSCCallFound && add)
                        newFileContent.Add("    " + AddSC + "();");

                    inCallContext = false;
                }

                newFileContent.Add(line);
            }

            File.WriteAllLines(ScriptLoaderPath, newFileContent);
        }
    }
}
