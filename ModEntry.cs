using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.WildTrees;

namespace TreeShakeProducts;

/// <summary>The mod entry point.</summary>
public sealed class ModEntry : Mod
{
    private const float DropChance = 0.05f;
    private const string WildTreesAssetName = "Data/WildTrees";

    private static readonly IReadOnlyDictionary<string, string> ProductsByTreeType =
        new Dictionary<string, string>
        {
            ["1"] = "(O)725",          // Oak Resin
            ["2"] = "(O)724",          // Maple Syrup
            ["3"] = "(O)726",          // Pine Tar
            ["8"] = "(O)92",           // Sap (Mahogany Tree)
            ["13"] = "(O)MysticSyrup"  // Mystic Syrup
        };

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    /// <summary>Add the tapper products to each supported tree's vanilla shake data.</summary>
    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo(WildTreesAssetName))
            return;

        e.Edit(asset =>
        {
            IDictionary<string, WildTreeData> trees = asset.AsDictionary<string, WildTreeData>().Data;

            foreach ((string treeType, string productId) in ProductsByTreeType)
            {
                if (!trees.TryGetValue(treeType, out WildTreeData? tree))
                {
                    this.Monitor.Log(
                        $"Can't add the shake product because tree type '{treeType}' isn't present in {WildTreesAssetName}.",
                        LogLevel.Warn
                    );
                    continue;
                }

                tree.ShakeItems ??= new List<WildTreeItemData>();
                tree.ShakeItems.Add(new WildTreeItemData
                {
                    Id = $"{this.ModManifest.UniqueID}/TapperProduct",
                    ItemId = productId,
                    Chance = DropChance,
                    MinStack = 1,
                    MaxStack = 1
                });
            }
        });
    }
}
