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
    // NOTE: To see this part in action, you first need to link this sample project to your Unity Gaming Service
    //       and set up Analytics for this project.
    //       After that, in the Event Manager, add a custom event type with this name that has a property with the name here.
    public override string ScreenVisitEventName => "screenVisit";
    public override string ScreenVisitEventParameterName => "screenName";
    public bool DoNotReport()
    {
        return false;
    }
#endif
    // The part above is disabled until you manually include com.unity.services.analytics into this sample project.

    private Scene(string rawValue) : base(rawValue)
    { }
}