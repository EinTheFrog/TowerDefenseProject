using System.Numerics;

public class Node
{
    public int Id { get; private set; }
    public Vector2 Position { get; private set; }
    public Node(int id, float x, float z)
    {
        Id = id;
        Position = new Vector2(x, z);
    }


    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj is Node && (obj as Node).Id == Id) return true;
        return false;
    }

    public override int GetHashCode()
    {
        return Id + 31 * Position.GetHashCode();
    }

    public override string ToString()
    {
        return $"[id: {Id}], [position: {Position}]";
    }
}
