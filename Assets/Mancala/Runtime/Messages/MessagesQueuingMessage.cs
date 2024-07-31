using System.Collections.Generic;
using System.Linq;

namespace tana_gh.Mancala
{
    [Role("Message", SceneKind.Game)]
    public class MessagesQueuingMessage
    {
        public readonly IGameMessage[] gameMessages;

        public MessagesQueuingMessage(IEnumerable<IGameMessage> gameMessages)
        {
            this.gameMessages = gameMessages.ToArray();
        }
    }
}
