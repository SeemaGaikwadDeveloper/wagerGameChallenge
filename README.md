# WagerGameBackendAPI

This project is a simple REST API built using .NET Core and C#. It allows players to participate in a game of chance where they can predict a random number between 0 and 9, wager points on their prediction, and win or lose points based on the outcome.

### Features

- Player Identification: Players can be added to the system with a name, email address, and starting points.
- Place Bet: Players can send their bet as a request to the program. They specify the number they predict and the points they wager.
- Get Players: Retrieve a list of all players along with their details.

### Game Rules

- Each player starts with 10,000 points.
- Players can wager any number of points on a prediction.
- If the player predicts the correct number, they win 9 times their stake.
- If the prediction is incorrect, they lose their stake.

### Endpoints

- POST /api/game: Endpoint to place a bet. Requires a JSON body with `points` and `number`.
  Example:
  ```json
  {
    "playerId": 1,
    "points": 100,
    "number": 3
  }
  ```
- POST /api/game/players: Endpoint to add a new player. Requires a JSON body with `name`, `emailAddress`, and `points`.
  Example:
  ```json
  {
    "name": "Seema Gaikwad",
    "emailAddress": "Seema.Gaikwad@example.com",
    "points": 10000
  }
  ```
- GET /api/game/players: Endpoint to retrieve a list of all players.

### Error Handling

- Proper error messages are returned for various scenarios such as invalid wagers, insufficient balance, and player not found.

### Database

- The application uses a SQL Server database to store player information and manage points.

### Configuration

- The connection string to the database is read from the `appsettings.json` file.

### Setup

1. Clone this repository.
2. Open the solution in Visual Studio or any preferred IDE.
3. Set up the database and adjust the connection string in `appsettings.json`.
4. Run the application.

### Dependencies

- Microsoft.AspNetCore.Mvc
- Microsoft.Extensions.Configuration
- System.Data.SqlClient
