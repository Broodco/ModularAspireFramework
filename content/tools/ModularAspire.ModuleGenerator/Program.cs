using System.CommandLine;
using System.Diagnostics;

namespace ModularAspire.ModuleGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("Module generator for ModularAspire framework");
            
            var nameOption = new Option<string>(
                "--name",
                "Name of the module to create");
            nameOption.IsRequired = true;
            
            var rootDirOption = new Option<string>(
                "--root-dir",
                () => Directory.GetCurrentDirectory(),
                "Root directory of the solution");
            
            rootCommand.AddOption(nameOption);
            rootCommand.AddOption(rootDirOption);
            
            rootCommand.SetHandler((string name, string rootDir) =>
            {
                GenerateModule(name, rootDir);
            }, nameOption, rootDirOption);
            
            return rootCommand.Invoke(args);
        }
        
        static void GenerateModule(string moduleName, string rootDir)
        {
            if (string.IsNullOrWhiteSpace(moduleName) || !char.IsUpper(moduleName[0]))
            {
                Console.WriteLine("Module name must start with an uppercase letter.");
                return;
            }
            
            string templatePath = Path.Combine(rootDir, "templates", "ModuleTemplate");
            string modulesPath = Path.Combine(rootDir, "src", "Modules");
            string newModulePath = Path.Combine(modulesPath, moduleName);
            
            if (Directory.Exists(newModulePath))
            {
                Console.WriteLine($"Module {moduleName} already exists.");
                return;
            }
            
            Directory.CreateDirectory(newModulePath);
            
            CopyDirectoryWithReplacement(templatePath, newModulePath, "ModuleName", moduleName);
            
            UpdateSolutionFile(rootDir, moduleName);
            
            Console.WriteLine($"Module {moduleName} has been created successfully!");
        }
        
        static void CopyDirectoryWithReplacement(string sourceDir, string destDir, string placeholder, string replacement)
        {
            Directory.CreateDirectory(destDir);
            
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destFilePath = Path.Combine(destDir, fileName.Replace(placeholder, replacement));
                
                string content = File.ReadAllText(filePath);
                content = content.Replace($"${{{placeholder}}}", replacement)
                                .Replace(placeholder, replacement);
                
                File.WriteAllText(destFilePath, content);
            }
            
            // Process subdirectories
            foreach (string dirPath in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dirPath);
                string destSubDir = Path.Combine(destDir, dirName.Replace(placeholder, replacement));
                CopyDirectoryWithReplacement(dirPath, destSubDir, placeholder, replacement);
            }
        }
        
        static void UpdateSolutionFile(string rootDir, string moduleName)
        {
            string solutionFile = Directory.GetFiles(rootDir, "*.sln").FirstOrDefault();
            if (solutionFile == null)
            {
                Console.WriteLine("Solution file not found.");
                return;
            }

            string[] projectSuffixes = new[] { 
                "Domain", 
                "Application", 
                "Infrastructure", 
                "Presentation", 
                "IntegrationEvents" 
            };

            foreach (var suffix in projectSuffixes)
            {
                string projectPath = $"src/Modules/{moduleName}/ModularAspire.Modules.{moduleName}.{suffix}";
        
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"sln \"{solutionFile}\" add \"{projectPath}\" --solution-folder \"src/Modules/{moduleName}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
        
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
        
                Console.WriteLine(output);
            }
        }
    }
}