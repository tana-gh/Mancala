
namespace tana_gh.Mancala
{
    [Role("Message", SceneKind.Game)]
    public class StoneSelectedMessage
    {
        public Player Player { get; }
        public Pocket FromPocket { get; }
        public Pocket ToPocket { get; }
        public Stone Stone { get; }

        public StoneSelectedMessage(Player player, Pocket fromPocket, Pocket toPocket, Stone stone)
        {
            Player = player;
            FromPocket = fromPocket;
            ToPocket = toPocket;
            Stone = stone;
        }
    }
}
