using Moq;
using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
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
            User creator = new User();

            var playersManagerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            // Player has to be active
            playersManagerMock.SetupGet(pm => pm.ActivePlayers)
                              .Returns(new List<User> { creator });

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    sessionMock,
                    factoryMock,
                    playersManagerMock);

            // Act
            IGameSession? actual = sessionsManager.CreateSession(creator, level);

            //Assert
            Assert.NotNull(actual);
            Assert.Contains(sessionMock.Object, sessionsManager.Sessions);
            Assert.Equal(sessionMock.Object, actual);
            factoryMock.Verify(f => f.Create(creator, level), Times.Once());
            sessionMock.VerifyGet(s => s.Completed, Times.Once());
        }

        [Fact]
        public void CreateSession_ShouldFail_CreatorInvolvedAsOponent()
        {
            // Arrange
            User creator = new User();
            User somePlayer = new User();

            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            var somePlayersSessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            somePlayersSessionMock.SetupGet(s => s.Creator)
                                   .Returns(somePlayer);

            somePlayersSessionMock.SetupGet(s => s.Oponent)
                                  .Returns(creator);

            somePlayersSessionMock.SetupGet(s => s.Completed)
                                  .Returns(new Subject<IGameSession>());

            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            factoryMock.Setup(f => f.Create(somePlayer, level))
                       .Returns(somePlayersSessionMock.Object);

            var playersManagerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            playersManagerMock.Setup(pm => pm.GetPlayer(somePlayer.Id))
                              .Returns(somePlayer);

            playersManagerMock.SetupGet(pm => pm.ActivePlayers)
                              .Returns(new List<User> { creator, somePlayer });

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    sessionMock,
                    factoryMock,
                    playersManagerMock);

            sessionsManager.CreateSession(somePlayer, level);

            // Act
            IGameSession? actual = sessionsManager.CreateSession(creator, level);

            // Assert
            Assert.Null(actual);
            Assert.DoesNotContain(actual, sessionsManager.Sessions);
            factoryMock.Verify(f => f.Create(creator, level), Times.Never());
            sessionMock.VerifyGet(s => s.Completed, Times.Never());
        }

        [Fact]
        public void CreateSession_ShouldFail_CreatorInvolvedAsCreator()
        {
            // Arrange
            User creator = new User();

            var playersManagerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            playersManagerMock.SetupGet(pm => pm.ActivePlayers)
                  .Returns(new List<User> { creator });

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    sessionMock,
                    factoryMock, 
                    playersManagerMock);

            sessionsManager.CreateSession(creator, level);

            // Act
            IGameSession? actual = sessionsManager.CreateSession(creator, level);

            // Assert
            Assert.Null(actual);
            Assert.DoesNotContain(actual, sessionsManager.Sessions);
            factoryMock.Verify(f => f.Create(creator, level), Times.Once());
            sessionMock.VerifyGet(s => s.Completed, Times.Once());
        }

        [Fact]
        public void CreateSession_ShouldFail_CreatorInactive()
        {
            // Arrange
            User creator = new User();

            var playersManagerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            playersManagerMock.SetupGet(pm => pm.ActivePlayers)
                              .Returns(new List<User>());

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    sessionMock,
                    factoryMock,
                    playersManagerMock);

            // Act
            IGameSession? actual = sessionsManager.CreateSession(creator, level);

            // Assert
            Assert.Null(actual);
            Assert.DoesNotContain(actual, sessionsManager.Sessions);
            factoryMock.Verify(f => f.Create(creator, level), Times.Never());
            sessionMock.VerifyGet(s => s.Completed, Times.Never());
        }

        [Fact]
        public void OnCompleted_ShouldRemoveSession()
        {
            // Arrange
            User creator = new User();
            Subject<IGameSession> completed = new Subject<IGameSession>();

            var playersManagerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            playersManagerMock.SetupGet(pm => pm.ActivePlayers)
                              .Returns(new List<User> { creator });

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    sessionMock,
                    factoryMock,
                    playersManagerMock,
                    completed);

            // Act
            completed.OnNext(sessionMock.Object);

            // Assert
            Assert.DoesNotContain(sessionMock.Object, sessionsManager.Sessions);
        }

        [Fact]
        public void RemoveSessions_ShouldSucceed()
        {
            // Arrange
            User creator = new User();

            var playersManagerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            sessionMock.Setup(s => s.Creator)
                       .Returns(creator);

            playersManagerMock.SetupGet(pm => pm.ActivePlayers)
                  .Returns(new List<User> { creator });

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    sessionMock,
                    factoryMock,
                    playersManagerMock);

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
            User creator = new User();

            var playersManagerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            sessionMock.Setup(s => s.Creator)
                       .Returns(creator);

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    sessionMock,
                    factoryMock,
                    playersManagerMock);

            // Act
            sessionsManager.EndSessions(creator);

            // Assert
            Assert.DoesNotContain(sessionsManager.Sessions, s => s.Creator.Id == creator.Id);
        }

        [Fact]
        public void FindSession_ShouldSucceed()
        {
            // Arrange
            User creator = new User();

            var battlefieldMock = new Mock<IBattlefield>();
            var playersManagerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            battlefieldMock.SetupGet(b => b.Owner)
                           .Returns(creator);

            sessionMock.SetupGet(s => s.Battlefields)
                       .Returns(new[] { battlefieldMock.Object, null });

            playersManagerMock.SetupGet(pm => pm.ActivePlayers)
                              .Returns(new List<User> { creator });

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    sessionMock,
                    factoryMock,
                    playersManagerMock);

            sessionsManager.CreateSession(creator, level);

            // Act
            IGameSession? actual = sessionsManager.FindSession(creator);

            // Assert
            Assert.Equal(sessionMock.Object, actual);
        }

        [Fact]
        public void FindSession_ShouldFail()
        {
            // Arrange
            User creator = new User();

            var battlefieldMock = new Mock<IBattlefield>();
            var playersManagerMock = new Mock<IPlayersManager>(MockBehavior.Strict);
            var factoryMock = new Mock<ISessionFactory>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            battlefieldMock.SetupGet(b => b.Owner)
                           .Returns(creator);

            sessionMock.SetupGet(s => s.Battlefields)
                       .Returns(new[] { battlefieldMock.Object, null });

            playersManagerMock.SetupGet(pm => pm.ActivePlayers)
                              .Returns(new List<User> { creator });

            SessionsManager sessionsManager =
                Arrange_SessionsManager(
                    creator,
                    sessionMock,
                    factoryMock,
                    playersManagerMock);

            // Act
            IGameSession? actual = sessionsManager.FindSession(creator);

            // Assert
            Assert.Null(actual);
        }

        #region private helpers

        private readonly Level level = new Level { BattlefieldSize = 12 };

        private SessionsManager Arrange_SessionsManager(
            User creator,
            Mock<IGameSession> sessionMock,
            Mock<ISessionFactory> factoryMock,
            Mock<IPlayersManager> playersManagerMock)
            => Arrange_SessionsManager(
                creator,
                sessionMock,
                factoryMock,
                playersManagerMock,
                new Subject<IGameSession>());

        private SessionsManager Arrange_SessionsManager(
            User creator,
            Mock<IGameSession> sessionMock,
            Mock<ISessionFactory> factoryMock,
            Mock<IPlayersManager> playersManagerMock,
            Subject<IGameSession> completed)
        {
            // Has to subscribe to completed
            sessionMock.SetupGet(session => session.Completed)
                       .Returns(() => completed.AsObservable());

            // Has to check for player being creator of session
            sessionMock.SetupGet(session => session.Creator)
                       .Returns(creator);

            // Has to create session
            factoryMock.Setup(factory => factory.Create(creator, level))
                       .Returns(sessionMock.Object);

            // Has to find player with this id
            playersManagerMock.Setup(manager => manager.GetPlayer(creator.Id))
                              .Returns(creator);

            return new SessionsManager(factoryMock.Object, playersManagerMock.Object);
        }

        #endregion
    }
}
