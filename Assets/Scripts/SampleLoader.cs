using System;
using Cysharp.Threading.Tasks;
using UniSwitcher.Domain;
using UnityEngine;
using Zenject;

public class SampleLoader : MonoBehaviour, ISceneEntryPoint
{
    private bool _isHeld;

#pragma warning disable 649
    [InjectOptional] private SampleData _data;
#pragma warning restore 649

    public async UniTask Fire()
    {
        Debug.Log(_data.Answer);
        await UniTask.Delay(TimeSpan.FromSeconds(3));
    }

    public bool Validate()
    {
        return _data != null;
    }

    public void OnFailure(Exception e)
    {
        Debug.LogException(e);
    }
}
