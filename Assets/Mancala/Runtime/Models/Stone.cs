using System;

namespace tana_gh.Mancala
{
    [Serializable]
    public class Stone : IEntity
    {
        public readonly Guid id;

        public Guid Id => id;

        public Stone(Guid id)
        {
            this.id = id;
        }
    }
}
