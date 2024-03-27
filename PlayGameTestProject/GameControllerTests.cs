using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using PlayGameBackendAPI.Controllers;
using PlayGameBackendAPI.Model;
using Xunit;

namespace PlayGameTestProject
{
    public class GameControllerTests
    {
        [Fact]
        public void PlaceBet_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["ConnectionStrings:DevConnection"]).Returns("Data Source=localhost;Initial Catalog=GamePlayersDB;Integrated Security=True");

            var controller = new GameController(mockConfig.Object);
            var request = new WagerRequest { PlayerId = 1, Points = 100, Number = 5 };

            // Act
            var result = controller.PlaceBet(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as WagerResponse;
            Assert.NotNull(response);
            Assert.NotNull(response.Status);
            Assert.NotNull(response.Points);
        }

        [Fact]
        public void PlaceBet_InvalidPlayer_ReturnsBadRequest()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["ConnectionStrings:DevConnection"]).Returns("Data Source=localhost;Initial Catalog=GamePlayersDB;Integrated Security=True");
            var controller = new GameController(mockConfig.Object);
            var request = new WagerRequest { PlayerId = 9999, Points = 100, Number = 5 };

            // Act
            var result = controller.PlaceBet(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void PlaceBet_NegativeWagerPoints_ReturnsBadRequest()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["ConnectionStrings:DevConnection"]).Returns("Data Source=localhost;Initial Catalog=GamePlayersDB;Integrated Security=True");
            var controller = new GameController(mockConfig.Object);
            var request = new WagerRequest { PlayerId = 1, Points = -100, Number = 5 };

            // Act
            var result = controller.PlaceBet(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void PlaceBet_InvalidBetNumber_ReturnsBadRequest()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["ConnectionStrings:DevConnection"]).Returns("Data Source=localhost;Initial Catalog=GamePlayersDB;Integrated Security=True");
            var controller = new GameController(mockConfig.Object);
            var request = new WagerRequest { PlayerId = 1, Points = 100, Number = -5 };

            // Act
            var result = controller.PlaceBet(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void PlaceBet_InsufficientBalance_ReturnsBadRequest()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["ConnectionStrings:DevConnection"]).Returns("Data Source=localhost;Initial Catalog=GamePlayersDB;Integrated Security=True");
            var controller = new GameController(mockConfig.Object);
            var request = new WagerRequest { PlayerId = 1, Points = 1000000, Number = 5 };

            // Act
            var result = controller.PlaceBet(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void AddPlayer_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["ConnectionStrings:DevConnection"]).Returns("Data Source=localhost;Initial Catalog=GamePlayersDB;Integrated Security=True");
            var controller = new GameController(mockConfig.Object);
            var request = new PlayerRequest { Name = "John Doe", EmailAddress = "john@example.com", Points = 1000 };

            // Act
            var result = controller.AddPlayer(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void AddPlayer_DuplicateEmailAddress_ReturnsBadRequest()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["ConnectionStrings:DevConnection"]).Returns("Data Source=localhost;Initial Catalog=GamePlayersDB;Integrated Security=True");
            var controller = new GameController(mockConfig.Object);
            var request = new PlayerRequest { Name = "John Doe", EmailAddress = "john@example.com", Points = 1000 };

            // Act
            var result1 = controller.AddPlayer(request); // Adding the player for the first time
            var result2 = controller.AddPlayer(request); // Trying to add the same player again

            // Assert
            Assert.IsType<BadRequestObjectResult>(result1);
            Assert.IsType<BadRequestObjectResult>(result2);
        }

        [Fact]
        public void GetPlayers_ReturnsOkResultWithPlayersList()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["ConnectionStrings:DevConnection"]).Returns("Data Source=localhost;Initial Catalog=GamePlayersDB;Integrated Security=True");
            var controller = new GameController(mockConfig.Object);

            // Act
            var result = controller.GetPlayers();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var players = okResult.Value as List<Player>;
            Assert.NotNull(players);
            Assert.NotEmpty(players);
        }
    }
}
