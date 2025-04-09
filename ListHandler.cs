using System;
using System.Collections.Generic;
using UniverseLib.UI.Widgets.ScrollView;

namespace ATS.RenameVillager;

public class ListHandler<TData, TCell> : ICellPoolDataSource<TCell> where TCell : PanelCell
{
    protected Func<int> EntriesCount;
    protected Action<TCell, int> SetICell;
    // public List<TData> CurrentEntries { get; } = new List<TData>();


    public void OnCellBorrowed(TCell cell)
    {
    }

    public int ItemCount { get; set; }

    public ListHandler(
        Func<int> entriesCountMethod,
        Action<TCell, int> setICellMethod
    )
    {
        this.EntriesCount = entriesCountMethod;
        this.SetICell = setICellMethod;
    }

    public virtual void SetCell(TCell cell, int index)
    {
        if (this.ItemCount == 0)
            this.RefreshData();
        if (index < 0 || index >= ItemCount)
        {
            cell.Disable();
        }
        else
        {
            cell.Enable();
            this.SetICell(cell, index);
        }
    }

    public void RefreshData()
    {
        ItemCount = EntriesCount();
    }
}