using Brimborium.BaseMethods;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

using System.Linq;
using System.Reflection;

namespace Brimborium.SourceGenerator.BaseMethods.Test;
public class EquatableTests {
    /*
     docs\features\source-generators.cookbook.md
     */

    private int SourceCount = 0;

    public EquatableTests() {
        //this.SourceCount = 1 + AttributeSourceHelper.getBaseMethodsSources().Length;
        this.SourceCount = 2;
    }

    [Fact]
    public void IgnoreEquality_01_Test() {
        // using Brimborium.BaseMethods;
        const string source = @"
using Brimborium.BaseMethods;

namespace Test {

    [Equatable(Explicit = false)]
    public partial record UnitTest1Record(
        int A,
        [property: IgnoreEquality] int B,
        [property: IgnoreEquality] int C = 1
        );
}
";

        Compilation inputCompilation = CreateCompilation(source);
        BaseMethodsGenerator generator = new BaseMethodsGenerator();

        // Create the driver that will control the generation, passing in our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the generation pass
        // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

        // We can now assert things about the resulting compilation:
        Assert.True(!diagnostics.Where(d => d.Severity != DiagnosticSeverity.Hidden).Any(),
                diagnostics.FirstOrDefault(d => d.Severity != DiagnosticSeverity.Hidden)?.GetMessage() ?? "diagnostics");

        Assert.True(diagnostics.IsEmpty); // there were no diagnostics created by the generators
        Assert.True(inputCompilation.SyntaxTrees.Count() == SourceCount);  // we have two syntax trees, the original 'user' provided one, the one with the Attributtes
        Assert.True(outputCompilation.SyntaxTrees.Count() == SourceCount + 1); // plus the one added by the generator
        var outputDiagnostics = outputCompilation.GetDiagnostics();
        // verify the compilation with the added source has no diagnostics
        Assert.True(!outputDiagnostics.Where(d => d.Severity != DiagnosticSeverity.Hidden).Any(),
                outputDiagnostics.FirstOrDefault(d => d.Severity != DiagnosticSeverity.Hidden)?.GetMessage() ?? "outputDiagnostics");
        //Assert.True(outputDiagnostics.IsEmpty); 

        // Or we can look at the results directly:
        GeneratorDriverRunResult runResult = driver.GetRunResult();

        // The runResult contains the combined results of all generators passed to the driver
        Assert.True(runResult.GeneratedTrees.Length == 1);
        Assert.True(runResult.Diagnostics.IsEmpty);

        // Or you can access the individual results on a by-generator basis
        GeneratorRunResult generatorResult = runResult.Results[0];
        //Assert.True(generatorResult.Generator == generator);
        Assert.True(generatorResult.Diagnostics.IsEmpty);
        Assert.True(generatorResult.GeneratedSources.Length == 1);
        Assert.True(generatorResult.Exception is null);
    }

    private static string getLocation([CallerFilePath] string path = "") => path;

