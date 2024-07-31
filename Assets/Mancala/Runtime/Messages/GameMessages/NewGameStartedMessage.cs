using System;

namespace tana_gh.Mancala
{
    [Serializable]
    [Role("GameMessage", SceneKind.Game)]
    public class NewGameStartedMessage : IGameMessage
    {
        public readonly Game game;

        public NewGameStartedMessage(Game game)
        {
            this.game = game;
        }
    }
}
