namespace Brimborium.Details;

public class SolutionInfoConfiguration {
    public SolutionInfoConfiguration() {
        this.ListMainProjectName = new List<string>();
        this.ListMainProjectInfo = new List<ProjectInfo>();
        this.ListProject = new List<ProjectInfo>();
    }
    public string? RootPath { get; set; }
    public string? SolutionFilePath { get; set; }
    public string? DetailPath { get; set; }
    public List<string> ListMainProjectName { get; set; }
    public List<ProjectInfo> ListMainProjectInfo { get; set; }
    public List<ProjectInfo> ListProject { get; set; }
}

public record SolutionInfo(
     string RootPath,
     string SolutionFilePath,
     string DetailPath
) {
    public List<string> ListMainProjectName { get; set; } = new List<string>();
    public List<ProjectInfo> ListMainProjectInfo { get; set; } = new List<ProjectInfo>();
    public List<ProjectInfo> ListProject { get; set; } = new List<ProjectInfo>();

    public string GetRelativePath(string documentFilePath) {
        if (documentFilePath.StartsWith(this.RootPath)) {
            return GetNormalizedPath(documentFilePath.Substring(this.RootPath.Length + 1));
        } else {
            return GetNormalizedPath(System.IO.Path.GetRelativePath(this.RootPath, documentFilePath));
        }
    }

    public string GetFullPath(string documentFilePath) {
        return System.IO.Path.GetFullPath(
            System.IO.Path.Combine(
                this.RootPath,
                GetOsPath(documentFilePath)))
            ?? throw new System.Exception($"GetFullPath failed for {documentFilePath}");
    }

    public static string GetOsPath(string documentFilePath) {
        return documentFilePath.Replace('/', System.IO.Path.DirectorySeparatorChar);
    }

    public static string GetNormalizedPath(string documentFilePath) {
        return documentFilePath.Replace(System.IO.Path.DirectorySeparatorChar, '/');
    }

    public SolutionInfo PostLoad(string detailJsonDirectoryPath) {
        System.Console.Out.WriteLine($"detailJsonDirectoryPath: {detailJsonDirectoryPath}");

        var rootPath = string.IsNullOrEmpty(this.RootPath)
            ? detailJsonDirectoryPath
            : System.IO.Path.Combine(detailJsonDirectoryPath, this.RootPath)
            ?? throw new InvalidOperationException();
        System.Console.Out.WriteLine($"rootPath: {rootPath}");

        var thisRootPath = this with { RootPath = rootPath };

        var result = thisRootPath with {
            SolutionFilePath = thisRootPath.GetFullPath(thisRootPath.SolutionFilePath),
            DetailPath = thisRootPath.GetFullPath(thisRootPath.DetailPath),
        };
        return result;
    }

    public SolutionInfo PreSave(string detailJsonFullPath) {
        var detailDirectoryPath = System.IO.Path.GetDirectoryName(detailJsonFullPath)
            ?? throw new InvalidOperationException();

        var rootPath = string.Equals(detailDirectoryPath, this.RootPath, StringComparison.InvariantCultureIgnoreCase)
            ? string.Empty
            : System.IO.Path.GetRelativePath(detailDirectoryPath, this.RootPath);

        var result = this with {
            RootPath = rootPath,
            SolutionFilePath = this.GetRelativePath(this.SolutionFilePath),
            DetailPath = this.GetRelativePath(this.DetailPath),
        };
        return result;
    }
}

public record ProjectInfo(
    string Name,
    string FilePath,
    string Language,
    string FolderPath
) {
    public List<DocumentInfo> Documents { get; set; } = new List<DocumentInfo>();
}

public record DocumentInfo(
    string FilePath,
    string Language
);

public record SourceCodeMatch(
    string FilePath,
    int Index,
    int Line,
    MatchInfo Match,
    SourceCodeMatchCSContext? CSContext = null
);

public record MatchInfo(
    string MatchingText,
    bool IsCommand,
    string[] Parts
) {
    protected virtual bool PrintMembers(StringBuilder stringBuilder)
    {
        stringBuilder.Append($"MatchingText = \"{MatchingText}\", IsCommand = {IsCommand}, ");
        stringBuilder.Append("Parts = [");
        for(int idx=0;idx<Parts.Length;idx++){
            if (idx>0) {
                stringBuilder.Append(", ");
            }
            stringBuilder.Append("\"").Append(Parts[idx]).Append("\"");
        }
        stringBuilder.Append("]");
        return true;
    }
}

public record SourceCodeMatchCSContext(
    string FilePath,
    int Line,
    string FullName,
    string? Namespace,
    string? Type,
    string? Method
);