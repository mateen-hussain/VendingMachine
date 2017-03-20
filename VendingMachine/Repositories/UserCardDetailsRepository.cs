using System;
using System.Collections.Generic;
using System.Linq;
using VendingMachine.Data;

namespace VendingMachine.Repositories
{
    public class UserCardDetailsRepository : IUserCardDetailsRepository
    {
        private readonly UserDb _userDb;
        private readonly CreditCardDb _creditCardDb;

        public UserCardDetailsRepository(UserDb userDb, CreditCardDb creditCardDb)
        {
            _userDb = userDb;
            _creditCardDb = creditCardDb;
        }

        public IEnumerable<UserCardDetailsEntity> GetCardDetailsForUser(string userId)
        {
            var newItem = (from user in _userDb.Users
                           join creditCardDetails in _creditCardDb.CreditCards on user.UserId equals creditCardDetails.UserId
                           where user.UserId == userId
                           select new UserCardDetailsEntity
                           {
                               UserId = user.UserId,
                               CreditCardNumber = creditCardDetails.CreditCardNumber,
                               PinNumber = creditCardDetails.PinNumber,
                               Balance = user.CreditRemaining
                           });
            return newItem;
        }

        public bool UpdateBalance(string userId, double amountToDeduct, DateTimeOffset userDataReadTime)
        {
            var index = _userDb.Users.FindIndex(x => x.UserId == userId);
            if (index < 0)
                return false;
            
            //check if some other request update this record
            if (_userDb.Users[index].LastModified > userDataReadTime)
                return false;

            _userDb.Users[index].CreditRemaining = _userDb.Users[index].CreditRemaining - amountToDeduct;
            return true;
        }
    }

    public interface IUserCardDetailsRepository
    {
        IEnumerable<UserCardDetailsEntity> GetCardDetailsForUser(string userId);

        bool UpdateBalance(string userId, double amountToDeduct, DateTimeOffset userDataReadTime);
    }
}