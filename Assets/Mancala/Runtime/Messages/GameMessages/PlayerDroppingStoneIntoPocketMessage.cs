using System;

namespace tana_gh.Mancala
{
    [Serializable]
    [Role("GameMessage", SceneKind.Game)]
    public class PlayerDroppingStoneIntoPocketMessage : IGameMessage
    {
        public readonly Player player;
        public readonly Pocket fromPocket;
        public readonly Pocket toPocket;

        public PlayerDroppingStoneIntoPocketMessage(Player player, Pocket fromPocket, Pocket toPocket)
        {
            this.player = player;
            this.fromPocket = fromPocket;
            this.toPocket = toPocket;
        }
    }
}
