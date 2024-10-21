using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Eremite;
using Eremite.Characters.Villagers;
using Eremite.Controller;
using HarmonyLib;
using UnityEngine;
using UniverseLib;
using UniverseLib.UI;

namespace ATS.RenameVillager
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        private Harmony harmony;

        private static PluginConfig _pluginConfig;
        private static Dictionary<int, Villager> _villagersServiceVillagers = new();
        public static void LogInfo(object obj) => Instance.Logger.LogInfo(obj);

        public static void UniverseLog(string obj, LogType lt) =>
            Instance.Logger.LogInfo($"UniverseLog [{lt}]: " + obj);
        // private static Dictionary<ConfigEntry<KeyboardShortcut>, Action> _renameVillagersShortcuts = new();

        public static UIBase OverlayUiBase { get; private set; }
        internal static VillagerUIPanel panel;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loading...");

            Instance = this;
            harmony = Harmony.CreateAndPatchAll(typeof(Plugin));

            _pluginConfig = new PluginConfig();
            Universe.Init(onInitialized: () => { }, logHandler: UniverseLog);

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(GameController), nameof(GameController.StartGame))]
        [HarmonyPostfix]
        private static void HookEveryGameStart()
        {
            _villagersServiceVillagers = GameController.Instance.GameServices.VillagersService.Villagers;

            if (OverlayUiBase == null)
            {
                OverlayUiBase = UniversalUI.RegisterUI<UIBase>(PluginInfo.PLUGIN_GUID, null);
            }

            if (panel == null)
            {
                LogInfo($" Init panel");
                var hotkey = _pluginConfig.ShowPanel.Value.ToString();
                panel = new VillagerUIPanel(OverlayUiBase, _villagersServiceVillagers, hotkey);
            }
            else
            {
                panel.setVillagers(_villagersServiceVillagers);
            }

            panel.SetActive(true);
            panel.FillPanel();

        }

        [HarmonyPatch(typeof(GameController), nameof(GameController.OnDestroy))]
        [HarmonyPrefix]
        private static bool HookEveryGameStop()
        {
            _villagersServiceVillagers = new();

            panel.SetActive(false);
            return true;
        }

        internal void Update()
        {
            if (!OverlayUiBase?.RootObject)
                return;

            if (!GameController.IsGameActive || MB.InputService.IsLocked())
                return;
            var shortcutAction = GetShortcutAction();
            if (shortcutAction != null)
            {
                shortcutAction.Invoke();
            }
        }

        private static Action GetShortcutAction()
        {
            var matchingEntry = _pluginConfig.renameVillagersShortcuts
                .FirstOrDefault(entry => entry.Key.Value.IsDown());
            return matchingEntry.Value;
        }

        public static void ShowPanel()
        {
            if (panel != null)
            {
                panel.ShowPanel();
            }
        }
    }
}