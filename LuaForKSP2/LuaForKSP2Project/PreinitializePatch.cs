using System.Reflection;
using HarmonyLib;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using SpaceWarp;
using SpaceWarp.API.Mods;
using SpaceWarp.Patching.LoadingActions;
namespace LuaForKSP2;

[HarmonyPatch(typeof(PreInitializeModAction),nameof(PreInitializeModAction.DoAction))]
public static class PreinitializePatch
{
    [HarmonyPrefix]
    public static bool OnPreInitializedPostfix(ref PreInitializeModAction __instance, Action resolve, Action<string> reject)
    {try
        {
            // First we pre-initialize the plugin
            __instance._plugin.OnPreInitialized();
            LoadLuaPlugin(__instance);
            resolve();
        }
        catch (Exception e)
        {
            __instance._plugin.ModLogger.LogError(e.ToString());
            reject(null);
        }
        return false;
    }


    

    private static void LoadLuaPlugin(PreInitializeModAction __instance)
    {

        // Then we register all the functions in the assembly for lua
        UserData.RegisterAssembly(Assembly.GetAssembly(__instance.GetType()));
        // Then we load the lua scripts and initialize them
        var path = __instance._plugin.PluginFolderPath;
        var luaName = Path.Combine(path, "plugin.lua");
        if (!File.Exists(luaName)) return;
        var script = new Script
        {
            Options =
            {
                ScriptLoader = new FileSystemScriptLoader()
            },
            Globals =
            {
                ["Plugin"] = __instance._plugin,
                ["Logger"] = __instance._plugin.ModLogger,
                ["Info"] = __instance._plugin.Info,
                ["Path"] = __instance._plugin.PluginFolderPath
            }
        };
        script.DoFile(luaName);
        if (!Equals(script.Globals["OnPreInitialized"], DynValue.Nil))
        {
            script.Call(script.Globals["OnPreInitialized"]);
        }

        if (!Equals(script.Globals["OnInitialized"], DynValue.Nil))
        {
            LuaForKSP2Plugin.InitializedScripts.Add(__instance._plugin.SpaceWarpMetadata.ModID, script);
        }

        if (!Equals(script.Globals["OnPostInitialized"], DynValue.Nil))
        {
            LuaForKSP2Plugin.PostInitializedScripts.Add(__instance._plugin.SpaceWarpMetadata.ModID, script);
        }

        if (!Equals(script.Globals["Update"], DynValue.Nil))
        {
            LuaForKSP2Plugin.UpdateLoopScripts.Add(script);
        }

        if (!Equals(script.Globals["FixedUpdate"], DynValue.Nil))
        {
            LuaForKSP2Plugin.FixedUpdateScripts.Add(script);
        }

        if (!Equals(script.Globals["LateUpdate"], DynValue.Nil))
        {
            LuaForKSP2Plugin.LateUpdateScripts.Add(script);
        }

        if (!Equals(script.Globals["OnGUI"], DynValue.Nil))
        {
            LuaForKSP2Plugin.GUIScripts.Add(script);
        }
    }
}