    [Fact]
    public void CompileSamples_Test() {
        var sourceFolderPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    getLocation(),
                    @"..\..\Brimborium.SourceGenerator.BaseMethods.TestEffect\Classes"
                )
                );
        //var sourcePath = @"C:\github.com\FlorianGrimm\Brimborium.Net\Brimborium.BaseMethods\Brimborium.SourceGenerator.BaseMethods.TestEffect\Classes\BaseEquality.Sample.cs";
        foreach (
            var sourcePath
            in System.IO.Directory.GetFiles(sourceFolderPath, "*Sample.cs", SearchOption.AllDirectories)
            ) {

            var source = File.ReadAllText(sourcePath);
            Compilation inputCompilation = CreateCompilation(source);
            BaseMethodsGenerator generator = new BaseMethodsGenerator();

            // Create the driver that will control the generation, passing in our generator
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the generation pass
            // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            {
                GeneratorDriverRunResult runResult = driver.GetRunResult();
                var generatorResult = runResult.Results[0];
                if (generatorResult.GeneratedSources.Count() == 1) {
                var gs = generatorResult.GeneratedSources.Single();
                System.IO.File.WriteAllText(sourcePath + ".txt", gs.SourceText.ToString());
            }
            }
            // We can now assert things about the resulting compilation:
            Assert.True(diagnostics.IsEmpty); // there were no diagnostics created by the generators
            Assert.Equal(SourceCount, inputCompilation.SyntaxTrees.Count());  // we have two syntax trees, the original 'user' provided one, the one with the Attributtes
            if (outputCompilation.SyntaxTrees.Count() != SourceCount + 1) {
                if (sourcePath.EndsWith("DefaultBehavior.Sample.cs")) {
                    // ignore
                    continue;
                }
                //
            }
            Assert.Equal(SourceCount + 1, outputCompilation.SyntaxTrees.Count()); // plus the one added by the generator

            var outputDiagnostics = outputCompilation.GetDiagnostics();
            // verify the compilation with the added source has no diagnostics
            Assert.True(!outputDiagnostics.Where(d => d.Severity != DiagnosticSeverity.Hidden).Any(),
                System.IO.Path.GetFileName(sourcePath)
                + " - "
                + (outputDiagnostics.FirstOrDefault(d => d.Severity != DiagnosticSeverity.Hidden)?.ToString() ?? "outputDiagnostics"));
            //Assert.True(outputDiagnostics.IsEmpty); 

            {
                // Or we can look at the results directly:
                GeneratorDriverRunResult runResult = driver.GetRunResult();

                // The runResult contains the combined results of all generators passed to the driver
                Assert.True(runResult.GeneratedTrees.Length == 1);
                Assert.True(runResult.Diagnostics.IsEmpty);

                // Or you can access the individual results on a by-generator basis
                GeneratorRunResult generatorResult = runResult.Results[0];
                //Assert.True(generatorResult.Generator == generator);
                Assert.True(generatorResult.Diagnostics.IsEmpty);
                Assert.True(generatorResult.GeneratedSources.Length == 1);
                Assert.True(generatorResult.Exception is null);
            }
           
        }
    }

    private static SyntaxTree? _GlobalUsings;
    //private static SyntaxTree[]? _BaseMethodsSources;
    private static MetadataReference[]? _ArrayMetadataReference;
    private static Compilation CreateCompilation(string source) {
        /*
        if (_BaseMethodsSources is null) {
            _BaseMethodsSources
                = AttributeSourceHelper.getBaseMethodsSources()
                .Select(source => CSharpSyntaxTree.ParseText(source))
                .ToArray();
        }
        var arrSources = _BaseMethodsSources.Concat(
            new SyntaxTree[]{
                CSharpSyntaxTree.ParseText(source)
            }).ToArray();
        */
        if (_GlobalUsings is null) {
            _GlobalUsings = CSharpSyntaxTree.ParseText("""
global using System;
global using System.Collections.Generic;
global using Brimborium.BaseMethods;
""");

        }
        if (_ArrayMetadataReference is null) {

            List<MetadataReference> references = new List<MetadataReference>();
            references.AddRange(Basic.Reference.Assemblies.Net70.References.All);
            var pathBaseMethods = typeof(EquatableAttribute).GetTypeInfo().Assembly.Location;
            var assemblyMetadata = AssemblyMetadata.CreateFromFile(pathBaseMethods);
            references.Add(assemblyMetadata.GetReference());
            _ArrayMetadataReference = references.ToArray();
        }
        return CSharpCompilation.Create(
            "compilation",
              // arrSources,
              new SyntaxTree[]{
                _GlobalUsings,
                CSharpSyntaxTree.ParseText(source)
              },
              _ArrayMetadataReference,
              new CSharpCompilationOptions(
                  outputKind: OutputKind.DynamicallyLinkedLibrary,
                  optimizationLevel: OptimizationLevel.Debug,
                  warningLevel: 9999,
                  allowUnsafe: false,
                  nullableContextOptions: NullableContextOptions.Enable
                  ));
    }
}
