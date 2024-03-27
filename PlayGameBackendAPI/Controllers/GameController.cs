using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayGameBackendAPI.Model;
using System.Data.SqlClient;

namespace PlayGameBackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {

        public readonly string _connectionString;

        public GameController(IConfiguration configuration)
        {
            //_connectionString = configuration.GetConnectionString("DevConnection");
            _connectionString = configuration["ConnectionStrings:DevConnection"];
        }

        [HttpPost]
        public IActionResult PlaceBet(WagerRequest request)
        {
            var playerId = GetPlayerId(request.PlayerId);
            // Check if player exists
            if (playerId == null || playerId == 0)
                return BadRequest("Player not found.");
            // Check if wager points is greater than zero
            if (request.Points <= 0)
                return BadRequest("Wager points should be greater than zero.");
            // Check if wager is valid between 0 to 9
            if (request == null || request.Number < 0 || request.Number > 9)
                return BadRequest("Wager number should be between 0 to 9.");

            // Check if player has enough points
            var playerPoints = GetPlayerPoints(playerId.Value);
            if (playerPoints < request.Points)
                return BadRequest("Insufficient balance. Your current balance is: " + playerPoints);

            // Generate random number
            var random = new Random();
            var randomNumber = random.Next(10);

            // Update player points based on wager result
            var pointsChange = request.Number == randomNumber ? request.Points * 9 : -request.Points;
            UpdatePlayerPoints(playerId.Value, pointsChange);

            // Return result
            var newPoints = playerPoints + pointsChange;
            var status = request.Number == randomNumber ? "Won" : "Lost";
            var response = new WagerResponse
            {
                Account = newPoints,
                Status = status,
                Points = pointsChange > 0 ? $"+{pointsChange}" : pointsChange.ToString()
            };
            return Ok(response);
        }

        [HttpPost("players")]
        public IActionResult AddPlayer(PlayerRequest request)
        {
            // Check if the email already exists
            if (EmailAddressExists(request.EmailAddress))
            {
                return BadRequest("Email address already exists");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Players (Name, EmailAddress, Points) VALUES (@Name, @EmailAddress, @Points); SELECT SCOPE_IDENTITY();";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", request.Name);
                    command.Parameters.AddWithValue("@EmailAddress", request.EmailAddress);
                    command.Parameters.AddWithValue("@Points", request.Points);
                    var playerId = command.ExecuteScalar();
                    return Ok(new { Id = playerId, Name = request.Name, EmailAddress = request.EmailAddress, Points = request.Points });
                }
            }
        }

        [HttpGet("players")]
        public IActionResult GetPlayers()
        {
            var players = new List<Player>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT Id, Name, EmailAddress, Points FROM Players";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        players.Add(new Player
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            EmailAddress = reader.GetString(2),
                            Points = reader.GetInt32(3)
                        });
                    }
                }
            }
            return Ok(players);
        }

        private int? GetPlayerId(int playerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT Id FROM Players WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", playerId);
                    var playerIdResult = command.ExecuteScalar();
                    return playerIdResult == null ? (int?)null : Convert.ToInt32(playerIdResult);
                }
            }
        }

        private int GetPlayerPoints(int playerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT Points FROM Players WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", playerId);
                    var points = command.ExecuteScalar();
                    return points == null ? 0 : Convert.ToInt32(points);
                }
            }
        }

        private void UpdatePlayerPoints(int playerId, int pointsChange)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "UPDATE Players SET Points = Points + @PointsChange WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PointsChange", pointsChange);
                    command.Parameters.AddWithValue("@Id", playerId);
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool EmailAddressExists(string emailAddress)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM Players WHERE EmailAddress = @EmailAddress";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }
    }
}

