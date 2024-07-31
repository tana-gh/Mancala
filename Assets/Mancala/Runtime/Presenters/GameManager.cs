using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MessagePipe;
using VContainer;

namespace tana_gh.Mancala
{
    [Role("Handler", SceneKind.Game)]
    public partial class GameManager : IDisposable
    {
        private const int PlayerCount = 2;
        private const int PocketCount = 7;

        [Inject] private EntityStore<Game> GameStore { get; set; }
        [Inject] private EntityStore<Player> PlayerStore { get; set; }
        [Inject] private EntityStore<Board> BoardStore { get; set; }
        [Inject] private EntityStore<Pocket> PocketStore { get; set; }
        [Inject] private EntityStore<Stone> StoneStore { get; set; }
        [Inject] private IPublisher<MessagesQueuingMessage> MessagesQueuingPub { get; set; }
        [Inject] private ISubscriber<MessagesQueuingMessage> MessagesQueuingSub { get; set; }
        [Inject] private ISubscriber<NewGameStartingMessage> NewGameStartingSub { get; set; }
        [Inject] private ISubscriber<PocketSelectedMessage> PocketSelectedSub { get; set; }
        [Inject] private ISubscriber<StoneSelectedMessage> StoneSelectedSub { get; set; }

        private Game CurrentGame { get; set; }

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
            MessagesQueuingSub.Subscribe(OnMessagesQueuing).AddTo(Disposables);
            NewGameStartingSub.Subscribe(OnNewGameStarting).AddTo(Disposables);
            PocketSelectedSub.Subscribe(OnPocketSelected).AddTo(Disposables);
            StoneSelectedSub.Subscribe(OnStoneSelected).AddTo(Disposables);
        }

        private void OnMessagesQueuing(MessagesQueuingMessage msg)
        {
            CurrentGame.messageQueue.AddRange(msg.gameMessages);
        }

        private void OnNewGameStarting(NewGameStartingMessage msg)
        {
            Debug.Log("New game starting.");

            if (msg.InitialStoneCount != 4)
            {
                Debug.LogWarning("Initial stone count is invalid.");
                return;
            }

            GameStore.Clear();
            PlayerStore.Clear();
            BoardStore.Clear();
            PocketStore.Clear();
            StoneStore.Clear();

            var stoneLists =
                Enumerable.Range(0, PocketCount * PlayerCount)
                .Select
                (i =>
                    i % PocketCount == PocketCount - 1
                    ? new List<Stone>()
                    : Enumerable.Range(0, msg.InitialStoneCount)
                      .Select(_ => new Stone(Guid.NewGuid()))
                      .ToList()
                )
                .ToArray();
            StoneStore.AddRange(stoneLists.SelectMany(stone => stone));

            var pockets =
                Enumerable.Range(0, PocketCount * PlayerCount)
                .Select(i => new Pocket(Guid.NewGuid(), i % PocketCount == PocketCount - 1, stoneLists[i]))
                .ToArray();
            PocketStore.AddRange(pockets);

            var board = new Board(Guid.NewGuid(), pockets);
            BoardStore.Add(board);

            var players =
                Enumerable.Range(0, PlayerCount)
                .Select(_ => new Player(Guid.NewGuid()))
                .ToArray();
            PlayerStore.AddRange(players);

            var game = new Game(Guid.NewGuid(), players, board, new List<IGameMessage>());
            GameStore.Add(game);

            CurrentGame = game;

            MessagesQueuingPub.Publish
            (
                new MessagesQueuingMessage
                (
                    new IGameMessage[]
                    {
                        new NewGameStartedMessage(CurrentGame),
                        new WaitingPlayerTurnMessage(players[0])
                    }
                )
            );
        }

        private void OnPocketSelected(PocketSelectedMessage msg)
        {
            Debug.Log($"Pocket{PocketStore.IndexOf(msg.Pocket)} selected.");

            if (GetPlayerForPocket(msg.Pocket).id != msg.Player.id)
            {
                Debug.LogWarning("Selected pocket is not owned by the current player.");
                return;
            }

            if (msg.Pocket.stones.Count == 0)
            {
                Debug.LogWarning("Selected pocket is empty.");
                return;
            }

            var toPocket = msg.Pocket;
            var stoneCount = msg.Pocket.stones.Count;
            var gameMessages = new List<IGameMessage>();

            for (int i = 0; i < stoneCount; i++)
            {
                toPocket = GetNextPocket(msg.Player, toPocket);
                gameMessages.Add(new PlayerDroppingStoneIntoPocketMessage(msg.Player, msg.Pocket, toPocket));
            }

            MessagesQueuingPub.Publish(new MessagesQueuingMessage(gameMessages));
        }

