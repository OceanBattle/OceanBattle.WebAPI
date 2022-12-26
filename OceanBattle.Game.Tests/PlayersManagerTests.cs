using Moq;
using OceanBattle.DataModel;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Services;
using System.Reflection;

namespace OceanBattle.Game.Tests
{
    public class PlayersManagerTests
    {
        [Fact]
        public void AddAsActive_ShouldSucceed()
        {
            // Arrange
            PlayersManager playersManager = 
                ArrangePlayersManager();

            User user = new User();

            // Act
            playersManager.AddAsActive(user);

            // Assert
            Assert.Single(playersManager.ActivePlayers, user);
        }

        [Fact]
        public void AddAsActive_ShouldFail()
        {
            // Arrange
            PlayersManager playersManager = 
                ArrangePlayersManager();
            User user = new User();

            playersManager.AddAsActive(user);

            User user2 = new User { Id = user.Id };

            // Act
            playersManager.AddAsActive(user2);

            // Assert
            Assert.DoesNotContain(user2, playersManager.ActivePlayers);
        }

        [Fact]
        public void RemoveFromActive_ShouldSucceed()
        {
            // Arrange
            User user = new User();

            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
            sessionsManagerMock.Setup(sm => sm.EndSessions(user.Id));

            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            PlayersManager playersManager =
                new PlayersManager(interfaceMock.Object, sessionsManagerMock.Object);

            playersManager.AddAsActive(user);

            User user2 = new User { Id = user.Id };

            // Act
            playersManager.RemoveFromActive(user2);

            // Assert
            Assert.DoesNotContain(user, playersManager.ActivePlayers);
            sessionsManagerMock.Verify(sm => sm.EndSessions(user.Id), Times.Once());
        }

