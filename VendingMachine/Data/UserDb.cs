using System;
using System.Collections.Generic;

namespace VendingMachine.Data
{
    public class UserDb
    {
        // creating In memory DB and persisting state
        // This will not be the behaviour had we been using real data set or DB in real implementation
        private static readonly List<UserModel> _listUsers = new List<UserModel>()
        {
            new UserModel { UserId = "A123", CreditRemaining = 10.00, LastModified = DateTimeOffset.Now },
            new UserModel { UserId = "ABC1", CreditRemaining = 20.35, LastModified = DateTimeOffset.Now }
        };

        public List<UserModel> Users => _listUsers;
    }
}