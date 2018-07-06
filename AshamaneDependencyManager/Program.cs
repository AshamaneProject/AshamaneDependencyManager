using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;

namespace AshamaneDependencyManager
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                String githubProject    = "";
                String action           = "";
                // If no directory specified, we expect to be in AshamaneCore/Contrib/adm
                String directory        = Directory.GetCurrentDirectory() + "../..";

                if (args.Length < 2)
                {
                    Console.WriteLine("Welcome to the Ashamane Dependency Manager");
                    Console.WriteLine("");

                    Console.WriteLine("Please enter the github module name (example : AshamaneProject/CustomScriptModule) : ");
                    githubProject = Console.ReadLine();

                    Console.WriteLine("Enter the action required (add/remove) : ");
                    action = Console.ReadLine();

                    Console.WriteLine("If in 'contrib/adm' directory, left blank. Else, enter your core root directory : ");
                    String newDirectory = Console.ReadLine();

                    if (newDirectory != "")
                        directory = newDirectory;
                }
                else
                {
                    githubProject   = args[0];
                    action          = args[1];

                    if (args.Length == 3)
                        directory = args[2];
                }

                if (action != "add" && action != "remove")
                    throw new Exception("Invalid action");

                if (!githubProject.Contains("/"))
                    throw new Exception("Invalid github project");

                String moduleJson = HttpHelper.GetURLContent(HttpHelper.GetRawGithubURL(githubProject, "module.json"));
                var serializer = new DataContractJsonSerializer(typeof(ModuleDefinition));
                ModuleDefinition moduleDef = (ModuleDefinition)serializer.ReadObject(GenerateStreamFromString(moduleJson));

                String customScriptPath = directory + "/src/server/scripts/Custom/";
                String customScriptLoaderPath = customScriptPath + "custom_script_loader.cpp";

                String customSqlPath = directory + "/sql/ashamane/custom/";

                String moduleDirectoryName = RemoveSpecialCharacters(moduleDef.Title);
                String modulePath = customScriptPath + moduleDirectoryName;
                String moduleDefPath = modulePath + "/module.json";

                if (!File.Exists(customScriptLoaderPath))
                    throw new Exception("Invalid directory or custom_script_loader.cpp does not exist");

                bool addModule = action == "add";

                if (addModule && File.Exists(moduleDefPath))
                {
                    String oldModuleJson = File.ReadAllText(moduleDefPath);
                    ModuleDefinition oldModuleDef = (ModuleDefinition)serializer.ReadObject(GenerateStreamFromString(oldModuleJson));

                    if (oldModuleDef.ModuleVersion >= moduleDef.ModuleVersion)
                    {
                        Console.WriteLine("Same or newer version of this package is already installed (" + oldModuleDef.ModuleVersion.ToString() + "), nothing to do");
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("Installing newer version " + moduleDef.ModuleVersion.ToString() + " over actual version " + oldModuleDef.ModuleVersion.ToString());
                }
                else if (Directory.Exists(modulePath))
                {
                    if (addModule)
                    {
                        Console.WriteLine("Module folder '" + moduleDirectoryName + "' already exist, do you want to overwrite it ? y/N");
                        String answer = Console.ReadLine();

                        if (answer != "y" && answer != "Y")
                            return;
                    }

                    Directory.Delete(modulePath, true);
                    Thread.Sleep(1000);
                }

                if (addModule)
                {
                    Directory.CreateDirectory(modulePath);

                    moduleDef.Files.Add("module.json");
                    foreach (String file in moduleDef.Files)
                        using (var client = new WebClient())
                            client.DownloadFile(HttpHelper.GetRawGithubURL(githubProject, file), modulePath + "/" + file);

                    foreach (String sqlFile in moduleDef.SqlFiles.OnInstall)
                        using (var client = new WebClient())
                            client.DownloadFile(HttpHelper.GetRawGithubURL(githubProject, "sql/" + sqlFile), customSqlPath + "/world/" + moduleDirectoryName + "_" + sqlFile);


                    foreach (String sqlFile in moduleDef.SqlFiles.OnRemove)
                        File.Delete(customSqlPath + "/world/" + moduleDirectoryName + "_" + sqlFile);
                }
                else
                {

                    foreach (String sqlFile in moduleDef.SqlFiles.OnRemove)
                        using (var client = new WebClient())
                            client.DownloadFile(HttpHelper.GetRawGithubURL(githubProject, "sql/" + sqlFile), customSqlPath + "/world/" + moduleDirectoryName + "_" + sqlFile);

                    foreach (String sqlFile in moduleDef.SqlFiles.OnInstall)
                        File.Delete(customSqlPath + "/world/" + moduleDirectoryName + "_" + sqlFile);

                }

                foreach (String AddSC in moduleDef.AddSCs)
                    ScriptLoaderEditor.EditAddSC(customScriptLoaderPath, AddSC, addModule);

                Console.WriteLine(moduleDef.Title + " have been successfuly " + (addModule ? "installed": "removed"));
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                if (ex.Message != "")
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                }

                PrintUsage();
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: ./adm AshamaneProject/CustomScriptModule add|remove [AshamaneCore root directory]");
            Console.WriteLine();
            Console.WriteLine("If no core directory provided, adm expect to be run from AshamaneCore/Contrib/adm");
            Console.ReadKey();
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '-' || c == '_')
                    sb.Append(c);

            return sb.ToString();
        }
    }
}