        [Fact]
        public void IsPlayerActive_ShouldBeTrue()
        {
            // Arrange
            PlayersManager playersManager = 
                ArrangePlayersManager();

            User user = new User();

            playersManager.AddAsActive(user);

            // Act
            bool actual = 
                playersManager.IsPlayerActive(user.Id);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsPlayerActive_ShouldBeFalse()
        {
            // Arrange
            PlayersManager playersManager = 
                ArrangePlayersManager();

            User user = new User();

            // Act
            bool actual = 
                playersManager.IsPlayerActive(user.Id);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void GetPlayer_ShouldSucceed()
        {
            // Arrange
            PlayersManager playersManager = 
                ArrangePlayersManager();

            User player = new User();

            playersManager.AddAsActive(player);

            // Act
            User? actual =
                playersManager.GetPlayer(player.Id);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(player, actual);
        }

        [Fact]
        public void GetPlayer_ShouldFail() 
        {
            // Arrange
            PlayersManager playersManager = 
                ArrangePlayersManager();

            User player = new User();
            User player2 = new User();

            playersManager.AddAsActive(player);

            // Act
            User? actual = 
                playersManager.GetPlayer(player2.Id);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void InvitePlayer_ShouldSucced()
        {
            // Arrange
            User sender = new User();
            User reciever = new User();

            List<string> invitedPlayers = new List<string>();

            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
            var interfaceMock = SetupGameInterface(reciever, sender);

            PlayersManager playersManager = 
                Arange_InvitePlayer(
                    sender,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock,
                    invitedPlayers);

            playersManager.AddAsActive(sender);
            playersManager.AddAsActive(reciever);

            // Act
            playersManager.InvitePlayer(reciever, sender);

            // Assert
            interfaceMock.Verify(gI => gI.SendInvite(reciever.Id, sender), Times.Once());
            sessionsManagerMock.Verify(sm => sm.FindSession(sender.Id), Times.Once());
            sessionMock.VerifyGet(s => s.InvitedPlayersIDs, Times.AtLeastOnce());
            Assert.Contains(reciever.Id, invitedPlayers);
        }

        [Fact]
        public void InvitePlayer_ShouldFail_PlayerInactive()
        {
            // Arrange
            User sender = new User();
            User reciever = new User();

            List<string> invitedPlayers = new List<string>();

            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
            var interfaceMock = SetupGameInterface(reciever, sender);

            PlayersManager playersManager =
                Arange_InvitePlayer(
                    sender,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock,
                    invitedPlayers);

            playersManager.AddAsActive(sender);

            // Act
            playersManager.InvitePlayer(reciever, sender);

            // Assert
            interfaceMock.Verify(gI => gI.SendInvite(reciever.Id, sender), Times.Never());
            sessionMock.VerifyGet(s => s.InvitedPlayersIDs, Times.Never());
            Assert.DoesNotContain(reciever.Id, invitedPlayers);
        }

        [Fact]
        public void InvitePlayer_ShouldFail_SenderInactive()
        {
            // Arrange
            User sender = new User();
            User reciever = new User();

            List<string> invitedPlayers = new List<string>();

            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
            var interfaceMock = SetupGameInterface(reciever, sender);

            PlayersManager playersManager =
                Arange_InvitePlayer(
                    sender,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock,
                    invitedPlayers);

            playersManager.AddAsActive(reciever);

            // Act
            playersManager.InvitePlayer(reciever, sender);

            // Assert
            interfaceMock.Verify(gI => gI.SendInvite(reciever.Id, sender), Times.Never());
            sessionMock.VerifyGet(s => s.InvitedPlayersIDs, Times.Never());
            Assert.DoesNotContain(reciever.Id, invitedPlayers);
        }

        [Fact]
        public void AcceptInvite_ShouldSucceed()
        {
            // Arrange
            User reciever = new User();
            User sender = new User();

            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>();
            var interfaceMock = new Mock<IGameInterface>();

            // Reciver has to be on the list of invited players.
            sessionMock.SetupGet(s => s.InvitedPlayersIDs)
                       .Returns(new List<string> { reciever.Id });

            // Session of invite sender cannot have any oponent already added.
            sessionMock.SetupGet(s => s.Oponent)
                       .Returns(() => null);

            // Reciver of invite cannot be involved in any session.
            sessionsManagerMock.Setup(sm => sm.FindSession(reciever.Id))
                               .Returns(() => null);

            // Session of invite sender has to exist.
            sessionsManagerMock.Setup(sm => sm.FindSession(sender.Id))
                               .Returns(sessionMock.Object);

            PlayersManager playersManager =
                Arrange_AcceptInvite(
                    reciever,
                    battlefieldMock,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock);

            // Sender of invite has to be active
            playersManager.AddAsActive(sender);

            // Reciever of invite has to be active.
            playersManager.AddAsActive(reciever);

            // Act
            IBattlefield? actual = playersManager.AcceptInvite(reciever, sender);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(battlefieldMock.Object, actual);
            sessionMock.Verify(s => s.AddOponent(reciever), Times.Once());
        }

        [Fact]
        public void AcceptInvite_ShouldFail_SenderInactive()
        {
            // Arrange
            User sender = new User();
            User reciever = new User();

            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            sessionMock.SetupGet(s => s.InvitedPlayersIDs)
                       .Returns(new List<string> { reciever.Id });

            sessionsManagerMock.Setup(sm => sm.FindSession(sender.Id))
                               .Returns(sessionMock.Object);

            sessionsManagerMock.Setup(sm => sm.FindSession(reciever.Id))
                               .Returns(() => null);

            sessionMock.SetupGet(s => s.Oponent)
                       .Returns(() => null);

            PlayersManager playersManager = 
                Arrange_AcceptInvite(
                    reciever,
                    battlefieldMock,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock);
    
            playersManager.AddAsActive(reciever);

            // Act
            IBattlefield? actual = playersManager.AcceptInvite(reciever, sender);

            // Assert
            Assert.Null(actual);
            sessionMock.Verify(s => s.AddOponent(reciever), Times.Never());
        }

        [Fact]
        public void AcceptInvite_ShouldFail_RecieverInactive()
        {
            // Arrange
            User sender = new User();
            User reciever = new User();

            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            sessionMock.SetupGet(s => s.InvitedPlayersIDs)
                       .Returns(new List<string> { reciever.Id });

            sessionsManagerMock.Setup(sm => sm.FindSession(sender.Id))
                               .Returns(sessionMock.Object);

            sessionsManagerMock.Setup(sm => sm.FindSession(reciever.Id))
                               .Returns(() => null);

            sessionMock.SetupGet(s => s.Oponent)
                       .Returns(() => null);

            PlayersManager playersManager =
                Arrange_AcceptInvite(
                    reciever,
                    battlefieldMock,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock);

            playersManager.AddAsActive(sender);

            // Act
            IBattlefield? battlefield = playersManager.AcceptInvite(reciever, sender);

            // Assert
            Assert.Null(battlefield);
            sessionMock.Verify(s => s.AddOponent(reciever), Times.Never());
        }

        [Fact]
        public void AcceptInvite_ShouldFail_SessionDoesNotExist()
        {
            // Arrange 
            User sender = new User();
            User reciever = new User();

            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            sessionMock.SetupGet(s => s.InvitedPlayersIDs)
                       .Returns(new List<string> { reciever.Id });

            sessionsManagerMock.Setup(sm => sm.FindSession(sender.Id))
                               .Returns(() => null);

            sessionsManagerMock.Setup(sm => sm.FindSession(reciever.Id))
                               .Returns(() => null);

            sessionMock.SetupGet(s => s.Oponent)
                       .Returns(() => null);

            PlayersManager playersManager =
                Arrange_AcceptInvite(
                    reciever,
                    battlefieldMock,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock);

            playersManager.AddAsActive(sender);
            playersManager.AddAsActive(reciever);

            // Act
            IBattlefield? battlefield = playersManager.AcceptInvite(reciever, sender);

            // Assert
            Assert.Null(battlefield);
            sessionMock.Verify(s => s.AddOponent(reciever), Times.Never());
        }

        [Fact]
        public void AcceptInvite_ShouldFail_RecieverInvolved()
        {
            // Arrange
            User sender = new User();
            User reciever = new User();

            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            var recieverSessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            sessionMock.SetupGet(s => s.InvitedPlayersIDs)
                       .Returns(new List<string> { reciever.Id });

            sessionsManagerMock.Setup(sm => sm.FindSession(sender.Id))
                               .Returns(sessionMock.Object);

            sessionsManagerMock.Setup(sm => sm.FindSession(reciever.Id))
                               .Returns(recieverSessionMock.Object);

            sessionMock.SetupGet(s => s.Oponent)
                       .Returns(() => null);

            PlayersManager playersManager =
                Arrange_AcceptInvite(
                    reciever,
                    battlefieldMock,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock);

            playersManager.AddAsActive(sender);
            playersManager.AddAsActive(reciever);

            // Act
            IBattlefield? battlefield = playersManager.AcceptInvite(reciever, sender);

            // Assert
            Assert.Null(battlefield);
            sessionMock.Verify(s => s.AddOponent(reciever), Times.Never());
        }

        [Fact]
        public void AcceptInvite_ShouldFail_AlreadyHasOponent()
        {
            // Arrange
            User sender = new User();
            User reciever = new User();
            User differentOponent = new User();

            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            sessionMock.SetupGet(s => s.InvitedPlayersIDs)
                       .Returns(new List<string> { reciever.Id });

            sessionsManagerMock.Setup(sm => sm.FindSession(sender.Id))
                               .Returns(sessionMock.Object);

            sessionsManagerMock.Setup(sm => sm.FindSession(reciever.Id))
                               .Returns(() => null);

            sessionMock.SetupGet(s => s.Oponent)
                       .Returns(() => differentOponent);

            PlayersManager playersManager =
                Arrange_AcceptInvite(
                    reciever,
                    battlefieldMock,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock);

            playersManager.AddAsActive(sender);
            playersManager.AddAsActive(reciever);

            // Act
            IBattlefield? battlefield = playersManager.AcceptInvite(reciever, sender);

            // Assert
            Assert.Null(battlefield);
            sessionMock.Verify(s => s.AddOponent(reciever), Times.Never());
        }

        [Fact]
        public void AcceptInvite_ShouldFail_PlayerUninvited()
        {
            // Arrange
            User reciever = new User();
            User sender = new User();

            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);
            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);
            var sessionsManagerMock = new Mock<ISessionsManager>();
            var interfaceMock = new Mock<IGameInterface>();

            sessionMock.SetupGet(s => s.InvitedPlayersIDs)
                       .Returns(new List<string>());

            sessionMock.SetupGet(s => s.Oponent)
                       .Returns(() => null);
        
            sessionsManagerMock.Setup(sm => sm.FindSession(reciever.Id))
                               .Returns(() => null);

            sessionsManagerMock.Setup(sm => sm.FindSession(sender.Id))
                               .Returns(sessionMock.Object);

            PlayersManager playersManager =
                Arrange_AcceptInvite(
                    reciever,
                    battlefieldMock,
                    sessionMock,
                    sessionsManagerMock,
                    interfaceMock);

            playersManager.AddAsActive(sender);
            playersManager.AddAsActive(reciever);

            // Act
            IBattlefield? actual = playersManager.AcceptInvite(reciever, sender);

            // Assert
            Assert.Null(actual);
            sessionMock.Verify(s => s.AddOponent(reciever), Times.Never());
        }

        [Fact]
        public void ConfirmReady_ShouldSucceed()
        {
            // Arrange
            User user = new User();

            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);

            battlefieldMock.SetupProperty(b => b.IsReady);

            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            sessionMock.Setup(s => s.GetBattlefield(user.Id))
                       .Returns(battlefieldMock.Object);

            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);

