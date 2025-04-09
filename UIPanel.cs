using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace ATS.RenameVillager;

public abstract class UIPanel : PanelBase
{
    public static Color darkColor = new Color(0.051f, 0.071f, 0.129f);
    public static Color ligthColor = new Color(0.137f, 0.188f, 0.337f);
    
    protected UIPanel(UIBase owner) : base(owner)
    {
    }

    public ButtonRef NavButton { get; internal set; }

    public override void SetActive(bool active)
    {
        if (this.Enabled != active)
        {
            base.SetActive(active);
        }

        if (!active)
        {
            if (Dragger != null)
                this.Dragger.WasDragging = false;
        }
        else
        {
            this.UIRoot.transform.SetAsLastSibling();
            base.SetActive(active);
        }
    }

    public override void ConstructUI()
    {
        base.ConstructUI();
        this.SetActive(true);
        this.SetActive(false);
        this.SetActive(true);
    }

    protected override void LateConstructUI()
    {
        base.LateConstructUI();
        Dragger.OnEndResize();
    }
}