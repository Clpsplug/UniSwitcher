using Cysharp.Threading.Tasks;
using UniSwitcher;
#if UGS_ANALYTICS
using Unity.Services.Core;
using Unity.Services.Analytics;
#endif
using UnityEngine;

public class Sample : Switcher
{
    private async void Awake()
    {
        // This part is initially disabled to prevent accidental use of your Unity Gaming Service Analytics event quota.
        // Include com.unity.services.analytics into this sample project to enable this part of the code.
#if UGS_ANALYTICS
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
        }
#endif
    }

    private void Start()
    {
        PerformSceneTransition(
            ChangeScene(Scene.TestScene, new SampleData(42))
                .WithTransitionEffect()
        ).Forget(Debug.LogException);
    }
}