using System;
using System.Linq;
using Eremite;
using Eremite.Buildings;
using Eremite.Characters.Villagers;
using Eremite.Model;
using Eremite.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;
using UniverseLib.Utility;

namespace ATS.RenameVillager;

public class BuildingCell : PanelCell {
    
    ButtonRef buildingBtn;
     ProductionBuilding currentBuilding;

    protected override void CreateRow()
    {
        var row = UIFactory.CreateHorizontalGroup(UIRoot, $"Cell_Group",
            false, false, true, true, 3, default,
            UIPanel.darkColor);
        buildingBtn = UIFactory.CreateButton(row, "BuildingBtn", "_");
        UIFactory.SetLayoutElement(buildingBtn.Component.gameObject, minHeight: MinHeight - 2, minWidth: 280, flexibleWidth: 500);
        RuntimeHelper.SetColorBlock(buildingBtn.Component, UIPanel.ligthColor, Color.gray, Color.black);
        buildingBtn.OnClick += OnButtonClicked;
    }

    private void OnButtonClicked()
    {
        assign();
    }

    public void UpdateCell(ProductionBuilding build)
    {
        currentBuilding= build;
        // Plugin.LogInfo($"UpdateCell villagerData.villager is Null?  {villagerData.villager ==null}");
        var text = currentBuilding.DisplayName;
        foreach (var slot in currentBuilding.Workers)
        {
            if (slot ==0)
            {
                text += " [ ]";
            }
            else
            {
                text += " [x]";
            }
        }

        var buttonText = text;
        buildingBtn.GameObject.GetComponentInChildren<Text>().text = buttonText;
        buildingBtn.GameObject.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
    }
    
    public void assign()
    {
        var villager = BuildingsUIPanel.currentVillager;
        if (villager!=null )
        {   
            // find empty slot
            
            try
            {
                var freeWorkplace = Array.IndexOf<int>(currentBuilding.Workers, 0);
                SetProfession(villager, currentBuilding, freeWorkplace);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            // Plugin.LogInfo($" villager af {villager.Profession} ");
        }
        Plugin.bPanel.SetActive(false);
        Plugin.vPanel.RefreshPanel();

    }
    
    private void SetProfession(Villager villager, ProductionBuilding building, int workplace)
    {
        // Plugin.LogInfo($" villager {villager.state.name } to  {villager.Profession} at {building} in slot {workplace} ");
        if(villager.Profession != MB.Settings.DefaultProfession.Name) GameMB.VillagersService.ReleaseFromProfession(villager);
        GameMB.VillagersService.SetProfession(villager, building.Profession, building, workplace);
    }
}