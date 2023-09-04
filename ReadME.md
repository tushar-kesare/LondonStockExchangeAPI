# London Stock Exchange API
This solution implements a set of London Stock Exchange APIs to store executed trades and provide current stock prices. 

## Instructions to run
This solution is implemented in .NET 6 and can be opened from Visual Studio 2022. 
Once the solution is opened, set LondonStockExchange.API as default project and run the solution.
You should be able to see the Swagger UI and try out the APIs.

## Architecture
1. LondonStockExchange.API is the main ASP.NET Web API application. It implemtents 3 APIs:

| API                                           | Description                                                                                                                                                      |
| --------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| POST /api/trades                              | Receives a trade notification and stores it in database. Also updates current stock price.                                                                       | 
| GET /api/stocks/{tickerSymbol}                | Returns current price of a stock.                                                                                                                                |
| GET /api/stocks?tickerSymbols=stock1,stock2   | Returns current price of all stocks or stocks provided in `tickerSymbols`. `tickerSymbols` is optional query parameter as comma seperated list of ticker symbols.|

2. This solution implements CQRS and Mediator pattern using Mediatr NuGet package.
	- Controllers send commands/queries to the mediator and the specific handler (from LondonStockExchange.Domain) processes the request by communicating with the repository.
	- CQRS pattern helps to separate read and write operations so that we can scale them independently depending on the load. We can also use different optimized data stores for read and write operations to improve the performance.
	- Mediator pattern decouples the command/query handlers from the controllers making the solution easier to test, modify and maintain.

## Further enhancements
1. GET /stocks endpoint can be updated to check if all ticker symbols exist in the database, else return a different response.
2. Handle any custom exceptions and return correct status code with message.
3. Currently the stock price is updated synchronously when a new trade is stored in the database. The stock price can be updated asynchronously in a diffenent handler. This will improve the performance of POST /api/trades, but will add some delay in updating the stock price.