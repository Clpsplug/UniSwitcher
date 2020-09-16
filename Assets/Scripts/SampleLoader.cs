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

    public void Fire()
    {
        Debug.Log(_data.Answer);
        Hold().Forget(Debug.LogException);
    }

    private async UniTask Hold()
    {
        _isHeld = true;
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        _isHeld = false;
    }

    public bool Validate()
    {
        return _data != null;
    }

    public void OnFailure(Exception e)
    {
        Debug.LogException(e);
    }

    public bool IsHeld()
    {
        return _isHeld;
    }
}