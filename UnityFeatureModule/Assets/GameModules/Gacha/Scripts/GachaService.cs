namespace TheOneStudio.HyperCasual.GameModules.Gacha.Scripts
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.HyperCasual.GameModules.Gacha.Scripts.Blueprint;
    using TheOneStudio.UITemplate.UITemplate.Extension;

    public class GachaService
    {
        #region Inject

        private readonly GachaBlueprint     gachaBlueprint;
        private readonly GachaPoolBlueprint gachaPoolBlueprint;

        public GachaService(GachaBlueprint gachaBlueprint, GachaPoolBlueprint gachaPoolBlueprint)
        {
            this.gachaBlueprint     = gachaBlueprint;
            this.gachaPoolBlueprint = gachaPoolBlueprint;
        }

        #endregion

        public IReadOnlyList<(string ItemId, int Amount, string Rarity)> DoGacha(string gachaId, int time = 1)
        {
            return this.gachaBlueprint[gachaId].GachaPackId.SelectMany(gachaPack =>
            {
                var gachaPackRecord = this.gachaPoolBlueprint[gachaPack.Key];
                var itemIds         = gachaPackRecord.ItemIdToData.ToDictionary(pack => pack.Key, pack => pack.Value.Weight);
                return IterTools.Repeat(
                    () =>
                    {
                        var itemId              = itemIds.RandomGachaWithWeight();
                        var gachaPackItemRecord = gachaPackRecord.ItemIdToData[itemId];
                        return (gachaPackItemRecord.Item, gachaPackItemRecord.Amount, gachaPackItemRecord.Rarity);
                    },
                    gachaPack.Value * time
                );
            }).ToArray();
        }
    }
}