using Eremite.Characters.Villagers;

namespace ATS.RenameVillager;

public class VillagerRenameData(Villager villager)
{
    public Villager villager = villager;
    public bool isAlive => villager.IsAlive();
    public string originalName = villager.state.name;
    public string newName="";

    public void UpdateName(string input)
    {
        if (input.IsNullOrWhiteSpace())
        {
            var newName = "" ;
            villager.state.name = originalName;
            villager.externalName = null; // DisplayName (message about leave)
        }
        else
        {
            newName = input;
            villager .state.name = newName;
            villager.externalName = newName; // DisplayName (message about leave)
        }
        villager.view.ShowName(villager.externalName); // popup with Name
    }
}