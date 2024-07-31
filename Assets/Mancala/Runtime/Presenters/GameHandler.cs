using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace tana_gh.Mancala
{
    [Role("Handler", SceneKind.Game)]
    public class GameHandler : IDisposable
    {
        [Inject] private EntityStore<Player> PlayerStore { get; set; }
        [Inject] private EntityStore<Pocket> PocketStore { get; set; }
        [Inject] private IPublisher<PocketSelectedMessage> PocketSelectedPub { get; set; }
        [Inject] private IPublisher<StoneSelectedMessage> StoneSelectedPub { get; set; }
        [Inject] private ISubscriber<NewGameStartedMessage> NewGameStartedSub { get; set; }
        [Inject] private ISubscriber<WaitingPlayerTurnMessage> WaitingPlayerTurnSub { get; set; }
        [Inject] private ISubscriber<PlayerDroppingStoneIntoPocketMessage> PlayerDroppingStoneIntoPocketSub { get; set; }
        [Inject] private ISubscriber<PlayerDroppedStoneIntoPocketMessage> PlayerDroppedStoneIntoPocketSub { get; set; }
        [Inject] private ISubscriber<PlayerJustGoaledMessage> PlayerJustGoaledSub { get; set; }
        [Inject] private ISubscriber<PlayerStolePocketMessage> PlayerStolePocketSub { get; set; }
        [Inject] private ISubscriber<PlayerGoaledAllStonesMessage> PlayerGoaledAllStonesSub { get; set; }
        [Inject] private ISubscriber<PlayerGoaledMoreThanHalfStonesMessage> PlayerGoaledMoreThanHalfStonesSub { get; set; }
        [Inject] private ISubscriber<PlayerWonMessage> PlayerWonSub { get; set; }
        [Inject] private ISubscriber<DrewMessage> DrewSub { get; set; }

        private DisposableBagBuilder Disposables { get; } = DisposableBag.CreateBuilder();
        private bool Disposed { get; set; } = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Disposables.Build().Dispose();
                }
                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Init()
        {
            NewGameStartedSub.Subscribe(OnNewGameStarted).AddTo(Disposables);
            WaitingPlayerTurnSub.Subscribe(OnWaitingPlayerTurn).AddTo(Disposables);
            PlayerDroppingStoneIntoPocketSub.Subscribe(OnPlayerDroppingStoneIntoPocket).AddTo(Disposables);
            PlayerDroppedStoneIntoPocketSub.Subscribe(OnPlayerDroppedStoneIntoPocket).AddTo(Disposables);
            PlayerJustGoaledSub.Subscribe(OnPlayerJustGoaled).AddTo(Disposables);
            PlayerStolePocketSub.Subscribe(OnPlayerStolePocket).AddTo(Disposables);
            PlayerGoaledAllStonesSub.Subscribe(OnPlayerGoaledAllStones).AddTo(Disposables);
            PlayerGoaledMoreThanHalfStonesSub.Subscribe(OnPlayerGoaledMoreThanHalfStones).AddTo(Disposables);
            PlayerWonSub.Subscribe(OnPlayerWon).AddTo(Disposables);
            DrewSub.Subscribe(OnDrew).AddTo(Disposables);
        }

        private void OnNewGameStarted(NewGameStartedMessage msg)
        {
            Debug.Log("New game started.");
        }

        private void OnWaitingPlayerTurn(WaitingPlayerTurnMessage msg)
        {
            Debug.Log($"Waiting for Player{PlayerStore.IndexOf(msg.player)}'s turn.");
            foreach (var (pocket, i) in PocketStore.GetAll().Select((pocket, i) => (pocket, i)))
            {
                Debug.Log($"Pocket{i} has {pocket.stones.Count} stones.");
            }

            PocketSelectedPub.Publish(new PocketSelectedMessage(msg.player, GetSelectedPocket(msg.player)));
        }

        private void OnPlayerDroppingStoneIntoPocket(PlayerDroppingStoneIntoPocketMessage msg)
        {
            Debug.Log($"Player{PlayerStore.IndexOf(msg.player)} dropping stone into Pocket{PocketStore.IndexOf(msg.toPocket)}.");

            StoneSelectedPub.Publish(new StoneSelectedMessage(msg.player, msg.fromPocket, msg.toPocket, msg.fromPocket.stones.First()));
        }

        private void OnPlayerDroppedStoneIntoPocket(PlayerDroppedStoneIntoPocketMessage msg)
        {
            Debug.Log($"Player{PlayerStore.IndexOf(msg.player)} dropped stone into Pocket{PocketStore.IndexOf(msg.toPocket)}.");
        }

        private void OnPlayerJustGoaled(PlayerJustGoaledMessage msg)
        {
            Debug.Log($"Player{PlayerStore.IndexOf(msg.player)} just goaled into Goal{PocketStore.IndexOf(msg.goal)}.");
        }

        private void OnPlayerStolePocket(PlayerStolePocketMessage msg)
        {
            Debug.Log($"Player{PlayerStore.IndexOf(msg.player)} stole Pocket{PocketStore.IndexOf(msg.fromPocket)}.");
        }

        private void OnPlayerGoaledAllStones(PlayerGoaledAllStonesMessage msg)
        {
            Debug.Log($"Player{PlayerStore.IndexOf(msg.player)} goaled all stones.");
        }

        private void OnPlayerGoaledMoreThanHalfStones(PlayerGoaledMoreThanHalfStonesMessage msg)
        {
            Debug.Log($"Player{PlayerStore.IndexOf(msg.player)} goaled more than half stones.");
        }

        private void OnPlayerWon(PlayerWonMessage msg)
        {
            Debug.Log($"Player{PlayerStore.IndexOf(msg.player)} won - {msg.winnerPoints} points.");
        }

        private void OnDrew(DrewMessage msg)
        {
            Debug.Log("Drew.");
        }

        private Pocket GetSelectedPocket(Player player)
        {
            var index = PlayerStore.IndexOf(player);
            var pocketCount = 7;
            return
                PocketStore.GetAll()
                .Skip(pocketCount * index)
                .Take(pocketCount - 1)
                .First(pocket => pocket.stones.Count >= 1);
        }
    }
}
