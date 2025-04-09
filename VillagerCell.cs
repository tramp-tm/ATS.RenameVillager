using Eremite;
using Eremite.Characters.Villagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;
using UniverseLib.Utility;

namespace ATS.RenameVillager;

public class VillagerCell : PanelCell
{
    ButtonRef villagerBtn;
    // Text OriginalNameLabel;
    InputFieldRef inputField;

    private VillagerRenameData villagerData;
    
    protected override void CreateRow()
    {
        var row = UIFactory.CreateHorizontalGroup(UIRoot, $"VillagerCell_Group",
            false, false, true, true, 3, default,
            UIPanel.darkColor);
        villagerBtn = UIFactory.CreateButton(row, "VillagerBtn", "_");
        UIFactory.SetLayoutElement(villagerBtn.Component.gameObject, minHeight: MinHeight - 2, minWidth: 280, flexibleWidth: 500);
        RuntimeHelper.SetColorBlock(villagerBtn.Component, UIPanel.ligthColor, Color.gray, Color.black);
        villagerBtn.OnClick += OnVillagerButtonClicked;
        
        // OriginalNameLabel = UIFactory.CreateLabel(row, $"OriginalNameCell_Label", "OriginalName_Label");
        // UIFactory.SetLayoutElement(OriginalNameLabel.gameObject, minWidth: 150, minHeight: MinHeight);

        ButtonRef assignBtn = UIFactory.CreateButton(row,"assignBtn","assign");
        RuntimeHelper.SetColorBlock(villagerBtn.Component, UIPanel.darkColor, Color.gray, Color.black);

        assignBtn.OnClick += AssignVillagerButtonClicked;
        UIFactory.SetLayoutElement(assignBtn.Component.gameObject, minWidth: 50, minHeight: MinHeight);
        
        GameObject obj = UIFactory.CreateUIObject("Spacer", row);
        UIFactory.SetLayoutElement(obj, minWidth: MinHeight, flexibleHeight: 0);
        inputField = UIFactory.CreateInputField(row, $"VillagerCell_Input", "");
        UIFactory.SetLayoutElement(inputField.GameObject, minWidth: 125, minHeight: MinHeight - 2, flexibleWidth: 1000);
        inputField.Component.GetOnEndEdit().AddListener(SetVillagerName);
    }

    private void OnVillagerButtonClicked()
    {
        villagerData.pick();
    }

    private void AssignVillagerButtonClicked()
    {
        if (Plugin.bPanel == null) 
            Plugin.bPanel = new BuildingsUIPanel(Plugin.OverlayUiBase, villagerData.villager);
        Plugin.bPanel.setCurrentVillager(villagerData.villager);
        Plugin.bPanel.FillPanel();
        Plugin.bPanel.SetActive(true);
    }

    private void SetVillagerName(string input)
    {
        villagerData.UpdateName(input);
    }

    public void UpdateCell(VillagerRenameData villagerRenameData)
    {
        if (! villagerRenameData.villager)
        {
             return;
        }
        villagerData = villagerRenameData;
        // Plugin.LogInfo($"UpdateCell villagerData.villager is Null?  {villagerData.villager ==null}");
        // Plugin.LogInfo($"UpdateCell villagerData.villager.raceModel.displayName.Text = {villagerData.villager.raceModel.displayName.Text ==null}");
        // Plugin.LogInfo($"UpdateCell villagerData.villager.professionModel.displayName = {villagerData.villager.professionModel.displayName ==null}");
        // Plugin.LogInfo($"UpdateCell villagerData.originalName = {villagerData.originalName ==null}");
        // Plugin.LogInfo($"UpdateCell villagerData.newName = {villagerData.newName ==null}");
        ;
        var resolve = villagerData.villager.raceModel.initialResolve.RoundToInt() +
                      villagerData.villager.GetResolveImpact();
        var genderSign = villagerData.villager.state.isMale ? "<color=cyan>\u2642</color>" : "<color=magenta>\u2640</color>";
        var buttonText = 
            $"<color=yellow>{villagerData.villager.raceModel.displayName.Text}</color> " +
            $" [{genderSign}] "+
            $" ({villagerData.villager.professionModel.displayName})" +
            $" {MB.RichTextService.GetColoredCounter(resolve, forceNoPlus: true)}";

        villagerBtn.GameObject.GetComponentInChildren<Text>().text = buttonText;
        villagerBtn.GameObject.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
        // OriginalNameLabel.text = villagerData.originalName;
        inputField.Text = villagerData.getName();
    }
}