using Cysharp.Threading.Tasks;
using UniSwitcher;
//using Unity.Services.Core;
//using Unity.Services.Analytics;
using UnityEngine;

public class Sample : Switcher
{
    private async void Awake()
    {
        /*
        try
        {
           Debug.Log("Init Unity Services");
           var options = new InitializationOptions();
           options.SetOption("com.unity.services.core.environment-name", "dev");
            await UnityServices.InitializeAsync(options);
            var consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
            Debug.Log("OK");
        }
        catch (ConsentCheckException e)
        {
            // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
            Debug.LogWarning($"Oops! Consent failed. {e.Reason}");
        }*/
    }
    
    
    private void Start()
    {
        PerformSceneTransition(
            ChangeScene(Scene.SecondScene, new SampleData(42))
                .WithTransitionEffect()
        ).Forget(Debug.LogException);
    }
}