using Moq;
using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Services;

namespace OceanBattle.Game.Tests
{
    public class SessionsManagerTests
    {
        [Fact]
        public void CreateSession_ShouldSucceed()
        {
            // Arrange
            int size = 12;
            User creator = new User();
            Level level = new Level { BattlefieldSize = size };

            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            
            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(creator, size)).Returns(sessionMock.Object);

            SessionsManager sessionsManager = new SessionsManager(factoryMock.Object);

            // Act
            var actual = sessionsManager.CreateSession(creator, level);

            //Assert
            Assert.Contains(sessionMock.Object, sessionsManager.Sessions);
            Assert.Equal(sessionMock.Object, actual);
        }
    }
}
