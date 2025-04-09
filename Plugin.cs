using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using Eremite;
using Eremite.Buildings;
using Eremite.Characters.Behaviours;
using Eremite.Characters.Villagers;
using Eremite.Controller;
using Eremite.Model.Sound;
using Eremite.Services;
using HarmonyLib;
using UnityEngine;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.Input;
using UniverseLib.UI;

namespace ATS.RenameVillager
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        private Harmony harmony;

        // private static PluginConfig _pluginConfig;
        private static Dictionary<int, Villager> _villagersServiceVillagers = new();
        public static void LogInfo(object obj) => Instance.Logger.LogInfo(obj);

        public static void UniverseLog(string obj, LogType lt) =>
            Instance.Logger.LogInfo($"UniverseLog [{lt}]: " + obj);
        // private static Dictionary<ConfigEntry<KeyboardShortcut>, Action> _renameVillagersShortcuts = new();

        public static UIBase OverlayUiBase { get; private set; }
        internal static VillagerUIPanel vPanel;
        internal static BuildingsUIPanel bPanel;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loading...");

            Instance = this;
            harmony = Harmony.CreateAndPatchAll(typeof(Plugin));
            
            var iniPath = Path.Combine(Paths.ConfigPath, "opinions.ini");
            OpinionsManager.LoadOpinions(iniPath);
            Logger.LogInfo($"Loaded {OpinionsManager.getTotals()}");
            
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

            if (vPanel == null)
            {
                LogInfo($" Init panel");
                // LogInfo($" 111 {PluginConfig.ShowPanel == null}");
                // LogInfo($" 222 { PluginConfig.ShowPanel.Value }");
                // LogInfo($" 3333 {OverlayUiBase == null}");

                var hotkey = PluginConfig.ShowPanel.Value.ToString();
                vPanel = new VillagerUIPanel(OverlayUiBase, _villagersServiceVillagers, hotkey);
            }
            vPanel.setVillagers(_villagersServiceVillagers);
            vPanel.SetActive(true);
            vPanel.RefreshPanel();
        }

        [HarmonyPatch(typeof(GameController), nameof(GameController.OnDestroy))]
        [HarmonyPrefix]
        private static bool HookEveryGameStop()
        {
            _villagersServiceVillagers = new();
            vPanel?.SetActive(false);
            return true;
        }

        [HarmonyPatch(typeof(Villager), nameof(Villager.SetUp))]
        [HarmonyPostfix]
        private static void onAddVillager()
        {
            vPanel?.RefreshPanel();
        }

        [HarmonyPatch(typeof(Villager), "Die", new Type[] {
            typeof(VillagerLossType),
            typeof(string),
            typeof(bool),
            typeof(float),
            typeof(SoundModel)
        })]
        [HarmonyPostfix]
        private static void onDelVillager()
        {
            vPanel?.RefreshPanel();
        }

        [HarmonyPatch(typeof(FulfillNeeds), nameof(FulfillNeeds.OnStart))]
        [HarmonyPostfix]
        private static void villagerIsHavingBreak(FulfillNeeds __instance)
        {
            LogInfo($"PluginConfig.ShowOpinion={PluginConfig.ShowOpinion} / PluginConfig.opinionThreshold={PluginConfig.OpinionThreshold}");
            if (!PluginConfig.ShowOpinion) return;
            
            var villager = __instance.villager;
            if (villager?.externalName == null) return;
            
            var message = OpinionsManager.GetRandomOpinion(villager);

            if (message != null)
                Serviceable.NewsService.PublishNews(message,message);
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
            var matchingEntry = PluginConfig.renameVillagersShortcuts
                .FirstOrDefault(entry => entry.Key.Value.IsDown());
            return matchingEntry.Value;
        }

        public static void ShowPanel()
        {
            if (vPanel != null)
            {
                vPanel.ShowPanel();
            }
        }


        // private void ddddd(Villager villager)
        // {
        //
        //     // var camps = GameMB.BuildingsService.Camps;
        //     // //.Values.MinBy<Camp, int>((Func<Camp, int>) (c => c.CountWorkers()));
        //     //
        //     // foreach (var camp in camps)
        //     // {
        //     //     LogInfo($"      {camp.Key} {camp.Value.ProductionBuildingState()}");
        //     // }
        //     //
        //     // GameMB.BuildingsService.ProductionBuildings.Count();
        //   var bb =  Serviceable.BuildingsService.ProductionBuildings
        //       .Where<ProductionBuilding>((Func<ProductionBuilding, bool>) (b => b.AreWorkplacesActive && b.CountWorkers() < b.Workplaces.Length && b.IsNot<Relic>())).ToList<ProductionBuilding>();
        //
        //   
        //   var building = bb.FirstOrDefault();
        //   
        //   var workplace = Array.IndexOf<int>(building.Workers, 0);
        //
        //   SetProfession(, building, workplace);
        // }


    }
}