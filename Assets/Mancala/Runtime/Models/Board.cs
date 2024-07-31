using System;

namespace tana_gh.Mancala
{
    [Serializable]
    public class Board : IEntity
    {
        public readonly Guid id;
        public readonly Pocket[] pockets;

        public Guid Id => id;

        public Board(Guid id, Pocket[] pockets)
        {
            this.id = id;
            this.pockets = pockets;
        }
    }
}
