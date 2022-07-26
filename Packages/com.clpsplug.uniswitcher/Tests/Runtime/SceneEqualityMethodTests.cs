using NUnit.Framework;
using UniSwitcher.Domain;

namespace UniSwitcher.Tests.Runtime
{
    [Category("Value Tests")]
    public class SceneEqualityMethodTests
    {
        [Test]
        public void ShouldDetectEquality()
        {
            var a = new CrudeScene("path1.unity");
            var b = new CrudeScene("path1.unity");
            Assert.IsTrue(a == b);
			Assert.IsTrue(a.Equals(b));
		}
        [Test]
        public void ShouldDetectInequality() {
            var a = new CrudeScene("path1.unity");
            var b = new CrudeScene("path2.unity");
            Assert.IsFalse(a == b);
			Assert.IsFalse(a.Equals(b));
        }
    	[Test]
    	public void ShouldHandleNull()
    	{
        	CrudeScene n1 = null;
        	CrudeScene n2 = null;
        	var a = new CrudeScene("test.unity");
        	Assert.IsTrue(n1 == n2);
        	Assert.IsFalse(n1 == a);
        	Assert.IsFalse(a == n1);
			Assert.IsFalse(a.Equals(n1));
    	}
    }
}
