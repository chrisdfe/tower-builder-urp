
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

        public string FormattedFunds() => Money.Format(funds);
    }
}