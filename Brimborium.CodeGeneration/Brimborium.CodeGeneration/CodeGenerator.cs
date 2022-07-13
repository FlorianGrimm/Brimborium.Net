namespace Brimborium.CodeGeneration {
    public class CodeGenerator {
        public static bool Generate(
            string outputFolder,
            string pattern,
            Dictionary<string, string> templateVariables,
            CodeGeneratorBindings codeGeneratorBindings,
            Action<string>? log,
            bool isVerbose,
            IFileContentService? fileContentService = default) {
            templateVariables["ProjectRoot"] = outputFolder;
            var di = new System.IO.DirectoryInfo(outputFolder);
            var arrFileInfo = di.GetFiles(pattern, System.IO.SearchOption.AllDirectories);
            var lstFileContent = arrFileInfo
                .Select(fi => FileContent.Create(fi.FullName, fileContentService))
                .OrderBy(fi => fi.FileName)
                .ToList();

            return CodeGenerator.Generate(
                outputFolder: outputFolder,
                lstFileContent: lstFileContent,
                templateVariables: templateVariables,
                codeGeneratorBindings: codeGeneratorBindings,
                log: log,
                isVerbose: isVerbose,
                fileContentService: fileContentService
                );
        }

        public static bool Generate(
            string outputFolder,
            string outputFolderCache,
            string pattern,
            Dictionary<string, string> templateVariables,
            CodeGeneratorBindings codeGeneratorBindings,
            Action<string>? log,
            bool isVerbose,
            IFileContentService? fileContentService = default) {
            templateVariables["ProjectRoot"] = outputFolderCache;
            var diCache = new System.IO.DirectoryInfo(outputFolderCache);
            if (!diCache.Exists) {
                diCache.Create();
            }
            var arrFileInfoCache = diCache.GetFiles(pattern, System.IO.SearchOption.AllDirectories);
            var lstFileContentCache = arrFileInfoCache
                .Select(fi => FileContent.Create(fi.FullName, fileContentService))
                .OrderBy(fi => fi.FileName)
                .ToList();

            var resultChanges = CodeGenerator.GenerateWithChanges(
                outputFolder: outputFolderCache,
                lstFileContent: lstFileContentCache,
                templateVariables: templateVariables,
                codeGeneratorBindings: codeGeneratorBindings,
                log: log,
                isVerbose: isVerbose,
                fileContentService: fileContentService
                );

            var result = resultChanges.Any(t => t.changed);
            foreach (var (_, fileContentCache) in resultChanges) {
                var fileNameRelative = fileContentCache.FileName.Substring(outputFolderCache.Length);
                var fileNameOutput = outputFolder + fileNameRelative;
                var fileContentOutput = FileContent.Create(fileNameOutput, fileContentService);
                if (fileContentOutput.Write(fileContentCache.Content).changed) {
                    result = true;
                }
            }

            return result;
        }

        public static bool Generate(
            string outputFolder,
            List<FileContent> lstFileContent,
            Dictionary<string, string> templateVariables,
            CodeGeneratorBindings codeGeneratorBindings,
            Action<string>? log,
            bool isVerbose,
            IFileContentService? fileContentService = default
            ) {
            var result = GenerateWithChanges(
                outputFolder,
                lstFileContent,
                templateVariables,
                codeGeneratorBindings,
                log,
                isVerbose,
                fileContentService);
            return result.Any(t => t.changed);
        }

        public static List<(bool changed, FileContent result)> GenerateWithChanges(
            string outputFolder,
            List<FileContent> lstFileContent,
            Dictionary<string, string> templateVariables,
            CodeGeneratorBindings codeGeneratorBindings,
            Action<string>? log,
            bool isVerbose,
            IFileContentService? fileContentService = default
            ) {
            if (log is null) { log = System.Console.Out.WriteLine; }
            var result = new List<(bool changed, FileContent result)>();
            System.Exception? lastError = null;
            var lstFileContentGenerated = new List<FileContent>();
            var lstFileContentReplacements = new List<FileContent>();
            foreach (var fc in lstFileContent) {
                if (ReplacementBindingExtension.ContainsReplace(fc.Content)) {
                    lstFileContentReplacements.Add(fc);
                } else {
                    lstFileContentGenerated.Add(fc);
                }
            }

            var dctFileContentGenerated = lstFileContentGenerated.ToDictionary(fc => fc.FileName, StringComparer.OrdinalIgnoreCase);
            var dctFileContentReplacements = lstFileContentReplacements.ToDictionary(fc => fc.FileName, StringComparer.OrdinalIgnoreCase);

            var renderGenerator = new RenderGenerator(codeGeneratorBindings.ReplacementBindings, templateVariables);
            foreach (var renderBinding in codeGeneratorBindings.RenderBindings) {
                var boundVariables = new Dictionary<string, string>(templateVariables);
                var outputFilename = renderBinding.GetFilename(boundVariables);
                if (string.IsNullOrEmpty(outputFilename)) {
                    continue;
                }
                if (!string.IsNullOrEmpty(outputFolder) && !System.IO.Path.IsPathFullyQualified(outputFilename)) {
                    outputFilename = System.IO.Path.GetFullPath(System.IO.Path.Combine(outputFolder, outputFilename));
                }
                if (dctFileContentReplacements.ContainsKey(outputFilename)) {
                    log($"skip: {outputFilename}");
                    continue;
                }
                dctFileContentGenerated.TryGetValue(outputFilename, out var fileContentGenerated);
                var codeIdentity = renderBinding.GetCodeIdentity();
                if (!string.IsNullOrEmpty(codeIdentity)) {
                    var lstFileContentFound = lstFileContent
                        .Where(fc => fc.Content.Contains(codeIdentity, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    if (lstFileContentFound.Count == 0) {
                        //
                    } else if (lstFileContentFound.FirstOrDefault(fc => string.Equals(fc.FileName, outputFilename, StringComparison.CurrentCultureIgnoreCase)) is FileContent fileContentAsDefined) {
                        // no change
                    } else if (lstFileContentFound.Count == 1) {
                        var fileContentFound = lstFileContentFound[0];
                        if (isVerbose) {
                            log($"redirect {outputFilename}->{fileContentFound.FileName}");
                        }
                        outputFilename = fileContentFound.FileName;
                        dctFileContentGenerated.TryGetValue(outputFilename, out fileContentGenerated);
                    } else {
                        log($"Error: codeIdentity:{codeIdentity} resolves to mmore than one file: {string.Join(", ", lstFileContent.Select(fc => fc.FileName))}");
                    }
                }

                {
                    if (dctFileContentReplacements.TryGetValue(outputFilename, out var fileContentReplacements)) {
                        // file contains Replacements - do not generate 
                        // lstFileContentReplacements.Remove(fileContentReplacements);
                        if (isVerbose) {
                            log($"info: file:{outputFilename}; change from generate to replacements.");
                        }
                        continue;
                    }
                }
                try {
                    if (fileContentGenerated is null) {
                        fileContentGenerated = FileContent.Create(outputFilename, fileContentService);
                    }
                    var flags = ReplacementBindingExtension.ReadFlags(fileContentGenerated.Content);
                    if (flags.TryGetValue("AutoGenerate", out var autoGenerate)) {
                        if (string.Equals(autoGenerate, "off", StringComparison.Ordinal)) {
                            if (flags.TryGetValue("Replacements", out var replacements)) {
                                if (string.Equals(replacements, "on", StringComparison.Ordinal)) {
                                    if (dctFileContentReplacements.TryAdd(fileContentGenerated.FileName, fileContentGenerated)) {
                                        lstFileContentReplacements.Add(fileContentGenerated);
                                    }
                                    continue;
                                }
                            }
                            continue;
                        }
                    }
                    var customize = ReplacementBindingExtension.ReadCustomize(fileContentGenerated.Content);
                    if (!flags.ContainsKey("AutoGenerate")) {
                        flags["AutoGenerate"] = "on";
                    }
                    if (!flags.ContainsKey("Customize")) {
                        flags["Customize"] = "off";
                    }
                    var sbOutput = new StringBuilder();
                    var printCtxt = new PrintContext(sbOutput, boundVariables, flags, customize);
                    renderBinding.Render(printCtxt);
                    var content = ReplacementBindingExtension.Replace(sbOutput.ToString(), renderGenerator.GetValue).content;
                    var fileContentGeneratedNext = fileContentGenerated.Write(content);
                    result.Add(fileContentGeneratedNext);

                    if (fileContentGeneratedNext.changed) {
                        log($"changed: {outputFilename}");
                    } else {
                        if (isVerbose) {
                            log($"ok     : {outputFilename}");
                        }
                    }
                } catch (System.Exception error) {
                    lastError = error;
                    log($"error     : {outputFilename}");
                    log(error.ToString());
                    log($"error     : {outputFilename}");
                    lastError = error;
                }
            }

            foreach (var fileContent in lstFileContentReplacements) {
                try {
                    var outputFilename = fileContent.FileName;
                    var flags = ReplacementBindingExtension.ReadFlags(fileContent.Content);
                    if (flags.TryGetValue("Replacements", out var replacements)) {
                        if (string.Equals(replacements, "off", StringComparison.Ordinal)) {
                            continue;
                        }
                    }
                    var (changed, content) = ReplacementBindingExtension.Replace(fileContent.Content, renderGenerator.GetValue);
                    if (changed) {
                        var fileContentGeneratedNext = fileContent.Write(content);
                        if (fileContentGeneratedNext.changed) {
                            log($"changed: {outputFilename}");
                        }
                        result.Add(fileContentGeneratedNext);
                    } else {
                        if (isVerbose) {
                            log($"ok     : {outputFilename}");
                        }
                        result.Add((false, fileContent));
                    }
                } catch (System.Exception error) {
                    lastError = error;
                    log($"error     : {fileContent.FileName}");
                    log(error.ToString());
                    log($"error     : {fileContent.FileName}");
                    lastError = error;
                }
            }
            if (lastError is not null) {
                //throw lastError;
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(lastError).Throw();
            }
            return result;
        }

    }
}
