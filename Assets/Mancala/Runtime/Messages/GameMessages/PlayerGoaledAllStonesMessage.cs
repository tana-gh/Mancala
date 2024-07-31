using System;

namespace tana_gh.Mancala
{
    [Serializable]
    [Role("GameMessage", SceneKind.Game)]
    public class PlayerGoaledAllStonesMessage : IGameMessage
    {
        public readonly Player player;

        public PlayerGoaledAllStonesMessage(Player player)
        {
            this.player = player;
        }
    }
}
