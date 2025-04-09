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

public abstract class PanelCell : ICell
{
    public GameObject UIRoot { get; set; }
    public float DefaultHeight => 30f;
    protected const int MinHeight = 25;

    public GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateUIObject("PanelCell", parent, new Vector2(25, 25));
        Rect = UIRoot.GetComponent<RectTransform>();
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(UIRoot, false, false, true, true, 3);
        UIFactory.SetLayoutElement(UIRoot, minHeight: MinHeight, minWidth: 50, flexibleWidth: 9999);

        CreateRow();
        
        return UIRoot;
    }

    protected abstract void CreateRow();


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
    
}