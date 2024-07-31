using System;

namespace tana_gh.Mancala
{
    [Serializable]
    [Role("GameMessage", SceneKind.Game)]
    public class WaitingPlayerTurnMessage : IGameMessage
    {
        public readonly Player player;

        public WaitingPlayerTurnMessage(Player player)
        {
            this.player = player;
        }
    }
}
