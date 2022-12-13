using OceanBattle.DataModel;
using OceanBattle.Game.Services;

namespace OceanBattle.Game.Tests
{
    public class PlayersManagerTests
    {
        [Fact]
        public void AddAsActive_ShouldSucceed()
        {
            // Arrange
            PlayersManager playersManager = new PlayersManager();
            User user1 = new User();
            User user2 = new User();

            // Act
            playersManager.AddAsActive(user1);
            playersManager.AddAsActive(user1);
            playersManager.AddAsActive(user2);
            playersManager.AddAsActive(user2);

            // Assert
            Assert.Single(playersManager.ActivePlayers, user1);
            Assert.Single(playersManager.ActivePlayers, user2);
        }

        [Fact]
        public void RemoveFromActive_ShouldSucceed()
        {
            // Arrange
            User user = new User();
            PlayersManager playersManager = new PlayersManager();
            playersManager.AddAsActive(user);

            // Act
            playersManager.RemoveFromActive(user);

            // Assert
            Assert.DoesNotContain(user, playersManager.ActivePlayers);
        }
    }
}
