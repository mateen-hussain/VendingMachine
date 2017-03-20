using NUnit.Framework;
using System;
using System.Linq;
using VendingMachine.Data;
using VendingMachine.Repositories;

namespace VendingMachine.Tests.Repositories
{
    [TestFixture]
    public class UserCardDetailsRepositoryTests
    {
        [Test]
        public void GetCardDetailsForUser_Returns_EmptyList_For_Non_Existent_User()
        {
            // Arrange
            var userDb = new UserDb();
            var creditCardDb = new CreditCardDb();
            var userCardDetailsRepo = new UserCardDetailsRepository(userDb, creditCardDb);

            // Act
            var result = userCardDetailsRepo.GetCardDetailsForUser("SOMERANDOM");

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetCardDetailsForUser_Returns_Expected_Rows_For_Existent_User()
        {
            // Arrange
            var userDb = new UserDb();
            var creditCardDb = new CreditCardDb();
            var userCardDetailsRepo = new UserCardDetailsRepository(userDb, creditCardDb);

            // Act
            var result = userCardDetailsRepo.GetCardDetailsForUser("A123");

            // Assert
            Assert.AreEqual(result.Count(), 2);
        }

        [Test]
        public void UpdateBalance_Returns_False_For_Non_Existent_USer()
        {
            // Arrange
            var userDb = new UserDb();
            var creditCardDb = new CreditCardDb();
            var userCardDetailsRepo = new UserCardDetailsRepository(userDb, creditCardDb);
            var userDataReadTime = DateTimeOffset.Now;

            // Act
            var result = userCardDetailsRepo.UpdateBalance("SomethingRandom", 1.0, userDataReadTime);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void UpdateBalance_Returns_True_And_Updates_Balance_For_Existent_USer()
        {
            // Arrange
            var userDb = new UserDb();
            var creditCardDb = new CreditCardDb();
            var userCardDetailsRepo = new UserCardDetailsRepository(userDb, creditCardDb);
            var userDataReadTime = DateTimeOffset.Now;

            // Act
            var result = userCardDetailsRepo.UpdateBalance("A123", 1.0, userDataReadTime);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(userDb.Users[0].CreditRemaining, 9.0);
        }

        [Test]
        public void UpdateBalance_Returns_False_If_User_Modified_Before_Read()
        {
            // Arrange
            var userDataReadTime = DateTimeOffset.Now.AddSeconds(-10);
            var userDb = new UserDb();
            var creditCardDb = new CreditCardDb();
            var userCardDetailsRepo = new UserCardDetailsRepository(userDb, creditCardDb);

            // Act
            var result = userCardDetailsRepo.UpdateBalance("A123", 1.0, userDataReadTime);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(userDb.Users[0].CreditRemaining, 10.0);
        }
    }
}