using System;
using System.Linq;
using System.Web.Http;
using VendingMachine.Models;
using VendingMachine.Repositories;

namespace VendingMachine.Controllers
{
    public class VendingController : ApiController
    {
        private readonly IUserCardDetailsRepository _userCardDetailsRepository;
        private readonly IVendingMachineRepository _vendingMachineRepository;

        public VendingController(IUserCardDetailsRepository userCardDetailsRepository, IVendingMachineRepository vendingMachineRepository)
        {
            _userCardDetailsRepository = userCardDetailsRepository;
            _vendingMachineRepository = vendingMachineRepository;
        }

        /// <summary>
        /// Assumptions:
        /// 1) There is only one type of item in vending machine
        /// 2) Both cards have always the same balance
        /// </summary>
        /// <param name="vendingOrderModel"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]VendingOrderModel vendingOrderModel)
        {
            var userDataReadTime = DateTimeOffset.Now;
            var userCardDetails = _userCardDetailsRepository.GetCardDetailsForUser(vendingOrderModel.CustomerId);

            // validate user exists
            if (!userCardDetails.Any())
                return NotFound();

            var cardDetails = userCardDetails.FirstOrDefault(x => x.CreditCardNumber == vendingOrderModel.CreditCardNumber);

            // validate is user had credit card registered
            if (cardDetails == null)
                return NotFound();

            // validate credit card and pin match
            if (cardDetails.PinNumber != vendingOrderModel.Pin)
                return BadRequest("Invalid credit card and pin combination");

            var vendingItemsDetails = _vendingMachineRepository.GetVendingMachineDetails();

            // validate total cost vs balance
            var costOfItems = vendingOrderModel.Quantity * vendingItemsDetails.CostPerItem;
            if (costOfItems > cardDetails.Balance)
                return BadRequest("Insufficient balance in card");

            // validate items remaining
            if (vendingOrderModel.Quantity > vendingItemsDetails.ItemsRemaining)
                return BadRequest($"quantity requested {vendingOrderModel.Quantity} exceeds inventory {vendingItemsDetails.ItemsRemaining}");

            // Deduct balance from card
            if (!_userCardDetailsRepository.UpdateBalance(vendingOrderModel.CustomerId, costOfItems,userDataReadTime))
                return InternalServerError();

            // Deduct items from vend
            var vendStatus = _vendingMachineRepository.UpdateVendingItemsBalance(vendingOrderModel.Quantity);

            if (!vendStatus)
            {
                // Refund the amount to User
                _userCardDetailsRepository.UpdateBalance(vendingOrderModel.CustomerId, -costOfItems, userDataReadTime);
                return InternalServerError();
            }
                

            return Ok($"Here are your {vendingOrderModel.Quantity} items");
        }
    }
}