            sessionsManagerMock.Setup(sm => sm.FindSession(user.Id))
                               .Returns(sessionMock.Object);

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            PlayersManager playersManager = 
                new PlayersManager(gameInterfaceMock.Object, sessionsManagerMock.Object);

            // Act
            bool actual = playersManager.ConfirmReady(user);

            // Assert
            Assert.True(actual);
            battlefieldMock.VerifySet(b => b.IsReady = true, Times.Once());
        }

        [Fact]
        public void ConfirmReady_ShouldFail()
        {
            // Arrange
            User user = new User();

            var battlefieldMock = new Mock<IBattlefield>(MockBehavior.Strict);

            battlefieldMock.SetupGet(b => b.IsReady)
                           .Returns(false);

            battlefieldMock.SetupSet(b => b.IsReady = true);

            var sessionMock = new Mock<IGameSession>(MockBehavior.Strict);

            sessionMock.Setup(s => s.GetBattlefield(user.Id))
                       .Returns(battlefieldMock.Object);

            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);

            sessionsManagerMock.Setup(sm => sm.FindSession(user.Id))
                               .Returns(sessionMock.Object);

            var gameInterfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);

            PlayersManager playersManager =
                new PlayersManager(gameInterfaceMock.Object, sessionsManagerMock.Object);

            // Act
            bool actual = playersManager.ConfirmReady(user);

            // Assert
            Assert.False(actual);
            battlefieldMock.VerifyGet(b => b.IsReady, Times.Once());
        }

        #region private helpers

        private PlayersManager Arange_InvitePlayer(
            User sender,
            Mock<IGameSession> sessionMock,
            Mock<ISessionsManager> sessionsManagerMock,
            Mock<IGameInterface> interfaceMock,
            List<string> invitedPlayers)
        {
            sessionMock.SetupGet(s => s.InvitedPlayersIDs)
                       .Returns(invitedPlayers);

            sessionsManagerMock.Setup(sm => sm.FindSession(sender.Id))
                               .Returns(sessionMock.Object);

            return new PlayersManager(interfaceMock.Object, sessionsManagerMock.Object);
        }        

        private PlayersManager Arrange_AcceptInvite(
            User reciever,
            Mock<IBattlefield> battlefieldMock,
            Mock<IGameSession> sessionMock, 
            Mock<ISessionsManager> sessionsManagerMock,
            Mock<IGameInterface> interfaceMock)
        {
            // Without playersManager.AddAsActive(reciever); x
            // Without playersManager.AddAsActive(sender); x
            // Without sessionsManagerMock.Setup(sm => sm.FindSession(sender.Id)).Returns(sessionMock.Object); x
            // Without sessionsManagerMock.Setup(sm => sm.FindSession(reciever.Id)).Returns(() => null); x
            // Without sessionMock.SetupGet(s => s.Oponent).Returns(() => null); x

            sessionMock.SetupGet(s => s.Battlefields)
                       .Returns(new[]
                       {
                           null,
                           battlefieldMock.Object
                       });

            sessionMock.Setup(s => s.AddOponent(reciever));

            PlayersManager playersManager =
                new PlayersManager(interfaceMock.Object, sessionsManagerMock.Object);

            return playersManager;
        }

        private PlayersManager ArrangePlayersManager()
            => ArrangePlayersManager(new Mock<IGameInterface>(MockBehavior.Strict));

        private PlayersManager ArrangePlayersManager(Mock<IGameInterface> gameInterfaceMock)
        {
            var sessionsManagerMock = new Mock<ISessionsManager>(MockBehavior.Strict);
           
            return new PlayersManager(gameInterfaceMock.Object, sessionsManagerMock.Object);
        }

        private Mock<IGameInterface> SetupGameInterface(User reciever, User sender)
        {
            var interfaceMock = new Mock<IGameInterface>(MockBehavior.Strict);
            interfaceMock.Setup(gI => gI.SendInvite(reciever.Id, sender))
                         .Returns(Task.CompletedTask);

            return interfaceMock;
        }

        #endregion
    }
}
