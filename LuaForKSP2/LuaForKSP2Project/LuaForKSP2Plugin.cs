using BepInEx;
using HarmonyLib;
using KSP.UI.Binding;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.Game;
using SpaceWarp.API.Game.Extensions;
using SpaceWarp.API.UI;
using SpaceWarp.API.UI.Appbar;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace LuaForKSP2;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class LuaForKSP2Plugin : BaseSpaceWarpPlugin
{
    // These are useful in case some other mod wants to add a dependency to this one
    public const string ModGuid = MyPluginInfo.PLUGIN_GUID;
    public const string ModName = MyPluginInfo.PLUGIN_NAME;
    public const string ModVer = MyPluginInfo.PLUGIN_VERSION;
    public void Awake()
    {
        UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
        Harmony.CreateAndPatchAll(typeof(LuaForKSP2Plugin).Assembly);
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            UserData.RegisterAssembly(assembly, true);
        }
    }
    
    // Time for all the static stuff
    internal static List<Script> UpdateLoopScripts = new();
    internal static List<Script> GUIScripts = new();
    internal static List<Script> FixedUpdateScripts = new();
    internal static List<Script> LateUpdateScripts = new();

    internal static Dictionary<string, Script> InitializedScripts = new();

    internal static Dictionary<string, Script> PostInitializedScripts = new();

    public void Update()
    {
        foreach (var script in UpdateLoopScripts)
        {
            script.Call(script.Globals["Update"]);
        }
    }

    public void OnGUI()
    {
        foreach (var script in GUIScripts)
        {
            script.Call(script.Globals["OnGUI"]);
        }
    }

    public void FixedUpdate()
    {
        foreach (var script in FixedUpdateScripts)
        {
            script.Call(script.Globals["FixedUpdate"]);
        }
    }

    public void LateUpdate()
    {
        foreach (var script in LateUpdateScripts)
        {
            script.Call(script.Globals["LateUpdate"]);
        }
    }
}
