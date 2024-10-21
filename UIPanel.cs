using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace ATS.RenameVillager;

public abstract class UIPanel : PanelBase
{
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