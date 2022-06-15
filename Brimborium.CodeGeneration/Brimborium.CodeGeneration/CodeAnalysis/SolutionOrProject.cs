namespace Brimborium.CodeGeneration.CodeAnalysis;

public class SolutionOrProject {
    public MSBuildWorkspace Workspace;
    public Solution? Solution;
    public Project? Project;
    private readonly Dictionary<ProjectId, ProjectData> _ProjectDatas;

    public SolutionOrProject(
        MSBuildWorkspace msWorkspace,
        Solution solution
        ) {
        this._ProjectDatas = new Dictionary<ProjectId, ProjectData>();
        this.Workspace = msWorkspace;
        this.Solution = solution;
    }

    public SolutionOrProject(
        MSBuildWorkspace msWorkspace,
        Project project
        ) {
        this._ProjectDatas = new Dictionary<ProjectId, ProjectData>();
        this.Workspace = msWorkspace;
        this.Project = project;
    }

    public List<Project> GetAllProjects() {
        var result = new List<Project>();
        if (this.Solution is Solution solution) {
            result.AddRange(
                solution.Projects
                );
        } else if (this.Project is Project project) {
            result.Add(project);
        }
        return result;
    }


    public ProjectData GetProjectData(Project project, bool noCache = false) {
        if (!noCache) {
            if (this._ProjectDatas.TryGetValue(project.Id, out var result)) {
                return result;
            }
        }
        {
            var result = new ProjectData(project);
            this._ProjectDatas[result.Id] = result;
            return result;
        }
    }

    /*
    public static async Task Gna() {
    string solutionPath = @"C:\play\Brimborium.CodeAnalyzsis\Brimborium.CodeAnalyzsis.sln";
    var msWorkspace = MSBuildWorkspace.Create();

    //var project1 = await msWorkspace.OpenProjectAsync(solutionPath);

    var solution = await msWorkspace.OpenSolutionAsync(solutionPath);

    foreach (var project in solution.Projects) {
       foreach (var document in project.Documents) {
           Console.WriteLine(project.Name + "\t\t\t" + document.Name);
       }
    }
    }
    */

    public static async Task<SolutionOrProject> LoadAsync(string filePath) {
        if (filePath.EndsWith(".sln")) {
            return await LoadSolutionAsync(filePath);
        }
        if (filePath.EndsWith(".csproj")) {
            return await LoadProjectAsync(filePath);
        }
        throw new ArgumentException(".sln or .csproj file expected.");
    }

    public static async Task<SolutionOrProject> LoadSolutionAsync(string solutionPath) {
        var msWorkspace = MSBuildWorkspace.Create();
        var solution = await msWorkspace.OpenSolutionAsync(solutionPath);
        return new SolutionOrProject(msWorkspace, solution);
    }

    public static async Task<SolutionOrProject> LoadProjectAsync(string projectPath) {
        var msWorkspace = MSBuildWorkspace.Create();
        var project = await msWorkspace.OpenProjectAsync(projectPath);
        return new SolutionOrProject(msWorkspace, project);
    }
}
