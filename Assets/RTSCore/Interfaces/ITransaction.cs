using System.Collections.Generic;

namespace Assets.RTSCore.Interfaces
{
    public interface ITransaction
    {
        List<string> ServicesProvided();
        bool CanDistributeItem(string itemName);
        bool CanRetrieveItem(string itemName);
        bool CanHarvestItem(string itemName);
        bool CanDistributeAndRetrieveItem(string itemName);
    }
}
