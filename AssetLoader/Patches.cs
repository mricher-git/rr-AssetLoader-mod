using AssetLoader.UMM;
using AssetPack.Runtime;
using HarmonyLib;
using Helpers;
using Model.Database;
using System.Collections.Generic;
using System.IO;
using UnityModManagerNet;

namespace AssetLoader.Patches;

[HarmonyPatch(typeof(AssetPackRuntimeStore), nameof(AssetPackRuntimeStore.BasePathForLocation))]
static class BasePathForLocationPatch
{
	private static bool Prefix(AssetPackRuntimeStore.StoreLocation location, ref string __result)
	{
		if ((int)location == 2)
		{
			__result = UnityModManager.modsPath;
			return false;
		}

		return true;
	}
}

[HarmonyPatch]
static class AssetLoaderPatch
{
	internal static Dictionary<string, string> tenderSwaps = new();

	[HarmonyPatch(typeof(Model.Database.PrefabStore), nameof(Model.Database.PrefabStore.CheckDefinitions))]
	[HarmonyPrefix]
	private static bool CheckDefinitions(PrefabStore __instance)
	{
		Loader.Log("Looking for assets to load...");

		foreach (var mod in UnityModManager.modEntries)
		{
			if (!mod.Enabled) continue;

			if (File.Exists(Path.Combine(mod.Path, "Catalog.json")))
			{
				Loader.Log("Adding: " + mod.Info.Id);
				var store = new AssetPackRuntimeStore(mod.Info.Id, (AssetPackRuntimeStore.StoreLocation)2);
				__instance._stores.Add(store);
			}

			foreach (var folder in Directory.GetDirectories(mod.Path))
			{
				if (File.Exists(Path.Combine(folder, "Catalog.json")))
				{
					Loader.Log("Adding: " + Path.Combine(mod.Info.Id, Path.GetFileName(folder)));
					var store = new AssetPackRuntimeStore(Path.Combine(mod.Info.Id, Path.GetFileName(folder)), (AssetPackRuntimeStore.StoreLocation)2);
					__instance._stores.Add(store);
				}
				else if (File.Exists(Path.Combine(folder, "Definitions.json")))
				{
					tenderSwaps.Add(Path.GetFileName(folder), folder);
				}
			}
		}

		return true;
	}

	[HarmonyPatch(typeof(AssetPackRuntimeStore), nameof(AssetPackRuntimeStore.DefinitionsPath), MethodType.Getter)]
	[HarmonyPrefix]
	private static bool DefinitionsPath(AssetPackRuntimeStore __instance, ref string __result)
	{
		if (tenderSwaps.TryGetValue(__instance.Identifier, out string path))
		{
			__result = Path.Combine(path, "Definitions.json");
			return false;
		}

		return true;
	}

	[HarmonyPatch(typeof(SceneryAssetManager), nameof(SceneryAssetManager.OnDestroy))]
	[HarmonyPrefix]
	private static bool OnDestroy()
	{
		tenderSwaps.Clear();

		return true;
	}
}
