using System;

namespace tana_gh.Mancala
{
    [Serializable]
    [Role("GameMessage", SceneKind.Game)]
    public class PlayerJustGoaledMessage : IGameMessage
    {
        public readonly Player player;
        public readonly Pocket goal;
        public readonly Stone stone;

        public PlayerJustGoaledMessage(Player player, Pocket goal, Stone stone)
        {
            this.player = player;
            this.goal = goal;
            this.stone = stone;
        }
    }
}
