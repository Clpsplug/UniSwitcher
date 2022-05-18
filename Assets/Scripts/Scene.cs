using UniSwitcher.Domain;

public class Scene : BaseScene
{
    public static Scene FirstScene => new Scene("Assets/Scenes/SampleScene.unity");
    public static Scene SecondScene => new Scene("Assets/Scenes/SecondScene.unity");

    private Scene(string rawValue) : base(rawValue)
    { }
}