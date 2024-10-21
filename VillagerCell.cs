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

public class VillagerCell : ICell
{
    public GameObject UIRoot { get; set; }
    public float DefaultHeight => 30f;

    ButtonRef villagerBtn;
    Text OriginalNameLabel;
    InputFieldRef inputField;

    public VillagerRenameData villagerData;

    private const int MinHeight = 25;

    public GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateUIObject("VillagerCell", parent, new Vector2(25, 25));
        Rect = UIRoot.GetComponent<RectTransform>();
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(UIRoot, false, false, true, true, 3);
        UIFactory.SetLayoutElement(UIRoot, minHeight: MinHeight, minWidth: 50, flexibleWidth: 9999);

        CreateRow();

        return UIRoot;
    }

    void CreateRow()
    {
        var row = UIFactory.CreateHorizontalGroup(UIRoot, $"VillagerCell_Group",
            false, false, true, true, 3, default,
            VillagerUIPanel.darkColor);
        villagerBtn = UIFactory.CreateButton(row, "VillagerBtn", "_");
        UIFactory.SetLayoutElement(villagerBtn.Component.gameObject, minHeight: MinHeight - 2, minWidth: 180,
            flexibleWidth: 0);
        RuntimeHelper.SetColorBlock(villagerBtn.Component, VillagerUIPanel.ligthColor,
            Color.gray, Color.black);
        villagerBtn.OnClick += OnVillagerButtonClicked;
        
        OriginalNameLabel = UIFactory.CreateLabel(row, $"OriginalNameCell_Label", "OriginalName_Label");
        UIFactory.SetLayoutElement(OriginalNameLabel.gameObject, minWidth: 150, minHeight: MinHeight);
        GameObject obj = UIFactory.CreateUIObject("Spacer", row);
        UIFactory.SetLayoutElement(obj, minWidth: MinHeight, flexibleHeight: 0);
        inputField = UIFactory.CreateInputField(row, $"VillagerCell_Input", "");
        UIFactory.SetLayoutElement(inputField.GameObject, minWidth: 125, minHeight: MinHeight - 2, flexibleWidth: 9999);
        inputField.Component.GetOnEndEdit().AddListener(SetVillagerName);
    }

    private void OnVillagerButtonClicked()
    {
        if (villagerData.villager!=null && villagerData.isAlive)
        {
            villagerData.villager.Pick();
        }
    }

    void CreateRow2()
    {
        GameObject row = UIFactory.CreateHorizontalGroup(UIRoot, $"RowGroup", false, false, true, true, 3, default,
            new(1, 1, 1, 0));

        var rect = row.GetComponent<RectTransform>();
        // rect.localPosition = new Vector3(0, 0, 0);
        rect.sizeDelta = new Vector2(200, 200);

        var textField = new GameObject("Input",
            typeof(CanvasRenderer),
            typeof(RectTransform),
            typeof(Image),
            typeof(TMP_InputField));
        textField.transform.parent = row.transform;
        textField.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);
        textField.GetComponent<RectTransform>().localPosition = Vector3.zero;
        textField.GetComponent<TMP_InputField>().text = "example";
        textField.GetComponent<TMP_InputField>().targetGraphic = textField.GetComponent<Image>();
        textField.GetComponent<Image>().type = Image.Type.Sliced;

        var placeHolder = new GameObject("Placeholder", typeof(CanvasRenderer), typeof(TextMeshProUGUI),
            typeof(LayoutElement));

        var text = new GameObject("Text", typeof(CanvasRenderer), typeof(TextMeshProUGUI));

        var textArea = new GameObject("Text Area", typeof(RectMask2D), typeof(RectTransform));

        textField.GetComponent<TMP_InputField>().textViewport = textArea.GetComponent<RectTransform>();
        textArea.transform.SetParent(textField.transform);
        textArea.GetComponent<RectTransform>().localPosition = Vector3.zero;

        placeHolder.transform.SetParent(textArea.transform);
        text.transform.SetParent(textArea.transform);
        text.GetComponent<TextMeshProUGUI>().color = Color.yellow;

        text.GetComponent<RectTransform>().localPosition = Vector3.zero;
        placeHolder.GetComponent<RectTransform>().localPosition = Vector3.zero;


        textField.GetComponent<TMP_InputField>().textComponent = text.GetComponent<TextMeshProUGUI>();

        rect.localPosition = new Vector3(0, 0, 0);
        rect.sizeDelta = new Vector2(200, 25);

        textField.GetComponent<TMP_InputField>().Select();
        textField.GetComponent<TMP_InputField>().ActivateInputField();
    }

    void SetVillagerName(string input)
    {
        villagerData.UpdateName(input);
    }

    private string GetRandomName(Villager villager)
    {
        return villager.state.isMale
            ? (villager.raceModel.maleNames.Length == 0
                ? VillagersNames.MaleNames.RandomElement<string>()
                : villager.raceModel.maleNames.RandomElement<string>())
            : (villager.raceModel.femaleNames.Length == 0
                ? VillagersNames.FemaleNames.RandomElement<string>()
                : villager.raceModel.femaleNames.RandomElement<string>());
    }

    public void Enable()
    {
        this.UIRoot.SetActive(true);
    }

    public void Disable()
    {
        this.UIRoot.SetActive(false);
    }

    public bool Enabled => this.UIRoot.activeSelf;
    public RectTransform Rect { get; set; }

    public void UpdateCell()
    {
        
        // Plugin.LogInfo($"UpdateCell villagerData.villager is Null?  {villagerData.villager ==null}");
        // Plugin.LogInfo($"UpdateCell villagerData.villager.raceModel.displayName.Text = {villagerData.villager.raceModel.displayName.Text ==null}");
        // Plugin.LogInfo($"UpdateCell villagerData.villager.professionModel.displayName = {villagerData.villager.professionModel.displayName ==null}");
        // Plugin.LogInfo($"UpdateCell villagerData.originalName = {villagerData.originalName ==null}");
        // Plugin.LogInfo($"UpdateCell villagerData.newName = {villagerData.newName ==null}");
        ;
        var resolve = villagerData.villager.raceModel.initialResolve.RoundToInt() +
                      villagerData.villager.GetResolveImpact();
        var buttonText = 
            $"<color=cyan>{villagerData.villager.raceModel.displayName.Text}</color> " +
            $"[{villagerData.villager.professionModel.displayName}] " +
            $" {MB.RichTextService.GetColoredCounter(resolve, forceNoPlus: true)}";

        villagerBtn.GameObject.GetComponentInChildren<Text>().text = buttonText;
        OriginalNameLabel.text = villagerData.originalName;
        inputField.Text = villagerData.newName;
    }
}