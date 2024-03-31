using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace AssetLoader.UMM;

#if DEBUG
[EnableReloading]
#endif
public static class Loader
{
	public static UnityModManager.ModEntry ModEntry { get; private set; }
	public static Harmony HarmonyInstance { get; private set; }
	
	//internal static AssetLoaderSettings Settings;
	
	private static bool Load(UnityModManager.ModEntry modEntry)
	{
		if (ModEntry != null)
		{
			modEntry.Logger.Warning("AssetLoader is already loaded!");
			return false;
		}

		ModEntry = modEntry;
		//Settings = UnityModManager.ModSettings.Load<AssetLoaderSettings>(modEntry);
		ModEntry.OnUnload = Unload;
		ModEntry.OnToggle = OnToggle;
		//ModEntry.OnGUI = OnGUI;
		//ModEntry.OnSaveGUI = Settings.Save;

		HarmonyInstance = new Harmony(modEntry.Info.Id);
		return true;
	}
	
	public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
	{
		if (value)
		{
			try
			{
				HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			}
			catch (Exception ex)
			{
				modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
				HarmonyInstance?.UnpatchAll(modEntry.Info.Id);

				return false;
			}
		}
		else
		{
			HarmonyInstance.UnpatchAll(modEntry.Info.Id);
		}

		return true;
	}

	private static bool Unload(UnityModManager.ModEntry modEntry)
	{
		return true;
	}

	/*public class AssetLoaderSettings : UnityModManager.ModSettings, IDrawable
	{
		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}

		public void OnChange()
		{
			Instance?.OnSettingsChanged();
		}
	}*/
	
	/*private static void OnGUI(UnityModManager.ModEntry modEntry)
	{
		Settings.Draw(modEntry);
	}*/
	
		public static void Log(string str)
	{
		ModEntry?.Logger.Log(str);
	}

	public static void LogDebug(string str)
	{
#if DEBUG
		ModEntry?.Logger.Log(str);
#endif
	}
}
