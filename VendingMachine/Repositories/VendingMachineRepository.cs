using VendingMachine.Data;

namespace VendingMachine.Repositories
{
    public class VendingMachineRepository : IVendingMachineRepository
    {
        private VendingDb _vendingDb;

        public VendingMachineRepository(VendingDb vendingDb)
        {
            _vendingDb = vendingDb;
        }

        public VendingItemEntity GetVendingMachineDetails()
        {
            return new VendingItemEntity { CostPerItem = _vendingDb.ItemCost, ItemsRemaining = _vendingDb.ItemsRemaining };
        }

        public bool UpdateVendingItemsBalance(int numberOfItemsVended)
        {
            if (numberOfItemsVended > _vendingDb.ItemsRemaining)
                return false;
            _vendingDb.ItemsRemaining = _vendingDb.ItemsRemaining - numberOfItemsVended;
            return true;
        }
    }

    public interface IVendingMachineRepository
    {
        VendingItemEntity GetVendingMachineDetails();

        bool UpdateVendingItemsBalance(int numberOfItemsVended);
    }
}