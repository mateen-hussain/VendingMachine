using System;

namespace VendingMachine.Data
{
    public class UserModel
    {
        public string UserId;
        public double CreditRemaining;
        public DateTimeOffset LastModified;
    }
}