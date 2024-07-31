using System;

namespace tana_gh.Mancala
{
    [Serializable]
    [Role("GameMessage", SceneKind.Game)]
    public class PlayerGoaledMoreThanHalfStonesMessage : IGameMessage
    {
        public readonly Player player;

        public PlayerGoaledMoreThanHalfStonesMessage(Player player)
        {
            this.player = player;
        }
    }
}
