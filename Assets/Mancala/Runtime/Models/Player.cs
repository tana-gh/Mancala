using System;

namespace tana_gh.Mancala
{
    [Serializable]
    public class Player : IEntity
    {
        public readonly Guid id;

        public Guid Id => id;

        public Player(Guid id)
        {
            this.id = id;
        }
    }
}
