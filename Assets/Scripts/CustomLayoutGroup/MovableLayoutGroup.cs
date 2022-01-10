using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovableLayoutGroup : LayoutGroup
{

    public bool isVerticalAxis;
    [SerializeField] protected float m_Spacing = 0;

    /// <summary>
    /// The spacing to use between layout elements in the layout group.
    /// </summary>
    public float spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

    [SerializeField] protected bool m_ChildForceExpandWidth = true;

    /// <summary>
    /// Whether to force the children to expand to fill additional available horizontal space.
    /// </summary>
    public bool childForceExpandWidth { get { return m_ChildForceExpandWidth; } set { SetProperty(ref m_ChildForceExpandWidth, value); } }

    [SerializeField] protected bool m_ChildForceExpandHeight = true;

    /// <summary>
    /// Whether to force the children to expand to fill additional available vertical space.
    /// </summary>
    public bool childForceExpandHeight { get { return m_ChildForceExpandHeight; } set { SetProperty(ref m_ChildForceExpandHeight, value); } }

    [SerializeField] protected bool m_ChildControlWidth = true;

    /// <summary>
    /// Returns true if the Layout Group controls the widths of its children. Returns false if children control their own widths.
    /// </summary>
    /// <remarks>
    /// If set to false, the layout group will only affect the positions of the children while leaving the widths untouched. The widths of the children can be set via the respective RectTransforms in this case.
    ///
    /// If set to true, the widths of the children are automatically driven by the layout group according to their respective minimum, preferred, and flexible widths. This is useful if the widths of the children should change depending on how much space is available.In this case the width of each child cannot be set manually in the RectTransform, but the minimum, preferred and flexible width for each child can be controlled by adding a LayoutElement component to it.
    /// </remarks>
    public bool childControlWidth { get { return m_ChildControlWidth; } set { SetProperty(ref m_ChildControlWidth, value); } }

    [SerializeField] protected bool m_ChildControlHeight = true;

    /// <summary>
    /// Returns true if the Layout Group controls the heights of its children. Returns false if children control their own heights.
    /// </summary>
    /// <remarks>
    /// If set to false, the layout group will only affect the positions of the children while leaving the heights untouched. The heights of the children can be set via the respective RectTransforms in this case.
    ///
    /// If set to true, the heights of the children are automatically driven by the layout group according to their respective minimum, preferred, and flexible heights. This is useful if the heights of the children should change depending on how much space is available.In this case the height of each child cannot be set manually in the RectTransform, but the minimum, preferred and flexible height for each child can be controlled by adding a LayoutElement component to it.
    /// </remarks>
    public bool childControlHeight { get { return m_ChildControlHeight; } set { SetProperty(ref m_ChildControlHeight, value); } }

    [SerializeField] protected bool m_ChildScaleWidth = false;

    /// <summary>
    /// Whether to use the x scale of each child when calculating its width.
    /// </summary>
    public bool childScaleWidth { get { return m_ChildScaleWidth; } set { SetProperty(ref m_ChildScaleWidth, value); } }

    [SerializeField] protected bool m_ChildScaleHeight = false;

    /// <summary>
    /// Whether to use the y scale of each child when calculating its height.
    /// </summary>
    public bool childScaleHeight { get { return m_ChildScaleHeight; } set { SetProperty(ref m_ChildScaleHeight, value); } }

    /// <summary>
    /// Whether the order of children objects should be sorted in reverse.
    /// </summary>
    /// <remarks>
    /// If False the first child object will be positioned first.
    /// If True the last child object will be positioned first.
    /// </remarks>
    public bool reverseArrangement { get { return m_ReverseArrangement; } set { SetProperty(ref m_ReverseArrangement, value); } }

    [SerializeField] protected bool m_ReverseArrangement = false;

    [SerializeField] private Vector2 _moveOffset;
    
    /// <summary>
    /// Offset que pode ser atualizado para permitir a movimentação dos elementos filhos do gameobject atrelado a este component.
    /// </summary>
    public Vector2 MoveOffset { get { return _moveOffset; } set { SetProperty(ref _moveOffset, value); } }
        
    
    protected void CalcAlongAxis(int axis, bool isVertical)
    {
        float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);
        bool controlSize = (axis == 0 ? m_ChildControlWidth : m_ChildControlHeight);
        bool useScale = (axis == 0 ? m_ChildScaleWidth : m_ChildScaleHeight);
        bool childForceExpandSize = (axis == 0 ? m_ChildForceExpandWidth : m_ChildForceExpandHeight);

        float totalMin = combinedPadding;
        float totalPreferred = combinedPadding;
        float totalFlexible = 0;

        bool alongOtherAxis = (isVertical ^ (axis == 1));
        var rectChildrenCount = rectChildren.Count;
        for (int i = 0; i < rectChildrenCount; i++)
        {
            RectTransform child = rectChildren[i];
            float min, preferred, flexible;
            GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);

            if (useScale)
            {
                float scaleFactor = child.localScale[axis];
                min *= scaleFactor;
                preferred *= scaleFactor;
                flexible *= scaleFactor;
            }

            if (alongOtherAxis)
            {
                totalMin = Mathf.Max(min + combinedPadding, totalMin);
                totalPreferred = Mathf.Max(preferred + combinedPadding, totalPreferred);
                totalFlexible = Mathf.Max(flexible, totalFlexible);
            }
            else
            {
                totalMin += min + spacing;
                totalPreferred += preferred + spacing;

                // Increment flexible size with element's flexible size.
                totalFlexible += flexible;
            }
        }

        if (!alongOtherAxis && rectChildren.Count > 0)
        {
            totalMin -= spacing;
            totalPreferred -= spacing;
        }
        totalPreferred = Mathf.Max(totalMin, totalPreferred);
        SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, axis);
    }
    
    protected void SetChildrenAlongAxis(int axis, bool isVertical)
    {
        float size = rectTransform.rect.size[axis];
        bool controlSize = (axis == 0 ? m_ChildControlWidth : m_ChildControlHeight);
        bool useScale = (axis == 0 ? m_ChildScaleWidth : m_ChildScaleHeight);
        bool childForceExpandSize = (axis == 0 ? m_ChildForceExpandWidth : m_ChildForceExpandHeight);
        float alignmentOnAxis = GetAlignmentOnAxis(axis);

        bool alongOtherAxis = (isVertical ^ (axis == 1));
        int startIndex = m_ReverseArrangement ? rectChildren.Count - 1 : 0;
        int endIndex = m_ReverseArrangement ? 0 : rectChildren.Count;
        int increment = m_ReverseArrangement ? -1 : 1;
        var offset = axis == 0 ? _moveOffset.x : _moveOffset.y;
        
        if (alongOtherAxis)
        {
            float innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);

            for (int i = startIndex; m_ReverseArrangement ? i >= endIndex : i < endIndex; i += increment)
            {
                RectTransform child = rectChildren[i];
                float min, preferred, flexible;
                GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);
                float scaleFactor = useScale ? child.localScale[axis] : 1f;

                float requiredSpace = Mathf.Clamp(innerSize, min, flexible > 0 ? size : preferred);
                float startOffset = GetStartOffset(axis, requiredSpace * scaleFactor) + offset;
                if (controlSize)
                {
                    SetChildAlongAxisWithScale(child, axis, startOffset, requiredSpace, scaleFactor);
                }
                else
                {
                    float offsetInCell = (requiredSpace - child.sizeDelta[axis]) * alignmentOnAxis;
                    SetChildAlongAxisWithScale(child, axis, startOffset + offsetInCell, scaleFactor);
                }
            }
        }
        else
        {
           
            float pos = (axis == 0 ? padding.left : padding.top) +offset;
            float itemFlexibleMultiplier = 0;
            float surplusSpace = size - GetTotalPreferredSize(axis);

            if (surplusSpace > 0)
            {
                if (GetTotalFlexibleSize(axis) == 0)
                    pos = GetStartOffset(axis, GetTotalPreferredSize(axis) - (axis == 0 ? padding.horizontal : padding.vertical)) + offset;
                else if (GetTotalFlexibleSize(axis) > 0)
                    itemFlexibleMultiplier = surplusSpace / GetTotalFlexibleSize(axis);
            }

            float minMaxLerp = 0;
            if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
                minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));

            for (int i = startIndex; m_ReverseArrangement ? i >= endIndex : i < endIndex; i += increment)
            {
                RectTransform child = rectChildren[i];
                float min, preferred, flexible;
                GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);
                float scaleFactor = useScale ? child.localScale[axis] : 1f;

                float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
                childSize += flexible * itemFlexibleMultiplier;
                
                if (controlSize)
                {
                    SetChildAlongAxisWithScale(child, axis, pos, childSize, scaleFactor);
                }
                else
                {
                    float offsetInCell = (childSize - child.sizeDelta[axis]) * alignmentOnAxis;
                    SetChildAlongAxisWithScale(child, axis, pos + offsetInCell, scaleFactor);
                }
                pos += childSize * scaleFactor + spacing;
            }
        }
    }

    private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand,
        out float min, out float preferred, out float flexible)
    {
        if (!controlSize)
        {
            min = child.sizeDelta[axis];
            preferred = min;
            flexible = 0;
        }
        else
        {
            min = LayoutUtility.GetMinSize(child, axis);
            preferred = LayoutUtility.GetPreferredSize(child, axis);
            flexible = LayoutUtility.GetFlexibleSize(child, axis);
        }

        if (childForceExpand)
            flexible = Mathf.Max(flexible, 1);
    }
    
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        CalcAlongAxis(0, isVerticalAxis);
    }

    public override void CalculateLayoutInputVertical()
    {
        CalcAlongAxis(1, isVerticalAxis);
    }

    public override void SetLayoutHorizontal()
    {
        SetChildrenAlongAxis(0, isVerticalAxis);
    }

    public override void SetLayoutVertical()
    {
        SetChildrenAlongAxis(1, isVerticalAxis);
    }
    
    /// <summary>
    /// Update the positions of the child layout elements. 
    /// </summary>
    /// <param name="speed"></param>

    public void MoveChildren(float speed)
    {
        if (isVerticalAxis)
        {
            MoveOffset += Vector2.up * speed;
        }
        else
        {
            MoveOffset += Vector2.right * speed;
        }
    }
}
