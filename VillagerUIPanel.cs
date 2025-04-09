using System;
using System.Collections.Generic;
using System.Linq;
using Eremite;
using Eremite.Characters.Villagers;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;

namespace ATS.RenameVillager;

public class VillagerUIPanel : UIPanel
{
    public override string Name => "Villagers List";

    public override int MinWidth => 700;
    public override int MinHeight => 300;
    public override Vector2 DefaultAnchorMin => new(0.5f, 0.5f);
    public override Vector2 DefaultAnchorMax => new(0.5f, 0.5f);

    public override bool CanDragAndResize => true;


    private bool ShowMenu
    {
        get => Plugin.OverlayUiBase != null && Plugin.OverlayUiBase.Enabled;
        set
        {
            if (Plugin.OverlayUiBase == null || !UIRoot || Plugin.OverlayUiBase.Enabled == value)
                return;

            UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID, value);
        }
    }

    private ListHandler<VillagerRenameData, VillagerCell> dataHandler;
    private ScrollPool<VillagerCell> villagerScrollPool;

    private ButtonRef newCloseButton;
    // private string HotkeyString;

    private static Dictionary<int, Villager> _villagersList = new();
    private static Dictionary<int, VillagerRenameData> _villagersData = new();

    public VillagerUIPanel(UIBase owner, Dictionary<int, Villager> villagers, string hotkey) :
        base(owner)
    {
        if (newCloseButton != null)
        {
            newCloseButton.ButtonText.text = hotkey;
        }

        setVillagers(villagers);
    }

    private void SyncVillagers()
    {
        var idsToRemove = _villagersData.Keys.Except(_villagersList.Keys).ToList();
        foreach (var id in idsToRemove)
        {
            _villagersData.Remove(id);
        }

        foreach (var villager in _villagersList.Values)
        {
            if (!_villagersData.ContainsKey(villager.Id))
            {
                _villagersData[villager.Id] = new VillagerRenameData(villager);
            }
        }
    }

    public override void ConstructUI()
    {
        base.ConstructUI();
        Image img = uiRoot.GetComponent<Image>();
        img.color = darkColor;
        TitleBar.gameObject.GetComponent<Image>().color = ligthColor;

        var refreshButton = UIFactory.CreateButton(TitleBar, "RefreshListButton", "Refresh");
        UIFactory.SetLayoutElement(refreshButton.GameObject, minWidth: 50, minHeight: 25, flexibleWidth: 9999);
        RuntimeHelper.SetColorBlock((Selectable)refreshButton.Component, darkColor,
            Color.gray, Color.black);
        refreshButton.OnClick += RefreshButton_OnClick;

        GameObject closeHolder = this.TitleBar.transform.Find("CloseHolder").gameObject;
        GameObject closeButtonObj = closeHolder.transform.Find("CloseButton")?.gameObject;

        if (closeButtonObj != null)
        {
            closeButtonObj.Destroy();

            newCloseButton = UIFactory.CreateButton(closeHolder, "CloseButton", "X");

            GameObject gameObject4 = newCloseButton.Component.gameObject;
            UIFactory.SetLayoutElement(gameObject4, minWidth: 25, minHeight: 25, 0, preferredWidth: 10);
            RuntimeHelper.SetColorBlock(newCloseButton.Component, new Color(0.63f, 0.32f, 0.31f),
                new Color(0.81f, 0.25f, 0.2f), new Color(0.6f, 0.18f, 0.16f));
            newCloseButton.OnClick += (Action)(() => this.ShowPanel());
        }
        else
        {
            Plugin.LogInfo("ButtonRef и UnityEngine.UI.Button не найдены.");
        }


        refreshButton.Component.transform.SetSiblingIndex(closeHolder.transform.GetSiblingIndex());
    }

    void RefreshButton_OnClick()
    {
        RefreshPanel();
    }

    protected override void ConstructPanelContent()
    {
        dataHandler = new ListHandler<VillagerRenameData, VillagerCell>(() =>
        {
            SyncVillagers();
            return _villagersData.Count;
        }, SetCell);
        villagerScrollPool = UIFactory.CreateScrollPool<VillagerCell>(this.ContentRoot, "VillagersList",
            out GameObject scrollObj, out GameObject scrollContent, darkColor);
        villagerScrollPool.Initialize(dataHandler);
        // UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);
    }

    private void SetCell(VillagerCell cell, int index)
    {
        if (index >= _villagersData.Count)
            return;
        var sortedVillagers = _villagersData.Values
            .OrderBy(v => v.villager.Race) 
            .ThenBy(v => v.villager.Id) 
            .ToList();
        var obj = sortedVillagers[index];
        
        cell.UpdateCell(obj);
    }

    public void RefreshPanel()
    {
        SyncVillagers();
        dataHandler.RefreshData();
        villagerScrollPool.Refresh(true, true);
    }

    public void ShowPanel()
    {
        ShowMenu = !ShowMenu;
        // SyncVillagers();
        RefreshPanel();
    }

    public void setVillagers(Dictionary<int, Villager> villagers)
    {
        _villagersList = villagers;
        _villagersData.Clear();
        SyncVillagers();
    }
}