        private void OnStoneSelected(StoneSelectedMessage msg)
        {
            Debug.Log($"Stone{StoneStore.IndexOf(msg.Stone)} selected.");

            if (!msg.FromPocket.stones.Contains(msg.Stone))
            {
                Debug.LogWarning("Selected stone is not in the selected pocket.");
                return;
            }

            msg.FromPocket.stones.Remove(msg.Stone);
            msg.ToPocket.stones.Add(msg.Stone);

            var gameMessages = new List<IGameMessage>();

            if (msg.FromPocket.stones.Count == 0)
            {
                Player nextPlayer;

                if (msg.ToPocket.isGoal)
                {
                    gameMessages.Add(new PlayerJustGoaledMessage(msg.Player, msg.ToPocket, msg.Stone));
                    nextPlayer = msg.Player;
                }
                else
                {
                    var oppositePocket = GetOppositePocket(msg.ToPocket);

                    if (msg.ToPocket.stones.Count == 1 && oppositePocket.stones.Count >= 1)
                    {
                        var goal = GetGoal(msg.Player);
                        goal.stones.AddRange(msg.ToPocket.stones);
                        goal.stones.AddRange(oppositePocket.stones);
                        msg.ToPocket.stones.Clear();
                        oppositePocket.stones.Clear();

                        gameMessages.Add(new PlayerStolePocketMessage(msg.Player, oppositePocket, msg.ToPocket));
                    }

                    nextPlayer = PlayerStore.GetNext(msg.Player);
                }

                var anotherPlayer = PlayerStore.GetNext(msg.Player);

                if (AllPocketsEmpty(msg.Player) || AllPocketsEmpty(anotherPlayer))
                {
                    GoalAllStones(msg.Player);
                    GoalAllStones(anotherPlayer);

                    var (winner, winnerPoints, loserPoints) = GetWinnerAndPoints(CurrentGame);

                    if (winner == null)
                    {
                        gameMessages.Add(new DrewMessage());
                    }
                    else
                    {
                        gameMessages.Add(new PlayerWonMessage(winner, winnerPoints, loserPoints));
                    }
                }
                else
                {
                    var (winner, winnerPoints, loserPoints) = GetWinnerAndPoints(CurrentGame);

                    if (winner == null)
                    {
                        gameMessages.Add(new WaitingPlayerTurnMessage(nextPlayer));
                    }
                    else
                    {
                        gameMessages.Add(new PlayerWonMessage(winner, winnerPoints, loserPoints));
                    }
                }
            }
            else
            {
                gameMessages.Add(new PlayerDroppedStoneIntoPocketMessage(msg.Player, msg.FromPocket, msg.ToPocket, msg.Stone));
            }

            MessagesQueuingPub.Publish(new MessagesQueuingMessage(gameMessages));
        }

        private Player GetPlayerForPocket(Pocket pocket)
        {
            var index = PocketStore.IndexOf(pocket);
            return PlayerStore.ElementAt(index / PocketCount);
        }

        private Pocket GetGoal(Player player)
        {
            var index = PlayerStore.IndexOf(player);
            return PocketStore.ElementAt(PocketCount * (index + 1) - 1);
        }

        private IEnumerable<Pocket> GetPlayerPockets(Player player)
        {
            var index = PlayerStore.IndexOf(player);
            return
                PocketStore.GetAll()
                .Skip(PocketCount * index)
                .Take(PocketCount - 1);
        }

        private Pocket GetNextPocket(Player player, Pocket pocket)
        {
            var nextPocket = PocketStore.GetNext(pocket);
            if (nextPocket.isGoal && GetPlayerForPocket(nextPocket).id != player.id)
            {
                return PocketStore.GetNext(nextPocket);
            }
            else {
                return nextPocket;
            }
        }

        private Pocket GetOppositePocket(Pocket pocket)
        {
            var index = PocketStore.IndexOf(pocket);
            var oppositeIndex = PocketCount * PlayerCount - index % PocketCount - index / PocketCount * PocketCount - 2;
            return PocketStore.ElementAt(oppositeIndex);
        }

        private bool AllPocketsEmpty(Player player)
        {
            return
                PocketStore.GetAll()
                .Where(pocket => GetPlayerForPocket(pocket).id == player.id && !pocket.isGoal)
                .All(pocket => pocket.stones.Count == 0);
        }

        private void GoalAllStones(Player player)
        {
            var goal = GetGoal(player);
            foreach (var pocket in GetPlayerPockets(player))
            {
                goal.stones.AddRange(pocket.stones);
                pocket.stones.Clear();
            }
        }

        private (Player player, int winnerPoints, int loserPoints) GetWinnerAndPoints(Game game)
        {
            var player0Goal = PocketStore.ElementAt(PocketCount - 1);
            var player1Goal = PocketStore.ElementAt(PocketCount * PlayerCount - 1);
            var player0Points = player0Goal.stones.Count;
            var player1Points = player1Goal.stones.Count;
            if (player0Points > StoneStore.Count / 2)
            {
                return (PlayerStore.ElementAt(0), player0Points, player1Points);
            }
            else if (player1Points > StoneStore.Count / 2)
            {
                return (PlayerStore.ElementAt(1), player1Points, player0Points);
            }
            else
            {
                return (null, 0, 0);
            }
        }
    }
}
