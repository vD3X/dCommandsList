using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using MenuManager;
using Microsoft.Extensions.Logging;

namespace dCommandsList; 
 
public class dCommandsList : BasePlugin 
{ 
    public override string ModuleName => "[CS2] D3X - [ Lista Komend ]";
    public override string ModuleAuthor => "D3X";
    public override string ModuleDescription => " Plugin dodaje na serwer liste komend.";
    public override string ModuleVersion => "1.0.0";
    public static dCommandsList Instance { get; private set; } = new dCommandsList();

    public IMenuApi? _api;
    private readonly PluginCapability<IMenuApi?> _pluginCapability = new("menu:nfcore");   

    public override void Load(bool hotReload) {
        Instance = this;
        Config.Initialize();
        Command.Load();
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _api = _pluginCapability.Get();
        if (_api == null) Logger.LogWarning("MenuManager Core nie znaleziono...");
    }
} 
