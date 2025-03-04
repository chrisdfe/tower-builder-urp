
using System;

namespace TowerBuilder
{
    public class WalletController
    {
        public int funds { get; private set; } = 1000000;

        WorldController worldController;

        public WalletController(WorldController worldController)
        {
            this.worldController = worldController;
        }

        public void AddFunds(int amount)
        {
            funds += amount;
        }

        public void ReduceFunds(int amount)
        {
            funds -= amount;
        }

        public bool CanAfford(int amount) => (funds - amount) > 0;

        // Comma-seperates number
        // e.g 10000 -> 10,000
        public string FormattedFunds() => $"{funds:n0}";
    }
}