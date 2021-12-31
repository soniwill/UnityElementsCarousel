using System.Collections;
using System.Collections.Generic;

public abstract class Container
{
    public abstract bool CheckIfElementIsVisible(Element element);
    public abstract bool CheckIfElementIsFullyVisible(Element element);
}