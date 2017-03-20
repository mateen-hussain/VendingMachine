using System;

namespace VendingMachine.Data
{
    public class CreditCardModel
    {
        public string UserId;
        public long CreditCardNumber;
        public int PinNumber;
        public DateTimeOffset LastModified;
    }
}