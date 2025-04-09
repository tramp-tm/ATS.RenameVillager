using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eremite.Characters.Villagers;
using Eremite.Services;
using UnityEngine;

namespace ATS.RenameVillager;

internal static class OpinionsManager
{
    private static readonly Dictionary<string, List<string>> _opinions = new();
    private static readonly System.Random _random = new();

    public static void LoadOpinions(string path)
    {
        _opinions.Clear();

        if (!File.Exists(path)) return;

        string? currentKey = null;

        foreach (var line in File.ReadAllLines(path))
        {
            var trimmed = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                currentKey = trimmed.Trim('[', ']');
                if (!_opinions.ContainsKey(currentKey))
                    _opinions[currentKey] = new List<string>();
            }
            else if (currentKey != null)
            {
                var value = trimmed;
                if (!string.IsNullOrWhiteSpace(value))
                    _opinions[currentKey].Add(value);
            }
        }
    }

    public static string GetRandomOpinion(Villager villager)
    {
        if (string.IsNullOrWhiteSpace(villager.externalName) ) return null;

        if (_random.Next(100) >= PluginConfig.OpinionThreshold) return null;

        int resolve = villager.raceModel.initialResolve.RoundToInt() + villager.GetResolveImpact();
        var threshold = Serviceable.EffectsService.GetReputationTreshold(villager.raceModel);

        string key = resolve >= threshold ? "happy" : resolve <= 0 ? "sad" : "normal";
        
        if (!_opinions.TryGetValue(key, out var list) || list.Count == 0) return null;
        
        var opinion = list[_random.Next(list.Count)];

        return $"{villager.externalName} {opinion}";
    }

    public static string getTotals()
    {
        var ret = "";
        foreach (var key in _opinions.Keys)
        {
            ret += $"{key}: {_opinions[key].Count} opinions, ";
        }
        return ret;
    }
    
    public static bool hasAnyOpinions()
    {
        return _opinions.Values.Any(list => list.Count > 0);
    }
}
