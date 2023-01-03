using Moq;
using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.DataModel.Game.Ships;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Models;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OceanBattle.Game.Tests
{
    public class SessionTests
    {
        [Fact]
        public void Session_ShouldCreate()
        {
            // Arrange
            var factoryMock = CreateFactoryMock();
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            User user = new User();

            // Act
            Session session = 
                CreateSession(user, factoryMock.Object, gameInterfaceMock.Object);

            // Assert
            Assert.Equal(user, session.Creator);
            Assert.Equal(2, session.Battlefields.Length);
            Assert.NotNull(session.Battlefields[0]);
            Assert.Equal(user, session.Battlefields[0]!.Owner);
            Assert.Equal(level.BattlefieldSize, session.BattlefieldSize);
            Assert.Equal(level, session.Level);
        }

        [Fact]
        public void AddOponent_ShouldSucceed()
        {
            // Arrange
            var factoryMock = CreateFactoryMock();
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = CreateSession(factoryMock.Object, gameInterfaceMock);
            User oponent = new User();

            // Act
            session.AddOponent(oponent);

            // Assert
            gameInterfaceMock.Verify(gI => gI.StartDeployment(session), Times.Once());
            Assert.Equal(oponent, session.Oponent);
            Assert.NotNull(session.Battlefields[1]);
            Assert.Equal(oponent, session.Battlefields[1]!.Owner);
        }

        [Fact]
        public void AddOponent_ShouldFail()
        {
            // Arrange
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeActiveSession(gameInterfaceMock);
            User oponent = new User();

            // Act
            session.AddOponent(oponent);

            // Assert
            Assert.NotEqual(oponent, session.Oponent);
            gameInterfaceMock.Verify(gI => gI.StartDeployment(session), Times.Once());
        }

        [Fact]
        public void IsActive_ShouldBeTrue()
        {
            // Arrange
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeActiveSession(gameInterfaceMock);

            // Act
            bool actual = session.IsActive;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsActive_ShouldBeFalse_ShipsDestroyed()
        {
            // Arrange
            var factoryMock = CreateFactoryMock(
                battlefield => battlefield.SetupProperty(battlefield => battlefield.IsReady)
                                          .SetupGet(battlefield => battlefield.Ships)
                                          .Returns(destroyedShips));

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = CreateSession(factoryMock.Object, gameInterfaceMock);
            User oponent = new User();

            session.AddOponent(oponent);

            session.Battlefields[0]!.IsReady = true;
            session.Battlefields[1]!.IsReady = true;

            // Act
            bool actual = session.IsActive;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsActive_ShouldBeFalse_NotReady()
        {
            // Arrange
            var factoryMock = CreateFactoryMock(
                battlefield => battlefield.SetupProperty(battlefield => battlefield.IsReady)
                                          .SetupGet(battlefield => battlefield.Ships)
                                          .Returns(destroyedShips));

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = CreateSession(factoryMock.Object, gameInterfaceMock);
            User oponent = new User();

            session.AddOponent(oponent);
            session.Battlefields[1]!.IsReady = true;

            // Act
            bool actual = session.IsActive;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsActive_ShouldBeFalse_NoOponent()
        {
            // Arrange
            var factoryMock = CreateFactoryMock(
                battlefield => battlefield.SetupProperty(battlefield => battlefield.IsReady)
                                          .SetupGet(battlefield => battlefield.Ships)
                                          .Returns(functioningShips));

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = CreateSession(factoryMock.Object, gameInterfaceMock);

            session.Battlefields[0]!.IsReady = true;

            // Act
            bool actual = session.IsActive;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void GetBattlefield_ShouldSucceed()
        {
            // Arrange
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeActiveSession(gameInterfaceMock);
            User creator = session.Creator;

            // Act
            IBattlefield? battlefield = session.GetBattlefield(creator);

            // Assert
            Assert.NotNull(battlefield);
            Assert.Equal(session.Battlefields[0], battlefield);
        }

        [Fact]
        public void GetBattlefield_ShouldFail_UninvolvedUser()
        {
            // Arrange
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeActiveSession(gameInterfaceMock);
            User player = new User();

            // Act
            IBattlefield? battlefield = session.GetBattlefield(player);

            // Assert
            Assert.Null(battlefield);
        }

        [Fact]
        public void GetOponentBattlefield_ShouldSucceed()
        {
            // Arrange
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeActiveSession(gameInterfaceMock);
            User oponent = session.Oponent!;

            // Act
            IBattlefield? battlefield = session.GetOponentBattlefield(oponent);

            // Assert
            Assert.NotNull(battlefield);
            Assert.Equal(session.Battlefields[0], battlefield);
        }

        [Fact]
        public void GetOponentBattlefield_ShouldFail_UninvolvedUser()
        {
            // Arrange
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeActiveSession(gameInterfaceMock);
            User player = new User();

            // Act
            IBattlefield? battlefield = session.GetOponentBattlefield(player);

            // Assert
            Assert.Null(battlefield);
        }

        [Fact]
        public void Completed_ShouldEmitOnNext()
        {
            // Arrange
            Subject<(int x, int y)> gotHit = new Subject<(int x, int y)>();
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeActiveSession(gameInterfaceMock, gotHit);

            gameInterfaceMock.Setup(gI => gI.EndGame(session))
                .Returns(Task.CompletedTask);

            bool actual = false;
            session.Completed.Subscribe(coordinates => actual = true);

            // Act
            gotHit.OnCompleted();

            // Assert
            Assert.True(actual);            
        }

        [Fact]
        public void OnCompleted_ShouldCallGameEnd()
        {
            // Arrange
            Subject<(int x, int y)> gotHit = new Subject<(int x, int y)>();
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeActiveSession(gameInterfaceMock, gotHit);

            gameInterfaceMock.Setup(gI => gI.EndGame(session))
                .Returns(Task.CompletedTask);

            // Act
            gotHit.OnCompleted();

            // Assert
            gameInterfaceMock.Verify(gI => gI.EndGame(session), Times.AtLeastOnce());
        }

        [Fact]
        public void OnHit_ShouldCallGotHit()
        {
            // Arrange
            Subject<(int x, int y)> gotHit = new Subject<(int x, int y)>();
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeActiveSession(gameInterfaceMock, gotHit);

            (int x, int y) coordinates = (0, 0);

            gameInterfaceMock.Setup(gI => gI.GotHit(session, session.Creator, coordinates))
                .Returns(Task.CompletedTask);

            gameInterfaceMock.Setup(gI => gI.GotHit(session, session.Oponent!, coordinates))
                .Returns(Task.CompletedTask);
            
            // Act
            gotHit.OnNext(coordinates);

            // Assert
            gameInterfaceMock.Verify(
                gI => gI.GotHit(session, session.Creator, coordinates), Times.Once());

            gameInterfaceMock.Verify(
                gI => gI.GotHit(session, session.Oponent!, coordinates), Times.Once());
        }

        [Fact]
        public void Next_ShouldBeSet()
        {
            // Arrange
            Subject<(int x, int y)> gotHit = new Subject<(int x, int y)>();
            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Session session = ArrangeSessionWithoutOponent(gameInterfaceMock, gotHit);

            (int x, int y) coordinates = (0, 0);

            gameInterfaceMock.Setup(gI => gI.GotHit(session, session.Creator, coordinates))
                .Returns(Task.CompletedTask);

            // Act
            gotHit.OnNext(coordinates);

            // Assert
            Assert.Equal(session.Creator, session.Next);
        }

        [Fact]
        public void StatusChanged_ShouldCallStartGame()
        {
            // Arrange
            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Subject<bool> creatorStatusChanged = new Subject<bool>();

            Session session = Arrange_StatusChanged(
                true,
                interfaceMock,
                creatorStatusChanged);

            // Act
            creatorStatusChanged.OnNext(true);
            creatorStatusChanged.OnNext(true);

            // Assert
            interfaceMock.Verify(gi => gi.FinishDeployment(session), Times.Once());
            interfaceMock.Verify(gi => gi.StartGame(session), Times.Once());
        }

        [Fact]
        public void StatusChanged_ShouldNotCallStartGame_OponentNotReady()
        {
            // Arrange
            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            Subject<bool> creatorStatusChanged = new Subject<bool>();

            Session session = Arrange_StatusChanged(
                false,
                interfaceMock,
                creatorStatusChanged);

            // Act
            creatorStatusChanged.OnNext(true);

            // Assert
            interfaceMock.Verify(gi => gi.FinishDeployment(session), Times.Never());
            interfaceMock.Verify(gi => gi.StartGame(session), Times.Never());
        }

        #region private helpers

        private const int dimensions = 20;
        private Level level = new Level
        {
            BattlefieldSize = dimensions,
            AvailableTypes = new Dictionary<Type, int>
            {
                { typeof(Battleship), 5 },
                { typeof(Cruiser),    5 },
                { typeof(Destroyer),  5 },
                { typeof(Frigate),    5 },
                { typeof(Corvette),   5 },
            }
        };

        private List<Ship> functioningShips => new List<Ship>
        {
            new Corvette(100),
            new Corvette(100),
            new Frigate(150),
            new Destroyer(200)
        };

        private List<Ship> destroyedShips => new List<Ship>
        {
            new Corvette(0),
            new Corvette(0),
            new Frigate(0),
            new Destroyer(0)
        };

        private Session Arrange_StatusChanged(
            bool oponentReady,
            Mock<IGameInterface> interfaceMock,
            Subject<bool> statusChanged)
        {
            int battlefieldCount = 0;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(f => f.Create(level))
                       .Returns(() =>
                       {
                           var battlefieldMock = CreateBattlefieldMock();

                           if (battlefieldCount < 1)
                               battlefieldMock.SetupGet(f => f.StatusChanged)
                                              .Returns(() => statusChanged.AsObservable());
                           else
                               battlefieldMock.SetupGet(b => b.IsReady)
                                              .Returns(oponentReady);

                           battlefieldCount++;

                           return battlefieldMock.Object;
                       });

            Session session =
                CreateSession(factoryMock.Object, interfaceMock);

            var sequence = new MockSequence();

            interfaceMock.InSequence(sequence)
                         .Setup(gi => gi.FinishDeployment(session))
                         .Returns(Task.CompletedTask);

            interfaceMock.InSequence(sequence)
                         .Setup(gi => gi.StartGame(session))
                         .Returns(Task.CompletedTask);

            session.AddOponent(new User());

            return session;
        }

        private Session ArrangeSessionWithoutOponent(
            Mock<IGameInterface> gameInterfaceMock,
            Subject<(int x, int y)> gotHit)
        {
            var factoryMock = CreateFactoryMock(
                battlefield => battlefield.SetupProperty(battlefield => battlefield.IsReady)
                              .SetupGet(battlefield => battlefield.Ships)
                              .Returns(functioningShips),
                gotHit);

            Session session = CreateSession(factoryMock.Object, gameInterfaceMock);

            session.Battlefields[0]!.IsReady = true;

            return session;
        }

        private Session ArrangeActiveSession(
            Mock<IGameInterface> gameInterfaceMock, 
            Subject<(int x, int y)> gotHit)
        {
            Session session = 
                ArrangeSessionWithoutOponent(gameInterfaceMock, gotHit);

            User oponent = new User();
            session.AddOponent(oponent);

            session.Battlefields[1]!.IsReady = true;

            return session;
        }

        private Session ArrangeActiveSession(Mock<IGameInterface> gameInterfaceMock)
            => ArrangeActiveSession(gameInterfaceMock, new Subject<(int x, int y)>());

        private void SetupGameInterfaceMock(Mock<IGameInterface> mock, Session session) 
            => mock.Setup(gameInterface => gameInterface.StartDeployment(session))
                   .Returns(Task.CompletedTask);

        private Mock<IBattlefieldFactory> CreateFactoryMock() 
            => CreateFactoryMock(battlefield => { });

        private Mock<IBattlefieldFactory> CreateFactoryMock(
            Action<Mock<IBattlefield>> battlefieldSetup, 
            Subject<(int x, int y)> gotHit)
        {
            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(level))
                .Returns(
                () =>
                {
                    var battlefieldMock = CreateBattlefieldMock(gotHit);
                    battlefieldSetup(battlefieldMock);

                    return battlefieldMock.Object;
                });

            return factoryMock;
        }

        private Mock<IBattlefieldFactory> CreateFactoryMock(Action<Mock<IBattlefield>> battlefieldSetup) 
            => CreateFactoryMock(battlefieldSetup, new Subject<(int x, int y)>());

        private Session CreateSession(IBattlefieldFactory factory, Mock<IGameInterface> gameInterfaceMock)
        {
            Session session = CreateSession(factory, gameInterfaceMock.Object);
            SetupGameInterfaceMock(gameInterfaceMock, session);

            return session;
        }

        private Session CreateSession(IBattlefieldFactory factory, IGameInterface gameInterface)
            => CreateSession(new User(), factory, gameInterface);

        private Session CreateSession(
            User creator, 
            IBattlefieldFactory factory, 
            IGameInterface gameInterface) 
            => new Session(creator, level, factory, gameInterface);

        private Mock<IBattlefield> CreateBattlefieldMock()
            => CreateBattlefieldMock(new Subject<(int x, int y)>());

        private Mock<IBattlefield> CreateBattlefieldMock(Subject<(int x, int y)> gotHit)
        {
            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);

            battlefieldMock.SetupProperty(battlefield => battlefield.Owner)
                           .SetupGet(battlefield => battlefield.GotHit)
                           .Returns(() => gotHit.AsObservable());

            battlefieldMock.SetupGet(battlefield => battlefield.StatusChanged)
                           .Returns(new Subject<bool>().AsObservable());

            return battlefieldMock;
        }

        #endregion
    }
}