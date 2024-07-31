using System;

namespace tana_gh.Mancala
{
    [Serializable]
    [Role("GameMessage", SceneKind.Game)]
    public class PlayerDroppedStoneIntoPocketMessage : IGameMessage
    {
        public readonly Player player;
        public readonly Pocket fromPocket;
        public readonly Pocket toPocket;
        public readonly Stone stone;

        public PlayerDroppedStoneIntoPocketMessage(Player player, Pocket fromPocket, Pocket toPocket, Stone stone)
        {
            this.player = player;
            this.fromPocket = fromPocket;
            this.toPocket = toPocket;
            this.stone = stone;
        }
    }
}
