using System;
using System.Collections;
using UniSwitcher.Tests.Runtime.TestStubs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Zenject;
using Assert = UnityEngine.Assertions.Assert;
using Object = UnityEngine.Object;

namespace UniSwitcher.Tests.Runtime
{
    public class SceneSwitchValueTest: ZenjectIntegrationTestFixture
    {
        private bool sceneLoading;

        private const int Data = 42;
        
        [UnityTest]
        public IEnumerator CheckSceneChange()
        {
            SceneManager.LoadSceneAsync("Packages/com.clpsplug.uniswitcher/TestStubs/UnitTestScene1").completed +=
                _ =>
                {
                    sceneLoading = true;
                    Debug.Log("Scene OK");
                };
            
            yield return new WaitWhile(() => sceneLoading);
            
            PreInstall();
            PostInstall();
            
            yield return new WaitForEndOfFrame();

            var ctrl = GameObject.FindObjectOfType<UnitTestScene1Controller>();
            Assert.IsNotNull(ctrl);

            ctrl.PerformTest(Data);
            
            var frameCount = 0;
            while (frameCount <= 100)
            {
                // The next scene should have UniTestScene2Controller.
                if (GameObject.FindObjectOfType<UnitTestScene2Controller>() != null)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
                frameCount++;
            }

            if (frameCount > 100)
            {
                throw new TimeoutException("Next scene was not triggered in time.");
            }
            
            PreInstall();
            PostInstall();
            
            var ctrl2 = Object.FindObjectOfType<UnitTestScene2Controller>();
            Assert.IsNotNull(ctrl2);

            Assert.AreEqual(Data, ctrl2.GetData());

            yield return null;
        }
    }
}