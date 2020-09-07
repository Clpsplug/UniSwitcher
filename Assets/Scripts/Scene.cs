using UniSwitcher.Domain;

public class Scene : IScene
{
    public static Scene FirstScene => new Scene("Assets/Scenes/SampleScene.unity");

    public static Scene SecondScene => new Scene("Assets/Scenes/SecondScene.unity");
    
    private readonly string _rawValue;

    public Scene(string rawValue)
    {
        _rawValue = rawValue;
    }
    
    public string GetRawValue()
    {
        return _rawValue;
    }
}
