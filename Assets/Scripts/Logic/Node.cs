using System.Numerics;

namespace Logic
{
    public class Node
    {
        public int Id { get; }
        public Vector2 Position { get; }
        public Node(int id, float x, float z)
        {
            Id = id;
            Position = new Vector2(x, z);
        }


        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case Node node when node.Id == Id:
                    return true;
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return Id + 31 * Position.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Id: {Id}], [Position: {Position}]";
        }
    }
}
