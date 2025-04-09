using System;
using System.Collections.Generic;
using System.Linq;
using Eremite;
using Eremite.Buildings;
using Eremite.Characters.Villagers;
using Eremite.Services;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Widgets.ScrollView;

namespace ATS.RenameVillager;

public class BuildingsUIPanel : UIPanel
{
    public override string Name => "Builders List";

    public override int MinWidth => 400;
    public override int MinHeight => 200;
    public override Vector2 DefaultAnchorMin => new(0.7f, 0.7f);
    public override Vector2 DefaultAnchorMax => new(0.7f, 0.7f);

    public override bool CanDragAndResize => true;
    
    private  List<ProductionBuilding> buildings =>  Serviceable.BuildingsService.ProductionBuildings
        .Where<ProductionBuilding>(
            (Func<ProductionBuilding, bool>) (b => b.AreWorkplacesActive && b.CountWorkers() < b.Workplaces.Length && b.IsNot<Relic>()))
        .ToList<ProductionBuilding>();

    public static Villager currentVillager;

    private ListHandler<ProductionBuilding, BuildingCell> dataHandler;
    private ScrollPool<BuildingCell> scrollPool;
    public static Color darkColor = new Color(0.051f, 0.071f, 0.129f);
    public static Color ligthColor = new Color(0.137f, 0.188f, 0.337f);

    public BuildingsUIPanel(UIBase owner, Villager v) : base(owner)
    {
        currentVillager = v;
    }
    
    public override void ConstructUI()
    {
        base.ConstructUI();
        Image img = uiRoot.GetComponent<Image>();
        img.color = darkColor;
        TitleBar.gameObject.GetComponent<Image>().color = ligthColor;
    }

    public void FillPanel()
    {
        dataHandler.RefreshData();
        scrollPool.Refresh(true, true);
    }
    
    protected override void ConstructPanelContent()
    {
        dataHandler = new ListHandler<ProductionBuilding, BuildingCell>(() => { return buildings.Count; }, SetCell);
        scrollPool = UIFactory.CreateScrollPool<BuildingCell>(this.ContentRoot, "BuildingsList",
            out GameObject scrollObj, out GameObject scrollContent, darkColor);
        scrollPool.Initialize(dataHandler);
        // UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);
    }

    private void SetCell(PanelCell cell, int index)
    {
        if (index >= buildings.Count)
            return;
        var obj = buildings[index];
        (cell as BuildingCell).UpdateCell(obj);
    }

    public void setCurrentVillager(Villager v)
    {
        currentVillager = v;
    }
}