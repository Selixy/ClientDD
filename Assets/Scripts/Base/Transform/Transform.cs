using System;
using System.Numerics;

public class Transform3D
{
    public Vector3    Position { get; set; } = Vector3.Zero;
    public Quaternion Rotation { get; set; } = Quaternion.Identity;
    public Vector3    Scale    { get; set; } = Vector3.One;

    public Transform3D() { }

    public Transform3D(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale    = scale;
    }

    public void Translate(Vector3 delta)
    {
        Position += delta;
    }

    public void Rotate(Quaternion deltaRotation)
    {
        Rotation = Quaternion.Normalize(deltaRotation * Rotation);
    }

    public void ScaleBy(Vector3 scaleFactor)
    {
        Scale *= scaleFactor;
    }

    public override string ToString()
    {
        return $"Pos: {Position}, Rot: {Rotation}, Scale: {Scale}";
    }
}
