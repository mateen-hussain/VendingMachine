namespace VendingMachine.Data
{
    public class VendingDb
    {
        public double ItemCost = 0.50;

        // creating In memory DB and persisting state
        // This will not be the behaviour had we been using real data set or DB in real implementation
        private static int _itemsRemaining = 25;

        public int ItemsRemaining
        {
            get { return _itemsRemaining; }
            set { _itemsRemaining = value; }
        }
    }
}