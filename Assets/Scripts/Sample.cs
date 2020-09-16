using Cysharp.Threading.Tasks;
using UniSwitcher;
using UnityEngine;

public class Sample : Switcher
{
    private void Start()
    {
        PerformSceneTransition(
            ChangeScene(Scene.SecondScene, new SampleData(42))
                .WithTransitionEffect()
        ).Forget(Debug.LogException);
    }
}