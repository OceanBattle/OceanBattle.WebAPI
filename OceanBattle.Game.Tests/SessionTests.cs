using OceanBattle.DataModel;
using OceanBattle.Game.Models;
using Moq;
using OceanBattle.Game.Abstractions;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.DataModel.Game.Ships;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using OceanBattle.Game.Repositories;
using OceanBattle.DataModel.Game;

namespace OceanBattle.Game.Tests
{
    public class SessionTests
    {
        [Fact]
        public void Session_ShouldCreate()
        {
            // Arrange
            LevelsRepository levelsRepo = new LevelsRepository();
            Level level = levelsRepo
                .GetLevels()
                .Last();

            int size = level.BattlefieldSize;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = ConfigureBattlefieldMock();
                return battlefieldMock.Object;
            });

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            User user = new User();

            // Act
            Session session = new Session(
                user, 
                level,
                factoryMock.Object, 
                gameInterfaceMock.Object);

            // Assert
            Assert.Equal(user, session.Creator);
            Assert.Equal(2, session.Battlefields.Length);
            Assert.NotNull(session.Battlefields[0]);
            Assert.Equal(user, session.Battlefields[0]!.Owner);
            Assert.Equal(size, session.BattlefieldSize);
        }

        [Fact]
        public void AddOponent_ShouldSucceed()
        {
            // Arrange
            User creator = new User();
            LevelsRepository levelsRepo = new LevelsRepository();
            Level level = levelsRepo
                .GetLevels()
                .Last();

            int size = level.BattlefieldSize;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = ConfigureBattlefieldMock();
                return battlefieldMock.Object;
            });

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            Session session = new Session(
                creator, 
                level, 
                factoryMock.Object, 
                gameInterfaceMock.Object);

            gameInterfaceMock.Setup(gameInterface => gameInterface.StartDeployment(session))
                .Returns(Task.CompletedTask);

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
        public void IsActive_ShouldBeTrue()
        {
            // Arrange
            User creator = new User();
            User oponent = new User();

            LevelsRepository levelsRepo = new LevelsRepository();
            Level level = levelsRepo
                .GetLevels()
                .Last();

            int size = level.BattlefieldSize;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = ConfigureBattlefieldMock();
                battlefieldMock.SetupProperty(battlefield => battlefield.IsReady);
                battlefieldMock.SetupGet(battlefield => battlefield.Ships).Returns(() => new List<Ship>
                {
                    new Corvette(100),
                    new Corvette(100),
                    new Frigate(150),
                    new Destroyer(200)
                });

                return battlefieldMock.Object;
            });

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            Session session = new Session(
                creator, 
                level, 
                factoryMock.Object, 
                gameInterfaceMock.Object);

            gameInterfaceMock.Setup(gameInterface => gameInterface.StartDeployment(session))
                .Returns(Task.CompletedTask);

            session.AddOponent(oponent);

            session.Battlefields[0]!.IsReady = true;
            session.Battlefields[1]!.IsReady = true;

            // Act
            bool actual = session.IsActive;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsActive_ShouldBeFalse_ShipsDestroyed()
        {
            // Arrange
            User creator = new User();
            User oponent = new User();
            
            LevelsRepository levelsRepo = new LevelsRepository();
            Level level = levelsRepo
                .GetLevels()
                .Last();

            int size = level.BattlefieldSize;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = ConfigureBattlefieldMock();
                battlefieldMock.SetupProperty(battlefield => battlefield.IsReady);
                battlefieldMock.SetupGet(battlefield => battlefield.Ships).Returns(() => new List<Ship>
                {
                    new Corvette(0),
                    new Corvette(0),
                    new Frigate(0),
                    new Destroyer(0)
                });

                return battlefieldMock.Object;
            });

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            Session session = new Session(
                creator, 
                level, 
                factoryMock.Object, 
                gameInterfaceMock.Object);

            gameInterfaceMock.Setup(gameInterface => gameInterface.StartDeployment(session))
                .Returns(Task.CompletedTask);

            session.AddOponent(oponent);

            session.Battlefields[0]!.IsReady = true;
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
            User creator = new User();

            LevelsRepository levelsRepo = new LevelsRepository();
            Level level = levelsRepo
                .GetLevels()
                .Last();

            int size = level.BattlefieldSize;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = ConfigureBattlefieldMock();
                battlefieldMock.SetupProperty(battlefield => battlefield.IsReady);

                return battlefieldMock.Object;
            });

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            Session session = new Session(
                creator, 
                level, 
                factoryMock.Object, 
                gameInterfaceMock.Object);

            gameInterfaceMock.Setup(gameInterface => gameInterface.StartDeployment(session))
                .Returns(Task.CompletedTask);

            session.Battlefields[0]!.IsReady = true;

            // Act
            bool actual = session.IsActive;

            // Assert
            Assert.False(actual);
        }

        private Mock<IBattlefield> ConfigureBattlefieldMock()
        {
            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
            battlefieldMock.SetupProperty(battlefield => battlefield.Owner);
            battlefieldMock.SetupGet(battlefield => battlefield.GotHit)
                .Returns(new Subject<(int x, int y)>().AsObservable());

            return battlefieldMock;
        }
    }
}