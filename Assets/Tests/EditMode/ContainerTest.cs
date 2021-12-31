using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ContainerTest
{
    
   /*=====================================
   // Elements/Container Corner indexes and base case position with center at (0,0):
   // -Top-Left: 0 (-0.5, 0.5)
   // -Top-right: 1 (0.5, 0.5)
   // -Bottom-Left: 2 (-0.5, -0.5)
   // -Bottom-right: 3 (0.5, -0.5)
   //
   //     
   //    0*  *  *  *1
   //     *        *
   //     *        *
   //    2*  *  *  *3
   //
   //
   //
   //====================================*/
    private static readonly Vector2 _baseCaseCorner0 = new Vector2(-0.5f, 0.5f);
    private static readonly Vector2 _baseCaseCorner1 = new Vector2(0.5f, 0.5f);
    private static readonly Vector2 _baseCaseCorner2 = new Vector2(-0.5f, -0.5f);
    private static readonly Vector2 _baseCaseCorner3 = new Vector2(0.5f, -0.5f);
    public static IEnumerable<TestCaseData> ElementIsVisibleInsideContainerTestCases
    {
        get
        {
            
            var container = new BoxContainer(Vector2.zero, Vector2.one ); //base case
            
            var element = new Element(Vector2.zero, Vector2.one ); //base case
            yield return new TestCaseData(element, container).SetName("ElementHasTheSameContainerDimensions"); 
            
            var element1 = new Element(_baseCaseCorner3, Vector2.one ); //Element position at container _baseCaseCorner3
            yield return new TestCaseData(element1, container).SetName("OnlyElementCornerIndex0IsInside");
            
            var element2 = new Element(_baseCaseCorner2, Vector2.one ); //Element position at container _baseCaseCorner2
            yield return new TestCaseData(element2, container).SetName("OnlyElementCornerIndex1IsInside");
            
            var element3 = new Element(_baseCaseCorner1, Vector2.one ); //Element position at container _baseCaseCorner1
            yield return new TestCaseData(element3, container).SetName("OnlyElementCornerIndex2IsInside");
            
            var element4 = new Element(_baseCaseCorner0, Vector2.one ); //Element position at container _baseCaseCorner0
            yield return new TestCaseData(element4, container).SetName("OnlyElementCornerIndex3IsInside");
            
            var element5 = new Element(Vector2.zero, Vector2.one*2 ); 
            yield return new TestCaseData(element5, container).SetName("ElementIsBiggerThanContainerAndBothHasTheSamePosition");
            
            var element6 = new Element(_baseCaseCorner2, Vector2.zero ); //Element position at container _baseCaseCorner2
            yield return new TestCaseData(element6, container).SetName("ElementSizeIsZero");
        }
    }
    
    public static IEnumerable<TestCaseData> ElementIsNotVisibleInsideContainerTestCases
    {
        get
        {
            var container = new BoxContainer(Vector2.zero, Vector2.one ); //base case
            

            var element = new Element(_baseCaseCorner3 *10f, Vector2.one ); 
            yield return new TestCaseData(element, container).SetName("ElementPositionIsFarFromContainerBounds");
        }
    }

    public static IEnumerable<TestCaseData> ElementIsFullyVisibleInsideContainerTestCases
    {
        get
        {
            var container = new BoxContainer(Vector2.zero, Vector2.one*5 );
            
            var element = new Element(Vector2.zero, Vector2.one); 
            yield return new TestCaseData(element, container).SetName("ElementAndContainerPositionsAreEqualsAndElementSmallerThanContainer");
            
            var element1 = new Element(Vector2.one, Vector2.one); 
            yield return new TestCaseData(element1, container).SetName("ElementSmallerThanContainer");
        }
    }
    
    public static IEnumerable<TestCaseData> ElementIsNotFullyVisibleInsideContainerTestCases
    {
        get
        {
            var container = new BoxContainer(Vector2.zero, Vector2.one );
            
            var element = new Element(Vector2.zero, Vector2.one*5); 
            yield return new TestCaseData(element, container).SetName("ElementAndContainerPositionsAreEqualsAndElementBiggerThanContainer");
            
            var element1 = new Element(_baseCaseCorner0, Vector2.one); 
            yield return new TestCaseData(element1, container).SetName("ElementAndContainerPositionsAreDifferentButTheSameSize");
        }
    }
    
    
    // A Test behaves as an ordinary method
    [TestCaseSource(nameof(ElementIsVisibleInsideContainerTestCases))]
    public void ElementIsVisibleInsideContainer(Element element, Container container)
    {
        Assert.True(container.CheckIfElementIsVisible(element));
    }

    [TestCaseSource(nameof(ElementIsNotVisibleInsideContainerTestCases))]
    public void ElementIsNotVisibleInsideContainer(Element element, Container container)
    {
        Assert.False(container.CheckIfElementIsVisible(element));
    }
    
    [TestCaseSource(nameof(ElementIsFullyVisibleInsideContainerTestCases))]
    public void ElementIsFullyVisibleInsideContainer(Element element, Container container)
    {
        Assert.True(container.CheckIfElementIsFullyVisible(element));
    }
    
    [TestCaseSource(nameof(ElementIsNotFullyVisibleInsideContainerTestCases))]
    public void ElementIsNotFullyVisibleInsideContainer(Element element, Container container)
    {
        Assert.False(container.CheckIfElementIsFullyVisible(element));
    }
    
    
    // [Test]
    // public void ContainerShouldContainExpecetedNumberOfCarousels(int expectedCarouselNumber, Container container) 
    // {
    //     // container should be passed need to check if how many carousel it has.
    // }

    
}
