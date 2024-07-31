using System;

namespace tana_gh.Mancala
{
    [Serializable]
    [Role("GameMessage", SceneKind.Game)]
    public class PlayerWonMessage : IGameMessage
    {
        public readonly Player player;
        public readonly int winnerPoints;
        public readonly int loserPoints;

        public PlayerWonMessage(Player player, int winnerPoints, int loserPoints)
        {
            this.player = player;
            this.winnerPoints = winnerPoints;
            this.loserPoints = loserPoints;
        }
    }
}
