using System;
using System.Collections.Generic;

namespace tana_gh.Mancala
{
    [Serializable]
    public class Pocket : IEntity
    {
        public readonly Guid id;
        public readonly bool isGoal;
        public readonly List<Stone> stones;

        public Guid Id => id;

        public Pocket(Guid id, bool isGoal, List<Stone> stones)
        {
            this.id = id;
            this.isGoal = isGoal;
            this.stones = stones;
        }
    }
}
