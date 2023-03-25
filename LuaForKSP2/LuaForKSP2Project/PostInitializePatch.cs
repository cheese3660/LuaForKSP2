using HarmonyLib;
using MoonSharp.Interpreter;
using SpaceWarp.Patching.LoadingActions;

namespace LuaForKSP2;

[HarmonyPatch(typeof(PostInitializeModAction),nameof(PostInitializeModAction.DoAction))]
public static class PostInitializePatch
{
    [HarmonyPrefix]
    public static bool OnPostInitializedPrefix(ref PostInitializeModAction __instance, Action resolve, Action<string> reject)
    {try
        {
            __instance._plugin.OnPostInitialized();
            if (LuaForKSP2Plugin.PostInitializedScripts.TryGetValue(__instance._plugin.SpaceWarpMetadata.ModID,
                    out var script))
            {
                script.Call("OnPostInitialized");
            }
            resolve();
        }
        catch (Exception e)
        {
            __instance._plugin.ModLogger.LogError(e.ToString());
            reject(null);
        }
        return false;
    }
}