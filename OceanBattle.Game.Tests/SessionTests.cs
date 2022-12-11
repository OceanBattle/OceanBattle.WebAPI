using OceanBattle.DataModel;
using OceanBattle.Game.Models;
using Moq;
using OceanBattle.Game.Abstractions;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.DataModel.Game.Ships;

namespace OceanBattle.Game.Tests
{
    public class SessionTests
    {
        [Fact]
        public void Session_ShouldCreate()
        {
            // Arrange
            int size = 12;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
                battlefieldMock.SetupProperty(battlefield => battlefield.Owner);

                return battlefieldMock.Object;
            });

            User user = new User();

            // Act
            Session session = new Session(user, size, factoryMock.Object);

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
            int size = 22;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
                battlefieldMock.SetupProperty(battlefield => battlefield.Owner);

                return battlefieldMock.Object;
            });

            Session session = new Session(creator, size, factoryMock.Object);
            User oponent = new User();

            // Act
            session.AddOponent(oponent);

            // Assert
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
            int size = 12;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
                battlefieldMock.SetupProperty(battlefield => battlefield.Owner);
                battlefieldMock.SetupGet(battlefield => battlefield.Ships).Returns(() => new List<Ship>
                {
                    new Corvette(100),
                    new Corvette(100),
                    new Frigate(150),
                    new Destroyer(200)
                });

                return battlefieldMock.Object;
            });

            Session session = new Session(creator, size, factoryMock.Object);

            session.AddOponent(oponent);

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
            int size = 12;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
                battlefieldMock.SetupProperty(battlefield => battlefield.Owner);
                battlefieldMock.SetupGet(battlefield => battlefield.Ships).Returns(() => new List<Ship>
                {
                    new Corvette(0),
                    new Corvette(0),
                    new Frigate(0),
                    new Destroyer(0)
                });

                return battlefieldMock.Object;
            });

            Session session = new Session(creator, size, factoryMock.Object);

            session.AddOponent(oponent);

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
            int size = 10;

            var factoryMock = new Mock<IBattlefieldFactory>(MockBehavior.Strict);
            factoryMock.Setup(factory => factory.Create(size, size)).Returns(() =>
            {
                var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
                battlefieldMock.SetupProperty(battlefield => battlefield.Owner);

                return battlefieldMock.Object;
            });

            Session session = new Session(creator, size, factoryMock.Object);

            // Act
            bool actual = session.IsActive;

            // Assert
            Assert.False(actual);
        }
    }
}