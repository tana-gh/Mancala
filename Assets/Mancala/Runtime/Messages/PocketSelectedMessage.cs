
namespace tana_gh.Mancala
{
    [Role("Message", SceneKind.Game)]
    public class PocketSelectedMessage
    {
        public Player Player { get; }
        public Pocket Pocket { get; }

        public PocketSelectedMessage(Player player, Pocket pocket)
        {
            Player = player;
            Pocket = pocket;
        }
    }
}
