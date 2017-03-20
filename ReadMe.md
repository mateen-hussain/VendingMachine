Before you run the solution
- Restore Nuget Packages
- Build solution
- Uses Ninject for IoC
- Uses Nunit to run tests (ensure you have NUnit Test Adapter plugin in Visual Studio to run your tests)
- Run Tests using Test Explorer

To Use the api, make a post request to :
- http://localhost:55438/api/Vending
- with sample body
        {
          "CustomerId":"A123",
          "CreditCardNumber":1234567890,
          "Pin":1234,
          "Quantity":1
        }
 
 
Assumptions:
- If a user has multiple cards that implies they are linked and belong to the same account and hence have the same balance.
