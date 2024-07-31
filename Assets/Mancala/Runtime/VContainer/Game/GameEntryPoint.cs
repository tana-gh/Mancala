using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace tana_gh.Mancala
{
    public partial class GameEntryPoint : IAsyncStartable, ITickable
    {
        [Inject] private EntityStore<Game> GameStore { get; set; }
        [Inject] private IPublisher<NewGameStartingMessage> NewGameStartingPub { get; set; }
        [Inject] private IPublisher<NewGameStartedMessage> NewGameStartedPub { get; set; }
        [Inject] private IPublisher<WaitingPlayerTurnMessage> WaitingPlayerTurnPub { get; set; }
        [Inject] private IPublisher<PlayerDroppingStoneIntoPocketMessage> PlayerDroppingStoneIntoPocketPub { get; set; }
        [Inject] private IPublisher<PlayerDroppedStoneIntoPocketMessage> PlayerDroppedStoneIntoPocketPub { get; set; }
        [Inject] private IPublisher<PlayerJustGoaledMessage> PlayerJustGoaledPub { get; set; }
        [Inject] private IPublisher<PlayerStolePocketMessage> PlayerStolePocketPub { get; set; }
        [Inject] private IPublisher<PlayerGoaledAllStonesMessage> PlayerGoaledAllStonesPub { get; set; }
        [Inject] private IPublisher<PlayerGoaledMoreThanHalfStonesMessage> PlayerGoaledMoreThanHalfStonesPub { get; set; }
        [Inject] private IPublisher<PlayerWonMessage> PlayerWonPub { get; set; }
        [Inject] private IPublisher<DrewMessage> DrewPub { get; set; }
        
        partial void Init();

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            Init();

            await UniTask.Yield();

            Debug.Log("Game entry point started.");

            NewGameStartingPub.Publish(new NewGameStartingMessage(4));
        }

        public void Tick()
        {
            if (GameStore.Count == 0) return;

            var game = GameStore.GetSingle();

            if (game.messageQueue.Count == 0) return;

            var message = game.messageQueue[0];
            game.messageQueue.RemoveAt(0);

            switch (message)
            {
                case NewGameStartedMessage newGameStartedMessage:
                    NewGameStartedPub.Publish(newGameStartedMessage);
                    break;
                case WaitingPlayerTurnMessage waitingPlayerTurnMessage:
                    WaitingPlayerTurnPub.Publish(waitingPlayerTurnMessage);
                    break;
                case PlayerDroppingStoneIntoPocketMessage playerDroppingStoneIntoPocketMessage:
                    PlayerDroppingStoneIntoPocketPub.Publish(playerDroppingStoneIntoPocketMessage);
                    break;
                case PlayerDroppedStoneIntoPocketMessage playerDroppedStoneIntoPocketMessage:
                    PlayerDroppedStoneIntoPocketPub.Publish(playerDroppedStoneIntoPocketMessage);
                    break;
                case PlayerJustGoaledMessage playerJustGoaledMessage:
                    PlayerJustGoaledPub.Publish(playerJustGoaledMessage);
                    break;
                case PlayerStolePocketMessage playerStolePocketMessage:
                    PlayerStolePocketPub.Publish(playerStolePocketMessage);
                    break;
                case PlayerGoaledAllStonesMessage playerGoaledAllStonesMessage:
                    PlayerGoaledAllStonesPub.Publish(playerGoaledAllStonesMessage);
                    break;
                case PlayerGoaledMoreThanHalfStonesMessage playerGoaledMoreThanHalfStonesMessage:
                    PlayerGoaledMoreThanHalfStonesPub.Publish(playerGoaledMoreThanHalfStonesMessage);
                    break;
                case PlayerWonMessage playerWonMessage:
                    PlayerWonPub.Publish(playerWonMessage);
                    break;
                case DrewMessage drewMessage:
                    DrewPub.Publish(drewMessage);
                    break;
            }
        }
    }
}
