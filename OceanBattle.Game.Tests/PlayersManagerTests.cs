using Moq;
using OceanBattle.DataModel;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Services;

namespace OceanBattle.Game.Tests
{
    public class PlayersManagerTests
    {
        [Fact]
        public void AddAsActive_ShouldSucceed()
        {
            // Arrange
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);

            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            PlayersManager playersManager = new PlayersManager(interfaceMock.Object, sessionsManagerMock.Object);
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
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);

            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            PlayersManager playersManager = new PlayersManager(interfaceMock.Object, sessionsManagerMock.Object);
            playersManager.AddAsActive(user);

            // Act
            playersManager.RemoveFromActive(user);

            // Assert
            Assert.DoesNotContain(user, playersManager.ActivePlayers);
        }
    }
}
