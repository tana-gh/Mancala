
namespace tana_gh.Mancala
{
    [Role("Message", SceneKind.Game)]
    public class NewGameStartingMessage
    {
        public int InitialStoneCount { get; }

        public NewGameStartingMessage(int initialStoneCount)
        {
            InitialStoneCount = initialStoneCount;
        }
    }
}
