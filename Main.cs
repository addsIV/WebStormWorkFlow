using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Flow.Launcher.Plugin;

namespace WebStormWorkFlow;

public class Main : IPlugin
{
    private PluginInitContext _context;
    private Dictionary<string, string> _projects;

    public void Init(PluginInitContext context)
    {
        _context = context;
        LoadProjects();
    }

    public List<Result> Query(Query query)
    {
        var results = new List<Result>();
        var keyword = query.Search;

        foreach (var project in _projects)
        {
            if (project.Key.ToLower().Contains(keyword.ToLower()))
            {
                results.Add(new Result
                {
                    Title = project.Key,
                    SubTitle = $"Open {project.Key} with WebStorm",
                    IcoPath = "Images\\icon.png",
                    Action = e =>
                    {
                        OpenProject(project.Value);
                        return true;
                    }
                });
            }
        }

        return results;
    }

    private void LoadProjects()
    {
        // change to your target projects directory
        const string projectsDirectory = @"D:\projects";
        _projects = new Dictionary<string, string>();

        foreach (var dir in Directory.GetDirectories(projectsDirectory))
        {
            var dirName = Path.GetFileName(dir);
            _projects[dirName] = dir;
        }
    }

    private void OpenProject(string path)
    {
        // change to the path of your webstorm executable file
        const string webstormExecutable = @"C:\Users\kevin.lin93\AppData\Local\Programs\oh-my-posh\bin\webstorm.cmd";
        Process.Start(new ProcessStartInfo
        {
            FileName = webstormExecutable,
            Arguments = $"\"{path}\"",
            UseShellExecute = true
        });
    }
}