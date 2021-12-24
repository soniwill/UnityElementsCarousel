using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ContainerTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void ContainerShouldContainElement(Bounds elementBounds, Container container)//Todo this is not the correct container class. It still need to be created. the current one is form system.component model name space.
    {
        // Use the Assert class to test conditions
    }
    
    [Test]
    public void ContainerShouldNotContainElement(Bounds elementBounds, Container container)//Todo this is not the correct container class. It still need to be created. the current one is form system.component model name space.
    {
        // Use the Assert class to test conditions
    }

    [Test]
    public void ContainerShouldContainExpecetedNumberOfCarousels(int expectedCarouselNumber, Container container) //Todo this is not the correct container class. It still need to be created. the current one is form system.component model name space.
    {
        // container should be passed need to check if how many carousel it has.
    }

    
}
