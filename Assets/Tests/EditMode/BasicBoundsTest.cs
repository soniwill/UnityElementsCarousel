using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    public class BasicBoundsTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void BasicBoundsTestSimplePasses()
        {
            // Use the Assert class to test conditions
            var bounds = new Bounds(Vector2.zero, Vector2.one);
            Assert.True(bounds.Contains(Vector2.zero));
        }
    
    }
}
