using NUnit.Framework;
using VendingMachine.Data;
using VendingMachine.Repositories;

namespace VendingMachine.Tests.Repositories
{
    [TestFixture]
    public class VendingMachineRepositoryTests
    {
        [Test]
        public void UpdateVendingItemsBalance_Update_Remaining_Items_In_Vending_Machine()
        {
            // Arrange
            var vendingDb = new VendingDb();
            var vendingMachineRepository = new VendingMachineRepository(vendingDb);

            // Act
            var result = vendingMachineRepository.UpdateVendingItemsBalance(2);

            // Assert
            Assert.AreEqual(vendingDb.ItemsRemaining, 23);
            Assert.IsTrue(result);
        }

        [Test]
        public void UpdateVendingItemsBalance_Returns_False_When_Items_Vended_Exceed_Capacity()
        {
            // Arrange
            var vendingDb = new VendingDb();
            var vendingMachineRepository = new VendingMachineRepository(vendingDb);

            // Act
            var result = vendingMachineRepository.UpdateVendingItemsBalance(100);

            // Assert
            Assert.AreEqual(vendingDb.ItemsRemaining, 25);
            Assert.IsFalse(result);
        }
    }
}