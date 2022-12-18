using Moq;
using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Services;
using System.Reactive.Linq;
using System.Reactive.Subjects;

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
            sessionMock.SetupGet(session => session.Completed).Returns(new Subject<IGameSession>().AsObservable());

            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(creator, level)).Returns(sessionMock.Object);

            var managerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            managerMock.Setup(manager => manager.GetPlayer(creator.Id)).Returns(creator);

            SessionsManager sessionsManager = new SessionsManager(factoryMock.Object, managerMock.Object);

            // Act
            var actual = sessionsManager.CreateSession(creator, level);

            //Assert
            Assert.Contains(sessionMock.Object, sessionsManager.Sessions);
            Assert.Equal(sessionMock.Object, actual);
        }
    }
}
