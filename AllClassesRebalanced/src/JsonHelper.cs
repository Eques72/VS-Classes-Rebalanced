using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using SummonSpawnEntry = global::AllClassesRebalanced.ItemSummonerGear.SummonSpawnEntry;

namespace AllClassesRebalanced;

public class JsonHelper
{
    public static Dictionary<string, List<SummonSpawnEntry>>? ExtractSummonEntriesFromAttributes(JsonObject attributes, ICoreAPI api)
    {
        JsonObject spawningConfig = attributes?["spawnEntries"];
        if (spawningConfig == null || !spawningConfig.Exists)
        {
            WarnConfiguration(api, "[allrebalanced] SummonerGear has no spawnEntries configuration.");
            return null;
        }

        Dictionary<string, List<SummonSpawnEntry>> spawningByType =
            ParseSpawnEntries(spawningConfig, api);

        if (spawningByType.Count == 0)
        {
            WarnConfiguration(api, "[allrebalanced] SummonerGear has no usable spawnEntries configuration.");
        }

        return spawningByType;
    }

    public static List<SummonSpawnEntry>? GetSpawnEntries(ItemStack itemstack, Dictionary<string, List<SummonSpawnEntry>>? spawningByType)
    {
        if (spawningByType == null || spawningByType.Count == 0)
        {
            return null;
        }

        string itemCode = itemstack.Collectible.Code.ToString();

        if (spawningByType.TryGetValue(
                itemCode,
                out List<SummonSpawnEntry>? exact))
        {
            return exact;
        }

        List<SummonSpawnEntry>? suffixMatch = FindWildcardMatch(
            spawningByType,
            selector => selector.StartsWith("*", StringComparison.Ordinal),
            selector => itemCode.EndsWith(
                selector[1..],
                StringComparison.OrdinalIgnoreCase));

        if (suffixMatch != null)
        {
            return suffixMatch;
        }

        List<SummonSpawnEntry>? prefixMatch = FindWildcardMatch(
            spawningByType,
            selector => selector.EndsWith("*", StringComparison.Ordinal),
            selector => itemCode.StartsWith(
                selector[..^1],
                StringComparison.OrdinalIgnoreCase));

        if (prefixMatch != null)
        {
            return prefixMatch;
        }

        if (spawningByType.TryGetValue(
                "*",
                out List<SummonSpawnEntry>? fallback))
        {
            return fallback;
        }

        return null;
    }

    private static Dictionary<string, List<SummonSpawnEntry>> ParseSpawnEntries(
        JsonObject spawningConfig,
        ICoreAPI api)
    {
        var spawningByType = new Dictionary<string, List<SummonSpawnEntry>>();

        foreach (JsonObject entry in spawningConfig.AsArray())
        {
            string selector = entry["type"].AsString("");
            string entity = entry["entity"].AsString("");

            if (string.IsNullOrWhiteSpace(selector) ||
                string.IsNullOrWhiteSpace(entity))
            {
                WarnConfiguration(api, "[allrebalanced] SummonerGear has a spawnEntries entry without type or entity.");
                continue;
            }

            if (!spawningByType.TryGetValue(
                    selector,
                    out List<SummonSpawnEntry>? entries))
            {
                entries = new List<SummonSpawnEntry>();
                spawningByType[selector] = entries;
            }

            entries.Add(new SummonSpawnEntry
            {
                Entity = entity,
                Min = entry["min"].AsInt(1),
                Max = entry["max"].AsInt(1),
                Temp_Loss = entry["temp_loss"].AsFloat(0.0f)
            });
        }

        return spawningByType;
    }

    private static List<SummonSpawnEntry>? FindWildcardMatch(
        Dictionary<string, List<SummonSpawnEntry>> spawningByType,
        System.Func<string, bool> isWildcard,
        System.Func<string, bool> matches)
    {
        foreach (KeyValuePair<string, List<SummonSpawnEntry>> entry
                 in spawningByType)
        {
            if (isWildcard(entry.Key) && matches(entry.Key))
            {
                return entry.Value;
            }
        }

        return null;
    }

    private static void WarnConfiguration(ICoreAPI api, string message)
    {
        api.Logger.Warning(
            message
        );
    }
}
