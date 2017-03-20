using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using VendingMachine.Controllers;
using VendingMachine.Models;
using VendingMachine.Repositories;

namespace VendingMachine.Tests.Controllers
{
    [TestFixture]
    public class VendingControllerTests
    {
        private VendingOrderModel _vendingOrderModel;
        private const string _customerId = "ABC";
        private const int _creditCardNumber = 1111111111;
        private const int _pin = 1234;
        private const int _quantity = 1;

        [SetUp]
        public void SetupTestData()
        {
            _vendingOrderModel = new VendingOrderModel
            {
                CustomerId = _customerId,
                CreditCardNumber = _creditCardNumber,
                Pin = _pin,
                Quantity = _quantity
            };
        }

        [Test]
        public void Api_Returns_NotFound_When_UserId_Doesnt_Exist()
        {
            // Arrange
            var userCardDetailsRepository = new Mock<IUserCardDetailsRepository>();
            var vendingMachineRepository = new Mock<IVendingMachineRepository>();
            var vendingController = new VendingController(userCardDetailsRepository.Object, vendingMachineRepository.Object);
            var userDetails = Enumerable.Empty<UserCardDetailsEntity>();
            userCardDetailsRepository.Setup(x => x.GetCardDetailsForUser(_vendingOrderModel.CustomerId)).Returns(userDetails);

            // Act
            IHttpActionResult result = vendingController.Post(_vendingOrderModel);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Api_Returns_NotFound_When_CreditCard_Not_Registered()
        {
            // Arrange
            var userCardDetailsRepository = new Mock<IUserCardDetailsRepository>();
            var vendingMachineRepository = new Mock<IVendingMachineRepository>();
            var vendingController = new VendingController(userCardDetailsRepository.Object, vendingMachineRepository.Object);
            var userDetails = new List<UserCardDetailsEntity>();
            userDetails.Add(new UserCardDetailsEntity { UserId = _customerId, CreditCardNumber = 123456, Balance = 10, PinNumber = _pin });
            userCardDetailsRepository.Setup(x => x.GetCardDetailsForUser(_vendingOrderModel.CustomerId)).Returns(userDetails);

            // Act
            IHttpActionResult result = vendingController.Post(_vendingOrderModel);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Api_Returns_BadRequest_When_CreditCard_And_Pin_Combination_Doesnt_Match()
        {
            // Arrange
            var userCardDetailsRepository = new Mock<IUserCardDetailsRepository>();
            var vendingMachineRepository = new Mock<IVendingMachineRepository>();
            var vendingController = new VendingController(userCardDetailsRepository.Object, vendingMachineRepository.Object);
            var userDetails = new List<UserCardDetailsEntity>();
            userDetails.Add(new UserCardDetailsEntity { UserId = _customerId, CreditCardNumber = _creditCardNumber, Balance = 10, PinNumber = 000 });
            userCardDetailsRepository.Setup(x => x.GetCardDetailsForUser(_vendingOrderModel.CustomerId)).Returns(userDetails);

            // Act
            IHttpActionResult result = vendingController.Post(_vendingOrderModel);
            var content = (result as BadRequestErrorMessageResult).Message;

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.AreEqual(content, "Invalid credit card and pin combination");
        }

        [Test]
        public void Api_Returns_BadRequest_When_Insufficient_Credit_On_Card()
        {
            // Arrange
            var userCardDetailsRepository = new Mock<IUserCardDetailsRepository>();
            var vendingMachineRepository = new Mock<IVendingMachineRepository>();
            var vendingController = new VendingController(userCardDetailsRepository.Object, vendingMachineRepository.Object);

            var userDetails = new List<UserCardDetailsEntity>();
            userDetails.Add(new UserCardDetailsEntity { UserId = _customerId, CreditCardNumber = _creditCardNumber, Balance = 1, PinNumber = _pin });
            userCardDetailsRepository.Setup(x => x.GetCardDetailsForUser(_vendingOrderModel.CustomerId)).Returns(userDetails);

            var vendingItemsDetails = new VendingItemEntity { CostPerItem = 10, ItemsRemaining = 100 };
            vendingMachineRepository.Setup(x => x.GetVendingMachineDetails()).Returns(vendingItemsDetails);

            // Act
            IHttpActionResult result = vendingController.Post(_vendingOrderModel);
            var content = (result as BadRequestErrorMessageResult).Message;

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.AreEqual(content, "Insufficient balance in card");
        }

        [Test]
        public void Api_Returns_Bad_Request_When_Vending_Machine_Has_Insufficient_Items()
        {
            // Arrange
            var userCardDetailsRepository = new Mock<IUserCardDetailsRepository>();
            var vendingMachineRepository = new Mock<IVendingMachineRepository>();
            var vendingController = new VendingController(userCardDetailsRepository.Object, vendingMachineRepository.Object);

            var userDetails = new List<UserCardDetailsEntity>();
            userDetails.Add(new UserCardDetailsEntity { UserId = _customerId, CreditCardNumber = _creditCardNumber, Balance = 100, PinNumber = _pin });
            userCardDetailsRepository.Setup(x => x.GetCardDetailsForUser(_vendingOrderModel.CustomerId)).Returns(userDetails);

            var vendingItemsDetails = new VendingItemEntity { CostPerItem = 1, ItemsRemaining = 0 };
            vendingMachineRepository.Setup(x => x.GetVendingMachineDetails()).Returns(vendingItemsDetails);

            // Act
            IHttpActionResult result = vendingController.Post(_vendingOrderModel);
            var content = (result as BadRequestErrorMessageResult).Message;

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.AreEqual(content, $"quantity requested {_vendingOrderModel.Quantity} exceeds inventory {vendingItemsDetails.ItemsRemaining}");
        }

        [Test]
        public void Api_Returns_Success_Response_Upon_Successfull_Request()
        {
            // Arrange
            var userCardDetailsRepository = new Mock<IUserCardDetailsRepository>();
            var vendingMachineRepository = new Mock<IVendingMachineRepository>();
            var vendingController = new VendingController(userCardDetailsRepository.Object, vendingMachineRepository.Object);

            var userDetails = new List<UserCardDetailsEntity>();
            userDetails.Add(new UserCardDetailsEntity { UserId = _customerId, CreditCardNumber = _creditCardNumber, Balance = 100, PinNumber = _pin });
            userCardDetailsRepository.Setup(x => x.GetCardDetailsForUser(_vendingOrderModel.CustomerId)).Returns(userDetails);

            var vendingItemsDetails = new VendingItemEntity { CostPerItem = 1, ItemsRemaining = 100 };
            vendingMachineRepository.Setup(x => x.GetVendingMachineDetails()).Returns(vendingItemsDetails);

            var costPerItem = vendingItemsDetails.CostPerItem * _vendingOrderModel.Quantity;
            userCardDetailsRepository.Setup(x => x.UpdateBalance(_vendingOrderModel.CustomerId, costPerItem, It.IsAny<DateTimeOffset>())).Returns(true);
            vendingMachineRepository.Setup(x => x.UpdateVendingItemsBalance(_vendingOrderModel.Quantity)).Returns(true);

            // Act
            IHttpActionResult result = vendingController.Post(_vendingOrderModel);

            // Assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<string>>(result);
        }

        [Test]
        public void Api_Returns_InternalServerError_When_It_Fails_To_Deduct_User_Account_Balance()
        {
            // Arrange
            var userCardDetailsRepository = new Mock<IUserCardDetailsRepository>();
            var vendingMachineRepository = new Mock<IVendingMachineRepository>();
            var vendingController = new VendingController(userCardDetailsRepository.Object, vendingMachineRepository.Object);

            var userDetails = new List<UserCardDetailsEntity>();
            userDetails.Add(new UserCardDetailsEntity { UserId = _customerId, CreditCardNumber = _creditCardNumber, Balance = 100, PinNumber = _pin });
            userCardDetailsRepository.Setup(x => x.GetCardDetailsForUser(_vendingOrderModel.CustomerId)).Returns(userDetails);

            var vendingItemsDetails = new VendingItemEntity { CostPerItem = 1, ItemsRemaining = 100 };
            vendingMachineRepository.Setup(x => x.GetVendingMachineDetails()).Returns(vendingItemsDetails);

            var costPerItem = vendingItemsDetails.CostPerItem * _vendingOrderModel.Quantity;
            userCardDetailsRepository.Setup(x => x.UpdateBalance(_vendingOrderModel.CustomerId, costPerItem, It.IsAny<DateTimeOffset>())).Returns(false);

            // Act
            IHttpActionResult result = vendingController.Post(_vendingOrderModel);

            // Assert
            Assert.IsInstanceOf<InternalServerErrorResult>(result);
        }

        
    }
}