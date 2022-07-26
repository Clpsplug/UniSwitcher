using UniSwitcher.Domain;

public class Scene : BaseScene
{
    public static Scene FirstScene => new Scene("Assets/Scenes/SampleScene.unity");
    public static Scene SecondScene => new Scene("Assets/Scenes/SecondScene.unity");

    private Scene(string rawValue) : base(rawValue)
    { }

    public static bool operator ==(Scene a, Scene b)
    {
        // null == null
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        // If either one is null but NOT both are non-null
        if (ReferenceEquals(a, null) != ReferenceEquals(b, null)) return false;
        // Both are non-null, then use RawValue
        return a.RawValue == b.RawValue;
    }

    public static bool operator !=(Scene a, Scene b)
    {
        return !(a == b);
    }

    public override bool Equals(object o)
    {
        if (o == null || GetType() != o.GetType())
        {
            return false;
        }

        return this == (Scene)o;
    }

    public override int GetHashCode()
    {
        return RawValue.GetHashCode();
    }

    public override string ToString()
    {
        return RawValue;
    }
}