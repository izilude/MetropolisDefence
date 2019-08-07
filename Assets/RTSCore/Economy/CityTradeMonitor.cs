using System.Collections.Generic;
using System.Linq;
using Assets.RTSCore.Inventory;

namespace Assets.RTSCore.Economy
{
    public class CityTradeMonitor
    {
        public List<Item> ExportedItems = new List<Item>();
        public List<Item> ImportedItems = new List<Item>();

        public void ResetCounters()
        {
            ExportedItems.Clear();
            ImportedItems.Clear();
        }

        public void AddToLedger(Item item, bool isImport)
        {
            var ledger = isImport ? ImportedItems : ExportedItems;

            var ledgerItem = ledger.FirstOrDefault(x => x.Name == item.Name);
            if (ledgerItem == null)
            {
                ledger.Add(item);
            }
            else
            {
                ledgerItem.Amount += item.Amount;
            }
        }
    }
}
