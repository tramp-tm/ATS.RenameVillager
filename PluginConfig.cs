using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace ATS.RenameVillager;

internal static class PluginConfig
{
    public static Dictionary<ConfigEntry<KeyboardShortcut>, Action> renameVillagersShortcuts = new();
    
    private static ConfigEntry<bool> _showOpinion = BindConfigParam("ShowOpinion", false, "Show a message with a villager's opinion");
    public static bool ShowOpinion 
    {
        get => _showOpinion.Value && OpinionsManager.hasAnyOpinions();
        set
        {
            _showOpinion.Value = value;
            Plugin.Instance.Config.Save();
        }
    }

    public static ConfigEntry<KeyboardShortcut> ShowPanel = BindHotkey();

    private static ConfigEntry<int> _opinionThreshold = BindConfigParam("OpinionThreshold", 10, "Probability of villager's opinion (%)"); 
    public static int OpinionThreshold 
    {
        get => _opinionThreshold.Value;
        set
        {
            _opinionThreshold.Value = value;
            Plugin.Instance.Config.Save();
        }
    }

    private static ConfigEntry<KeyboardShortcut> BindHotkey()
    {
        var renameVillagersPanelHotkeyDefault = new KeyboardShortcut(KeyCode.F9, Array.Empty<KeyCode>());
        var bind = BindConfigParam(
            "RenameVillagersPanel",
            renameVillagersPanelHotkeyDefault,
            "Show Rename Villagers Panel"
        );

        renameVillagersShortcuts.Add(
            bind,
            Plugin.ShowPanel
        );
        return bind;
    }


    private static ConfigEntry<T> BindConfigParam<T>(string configDefinitionName, T defaultValue, string description)
    {
        var configDefinition = new ConfigDefinition(PluginInfo.PLUGIN_NAME, configDefinitionName);
        var configDescription = new ConfigDescription(description, null, []);
        var bind = Plugin.Instance.Config.Bind<T>(
            configDefinition,
            defaultValue,
            configDescription
        );
        return bind;
    }
}