using UniSwitcher.Domain;

public class Scene : BaseScene
#if UGS_ANALYTICS
    , IReportable
#endif
{
    public static Scene FirstScene => new Scene("Assets/Scenes/SampleScene.unity");
    public static Scene SecondScene => new Scene("Assets/Scenes/SecondScene.unity");

    // This part is initially disabled to prevent accidental use of your Unity Gaming Service Analytics event quota.
    // Include com.unity.services.analytics into this sample project to enable this part of the code.
#if UGS_ANALYTICS
    public override string ScreenVisitEventName => "screenVisit";
    public override string ScreenVisitEventPropertyName => "screenName";
    public bool DoNotReport()
    {
        return false;
    }
#endif
    // The part above is disabled until you manually include com.unity.services.analytics into this sample project.

    private Scene(string rawValue) : base(rawValue)
    { }
}