using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace ATS.RenameVillager;

public class PluginConfig
{
    public Dictionary<ConfigEntry<KeyboardShortcut>, Action> renameVillagersShortcuts = new();

    public readonly string GreetingString;

    public readonly ConfigEntry<KeyboardShortcut> ShowPanel;

    public PluginConfig()
    {
        GreetingString = BindConfigParam(
            "GreetingString",
            " just arrived!",
            "Message for Greeting Renamed Villagers"
        ).Value;
        ShowPanel = BindHotkey();
    }


    private ConfigEntry<KeyboardShortcut> BindHotkey()
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