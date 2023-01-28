using Cysharp.Threading.Tasks;

namespace UniSwitcher.Tests.Runtime.TestStubs
{
    public class UnitTestScene1Controller : Switcher
    {
        public void PerformTest(int dataToPass)
        {
            PerformSceneTransition(
                ChangeScene(
                    new CrudeScene("Packages/com.clpsplug.uniswitcher/TestStubs/UnitTestScene2"),
                    new CrudeData(dataToPass)
                )
            ).Forget();
        }
    }
}