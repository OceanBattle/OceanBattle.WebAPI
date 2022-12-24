using Moq;
using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Services;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection.Emit;

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

            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    level,
                    sessionMock,
                    factoryMock);

            // Act
            var actual = sessionsManager.CreateSession(creator, level);

            //Assert
            Assert.Contains(sessionMock.Object, sessionsManager.Sessions);
            Assert.Equal(sessionMock.Object, actual);
            factoryMock.Verify(f => f.Create(creator, level), Times.Once());
        }

        [Fact]
        public void RemoveSessions_ShouldSucceed()
        {
            // Arrange
            int size = 12;
            User creator = new User();
            Level level = new Level { BattlefieldSize = size };

            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            sessionMock.Setup(s => s.Creator)
                       .Returns(creator);

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    level,
                    sessionMock,
                    factoryMock);

            sessionsManager.CreateSession(creator.Id, level);
            sessionsManager.CreateSession(creator.Id, level);

            // Act
            sessionsManager.EndSessions(creator);

            // Assert
            Assert.DoesNotContain(sessionsManager.Sessions, s => s.Creator.Id == creator.Id);          
        }

        [Fact]
        public void RemoveSessions_ShouldFail()
        {
            // Arrange
            int size = 12;
            User creator = new User();
            Level level = new Level { BattlefieldSize = size };

            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            sessionMock.Setup(s => s.Creator)
                       .Returns(creator);

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    level,
                    sessionMock,
                    factoryMock);

            // Act
            sessionsManager.EndSessions(creator);

            // Assert
            Assert.DoesNotContain(sessionsManager.Sessions, s => s.Creator.Id == creator.Id);
        }

        #region private helpers

        private SessionsManager Arrange_SessionsManager(
            User creator,
            Level level,
            Mock<IGameSession> sessionMock,
            Mock<ISessionFactory> factoryMock)
        {
            sessionMock.SetupGet(session => session.Completed)
                       .Returns(new Subject<IGameSession>().AsObservable());

            factoryMock.Setup(factory => factory.Create(creator, level))
                       .Returns(sessionMock.Object);

            var managerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            managerMock.Setup(manager => manager.GetPlayer(creator.Id))
                       .Returns(creator);

            return new SessionsManager(factoryMock.Object, managerMock.Object);
        }

        #endregion
    }
}
