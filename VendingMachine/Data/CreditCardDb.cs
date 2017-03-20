using System;
using System.Collections.Generic;

namespace VendingMachine.Data
{
    public class CreditCardDb
    {
        private readonly List<CreditCardModel> _listCreditCards;

        public CreditCardDb()
        {
            _listCreditCards = new List<CreditCardModel>();
            _listCreditCards.Add(new CreditCardModel { UserId = "A123", CreditCardNumber = 1234567890, PinNumber = 1234, LastModified = DateTimeOffset.Now });
            _listCreditCards.Add(new CreditCardModel { UserId = "A123", CreditCardNumber = 1234567891, PinNumber = 1235, LastModified = DateTimeOffset.Now });
            _listCreditCards.Add(new CreditCardModel { UserId = "ABC1", CreditCardNumber = 1111111111, PinNumber = 1234, LastModified = DateTimeOffset.Now });
        }

        public IEnumerable<CreditCardModel> CreditCards => _listCreditCards;
    }
}