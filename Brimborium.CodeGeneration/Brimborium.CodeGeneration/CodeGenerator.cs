namespace Brimborium.CodeGeneration {
    public class CodeGenerator {
        public static bool Generate(
            string outputFolder,
            string pattern,
            Dictionary<string, string> templateVariables,
            CodeGeneratorBindings codeGeneratorBindings,
            Action<string>? log,
            bool isVerbose) {
            templateVariables["ProjectRoot"] = outputFolder;
            var di = new System.IO.DirectoryInfo(outputFolder);
            var arrFileInfo = di.GetFiles(pattern, System.IO.SearchOption.AllDirectories);

            return CodeGenerator.Generate(
                arrFileInfo: arrFileInfo,
                templateVariables: templateVariables,
                codeGeneratorBindings: codeGeneratorBindings,
                log: log,
                isVerbose: isVerbose
                );
        }

        public static bool Generate(
            FileInfo[] arrFileInfo,
            Dictionary<string, string> templateVariables,
            CodeGeneratorBindings codeGeneratorBindings,
            Action<string>? log,
            bool isVerbose
            ) {
            if (log is null) { log = System.Console.Out.WriteLine; }

            var lstFileContent = arrFileInfo
                            .Select(fi => new FileContent(fi.FullName, System.IO.File.ReadAllText(fi.FullName)))
                            .OrderBy(fi => fi.FileName)
                            .ToList();
            /*
            foreach (var fc in lstFileContent) {
                log(fc.FileName);
            }
            */
            var lstFileContentGenerated = new List<FileContent>();
            var lstFileContentReplacements = new List<FileContent>();
            foreach (var fc in lstFileContent) {
                if (ReplacementBindingExtension.ContainsReplace(fc.Content)) {
                    lstFileContentReplacements.Add(fc);
                } else {
                    lstFileContentGenerated.Add(fc);
                }
            }

            var result = false;
            var dctFileContentGenerated = lstFileContentGenerated.ToDictionary(fc => fc.FileName, StringComparer.OrdinalIgnoreCase);
            var dctFileContentReplacements = lstFileContentReplacements.ToDictionary(fc => fc.FileName, StringComparer.OrdinalIgnoreCase);
            // var lstFileContent = new List<FileContent>(lstFileContentGenerated.Count + lstFileContentReplacements.Count);
            // lstFileContent.AddRange(lstFileContentGenerated);
            // lstFileContent.AddRange(lstFileContentReplacements.Where(fc=>!fc.Content.Contains("-- Replace=SNIPPETS -")));

            System.Exception? lastError = null;
            var renderGenerator = new RenderGenerator(codeGeneratorBindings.ReplacementBindings, templateVariables);
            foreach (var renderBinding in codeGeneratorBindings.RenderBindings) {
                var boundVariables = new Dictionary<string, string>(templateVariables);
                var outputFilename = renderBinding.GetFilename(boundVariables);
                if (string.IsNullOrEmpty(outputFilename)) {
                    continue;
                }
                if (dctFileContentReplacements.ContainsKey(outputFilename)) {
                    log($"skip: {outputFilename}");
                    continue;
                }
                dctFileContentGenerated.TryGetValue(outputFilename, out var fileContentGenerated);
                var codeIdentity = renderBinding.GetCodeIdentity();
                if (!string.IsNullOrEmpty(codeIdentity)) {
                    var lstFileContentFound = lstFileContent.Where(fc => fc.Content.Contains(codeIdentity, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (lstFileContentFound.Count == 0) {
                    } else if (lstFileContentFound.FirstOrDefault(fc => string.Equals(fc.FileName, outputFilename, StringComparison.CurrentCultureIgnoreCase) ) is FileContent fileContentAsDefined ) {
                        // no change
                    } else if (lstFileContentFound.Count == 1) {
                        var fileContentFound = lstFileContentFound[0];
                        if (isVerbose) {
                            log($"redirect {outputFilename}->{fileContentFound.FileName}");
                        }
                        outputFilename = fileContentFound.FileName;
                        dctFileContentGenerated.TryGetValue(outputFilename, out fileContentGenerated);
                    } else {
                        log($"Error: codeIdentity:{codeIdentity} resolves to mmore than one file: {string.Join(", ", lstFileContent.Select(fc=>fc.FileName))}");
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
                    var sbOutput = new StringBuilder();
                    var printCtxt = new PrintContext(sbOutput, boundVariables);
                    renderBinding.Render(printCtxt);
                    var (changed, content) = ReplacementBindingExtension.Replace(sbOutput.ToString(), renderGenerator.GetValue);
                    if (changed) {
                        log($"changed: {outputFilename}");
                        result = true;
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
                }
            }

            foreach (var fileContent in lstFileContentReplacements) {
                try {
                    var outputFilename = fileContent.FileName;
                    var (changed, content) = ReplacementBindingExtension.Replace(fileContent.Content, renderGenerator.GetValue);
                    if (changed) {
                        if (fileContent.Write(content).changed) {
                            log($"changed: {outputFilename}");
                            result = true;
                        }
                    } else {
                        if (isVerbose) {
                            log($"ok     : {outputFilename}");
                        }
                    }
                } catch (System.Exception error) {
                    lastError = error;
                    log($"error     : {fileContent.FileName}");
                    log(error.ToString());
                    log($"error     : {fileContent.FileName}");
                }
                if (lastError is not null) {
                    throw lastError;
                }
            }
            return result;
        }

    }
}
