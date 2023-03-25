using HarmonyLib;
using SpaceWarp.Patching.LoadingActions;

namespace LuaForKSP2;

[HarmonyPatch(typeof(InitializeModAction),nameof(InitializeModAction.DoAction))]
public static class InitializePatch
{
    [HarmonyPrefix]
    public static bool OnPostInitializedPrefix(ref InitializeModAction __instance, Action resolve, Action<string> reject)
    {try
        {
            __instance._plugin.OnInitialized();
            if (LuaForKSP2Plugin.InitializedScripts.TryGetValue(__instance._plugin.SpaceWarpMetadata.ModID,
                    out var script))
            {
                script.Call("OnInitialized");
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