using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Flow.Launcher.Plugin;
using WebStormWorkFlow.Models;

namespace WebStormWorkFlow;

public class Main : IPlugin
{
    private PluginInitContext? _context;
    private Dictionary<string, string>? _projects;
    private Config? _config;

    public void Init(PluginInitContext context)
    {
        _context = context;
        _context.API.LogInfo("WebStormWorkFlow", "Plugin initialization started.");
        
        LoadConfig(); 
        _context.API.LogInfo("WebStormWorkFlow", "Configuration loaded successfully.");
        
        LoadProjects();  
        _context.API.LogInfo("WebStormWorkFlow", "Projects loaded successfully.");
    }

    public List<Result> Query(Query query)
    {
        var results = new List<Result>();
        var keyword = query.Search;

        foreach (var project in _projects!)
        {
            if (project.Key.ToLower().Contains(keyword.ToLower()))
            {
                results.Add(new Result
                {
                    Title = project.Key,
                    SubTitle = $"Open {project.Key} with WebStorm",
                    IcoPath = "Images\\icon.png",
                    Action = _ =>
                    {
                        OpenProject(project.Value);
                        return true;
                    }
                });
            }
        }

        return results;
    }
    
    private void LoadConfig()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configFilePath = Path.Combine(appDataPath, "FlowLauncher", "Plugins", "WebstormWorkFlow", "config.json");
       
        if (File.Exists(configFilePath))
        {
            var configJson = File.ReadAllText(configFilePath);
            _config = JsonSerializer.Deserialize<Config>(configJson)!;
        }
        else
        {
            throw new FileNotFoundException("WebstormWorkFlow: Configuration file not found.");
        }
    }

    private void LoadProjects()
    {
        _projects = new Dictionary<string, string>();

        foreach (var dir in Directory.GetDirectories(_config!.ProjectsDirectory))
        {
            var dirName = Path.GetFileName(dir);
            _projects[dirName] = dir;
        }
    }

    private void OpenProject(string path)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = _config!.WebstormExecutable,
            Arguments = $"\"{path}\"",
            UseShellExecute = true
        });
    }
}