using System;
using System.Linq;
using Eremite;
using Eremite.Buildings;
using Eremite.Characters.Villagers;
using Eremite.Services;

namespace ATS.RenameVillager;

public class VillagerRenameData(Villager villager)
{
    public Villager villager = villager;
   private bool isAlive => villager.IsAlive();
   private string originalName = villager.state.name;

    public void UpdateName(string input)
    {
        if (!villager)
        {
            return;
        }
        if (input.IsNullOrWhiteSpace())
        {
            villager.state.name = originalName;
            villager.externalName = null; // DisplayName (message about leave)
            villager.view.HideName(); // popup with Name

        }
        else
        {
            villager.state.name = input;
            villager.externalName = input; // DisplayName (message about leave)
            villager.view.ShowName(villager.externalName); // popup with Name
        }
    }

    public string getName()
    {
        return villager?.externalName ?? "";
    }

    public void pick()
    {
        if (villager!=null && isAlive)
        {
            villager.Pick();
        }
    }
}