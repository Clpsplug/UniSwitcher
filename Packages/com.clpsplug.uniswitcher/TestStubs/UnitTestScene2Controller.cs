using System;
using Cysharp.Threading.Tasks;
using UniSwitcher.Domain;
using UnityEngine;
using Zenject;

namespace UniSwitcher.Tests.Runtime.TestStubs
{
    public class UnitTestScene2Controller : MonoBehaviour, ISceneEntryPoint
    {
        private int _data = 0;
        [InjectOptional] private CrudeData _crudeData;

        public int GetData()
        {
            return _data;
        }
        
        public async UniTask Fire()
        {
            _data = _crudeData.Data;
            await UniTask.Yield();
        }

        public bool Validate()
        {
            return true;
        }

        public void OnFailure(Exception e)
        {
            Debug.LogException(e);
        }
    }
}