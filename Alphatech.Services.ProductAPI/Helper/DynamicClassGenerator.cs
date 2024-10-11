using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Alphatech.Services.ProductAPI.Helper
{
    public class DynamicClassGenerator
    {
        public void GenerateDTOClass(string className, Dictionary<string, Type> properties)
        {
            // Get the project base directory
            string projectDirectory = Directory.GetCurrentDirectory();
            string dtoFolderPath = Path.Combine(projectDirectory, "Models", "Dto");

            // Ensure the directory exists
            if (!Directory.Exists(dtoFolderPath))
            {
                Directory.CreateDirectory(dtoFolderPath);
            }

            // Generate the class content
            var classBuilder = new StringBuilder();
            classBuilder.AppendLine("using System;");
            classBuilder.AppendLine("namespace YourNamespace.Models.Dto"); // Adjust this namespace if necessary
            classBuilder.AppendLine("{");
            classBuilder.AppendLine($"    public class {className}");
            classBuilder.AppendLine("    {");

            foreach (var property in properties)
            {
                classBuilder.AppendLine($"        public {property.Value.Name} {property.Key} {{ get; set; }}");
            }

            classBuilder.AppendLine("    }");
            classBuilder.AppendLine("}");

            // Save the generated class to the /Models/Dto folder
            var outputFilePath = Path.Combine(dtoFolderPath, $"{className}.cs");
            File.WriteAllText(outputFilePath, classBuilder.ToString());

            Console.WriteLine($"Class {className} generated at {outputFilePath}");
        }

        public Type CompileGeneratedDTOClass(string className)
        {
            string projectDirectory = Directory.GetCurrentDirectory();
            string dtoFolderPath = Path.Combine(projectDirectory, "Models", "Dto");
            var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(dtoFolderPath, $"{className}.cs")));

            var compilation = CSharpCompilation.Create(
                $"{className}.dll",
                new[] { syntaxTree },
                new[]
                {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        Console.Error.WriteLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }
                    throw new InvalidOperationException("Compilation failed.");
                }

                // Load the compiled assembly
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                // Get the compiled type (class)
                return assembly.GetType($"YourNamespace.Models.Dto.{className}");
            }
        }
    }
}
