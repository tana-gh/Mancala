using System;
using System.Collections.Generic;

namespace tana_gh.Mancala
{
    [Serializable]
    public class Game : IEntity
    {
        public readonly Guid id;
        public readonly Player[] players;
        public readonly Board board;
        public List<IGameMessage> messageQueue;

        public Guid Id => id;

        public Game(Guid id, Player[] players, Board board, List<IGameMessage> messageQueue)
        {
            this.id = id;
            this.players = players;
            this.board = board;
            this.messageQueue = messageQueue;
        }
    }
}
