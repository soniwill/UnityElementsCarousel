using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// THIS CLASS IS USED AS AN COMPONENT THAT CREATES A GRID CONCEPT.
/// THE GAMEOBJECT USING THIS SCRIPT IS ALWAYS CONSIDERED AS THE CONTAINER.
/// ANY CHILD CREATED AT RUNTIME SHOULD GENERATE A COLUMN (A SUB-CONTAINER). EACH COLUMN CAN HAVE ONE OR MORE CHILDREN THAT
/// WILL BE CONSIDERED AS ROWS.
///
/// THE OnAddChild AND OnAddNewColumn DELEGATES CAN BE USED TO CREATE SOME EXTRA CONFIGURATIONS IF NEEDED.
///
/// AS AN EXAMPLE, THE DEFAULT BEHAVIOR WHEN A CHILD GAMEOBJECT IS CREATED IS TO CALL THE PRIVATE METHOD CreateSimpleContainer
/// IF THE DELEGATE OnAddNewColumn IS NULL. OTHERWISE SOME OTHER CUSTOM BEHAVIOR IS EXPECTED TO BE INJECTED AT THIS POINT. SEE THE
/// SCRIPT CustomLayoutGroupColumnConfig TO UNDERSTAND HOW A CUSTOM BEHAVIOR CAN BE CREATED.
///
/// ###################################### ATTENTION ################################################
/// IF YOU THINK THAT SHOULD CHANGE THIS CLASS, TRY TO CONSIDER IF IT'S A IMPROVEMENT OR BUG THAT NEED TO BE FIXED.
/// IF IT'S JUST A NEW FUNCTIONALITY THAT NEEDS TO BE ADDED, PLEASE, RECONSIDER CREATING A NEW SCRIPT THAT CAN BE USED
/// WITH ONE OF THE DELEGATES (OnAddChild AND OnAddNewColumn).
/// ###################################### ATTENTION ################################################
/// 
/// </summary>
public class CustomLayoutGroup : HorizontalOrVerticalLayoutGroup
{
    
    private HashSet<Transform> _columnList;

    public Action<Transform> OnAddChild;

    public Action<IEnumerable<Transform>> OnDetectNewChildren;
    
    /// <summary>
    /// This delegate is used to create a new child column for this layoutElement.
    /// use this when you need to implement a more complex column configuration/hierarchy.
    /// It passes as arguments a child transform and a index for the column and returns it's transform.
    /// </summary>
    public Func<Transform, int, Transform> OnAddNewColumn;

    public new void Start()
    {
        base.Start();
        _columnList = new HashSet<Transform>(transform.Cast<Transform>());
    }
    
    private new void Update()
    {
        base.Update();
        
        if (!Application.isPlaying) return;
        if (transform.childCount == _columnList.Count) return;
        
        DetectChildrenChange();
    }
    
    /// <summary>
    /// The general concept that MUST be followed on this script is:
    /// This container will ALWAYS have one row of columns. Each column can contains any number of children.
    ///
    /// The standard behavior is:
    /// Every time a new transform child is created or inserted a new transform (column), also child, take it's place and becomes it's parent.
    /// See the OnNewChildren method.
    /// 
    /// However, the standard behavior can be slight changed using the delegate OnDetectNewChildren, as long as the general concept is respected.
    /// If the General concept is not respected an undefined behavior is expected.
    /// See the class CustomLayoutGroupColumnConfig as an example of how the default behavior can be modified.
    /// 
    /// </summary>
    private void DetectChildrenChange()
    {
        var tempChildren = new HashSet<Transform>(transform.Cast<Transform>());
        if (transform.childCount > _columnList.Count)
        {
            
            var transforms = _columnList.Count > 0 ? tempChildren.Except(_columnList) : tempChildren;

            //var transforms = list.Cast<Transform>(); 
            
            if (OnDetectNewChildren is null)
            {
                OnNewChildren(transforms);
            }
            else
            {
                //send a array of new transforms inserted as children of this gameObject's transform
                OnDetectNewChildren(transforms);
                _columnList.UnionWith(transform.Cast<Transform>());
            }
        }
        else if (transform.childCount < _columnList.Count)
        {
            _columnList.IntersectWith(tempChildren);
        }
    }

    private void OnNewChildren(IEnumerable<Transform> transforms)
    {
        var childIndex = 0;
        var previousChildCount = _columnList.Count;
        foreach (var child in transforms)
        {
            var containerIndex = (childIndex++) + previousChildCount;
            OnAddChild?.Invoke(child);
            CreateContainerForNewChild(child, containerIndex);
        }
    }


    private void CreateContainerForNewChild(Transform child, int containerIndex)
    {
        CreateSimpleContainer(child, containerIndex);
        // if (OnAddNewColumn is null)
        // {
        //     CreateSimpleContainer(child, containerIndex);
        // }
        // else
        // {
        //     var newColumn =  OnAddNewColumn?.Invoke(child, containerIndex);
        //     ///We try to always keep _childList.count == transform.childCount.
        //     /// if for some reason newColumn is null, we underestand that no more columns
        //     /// should be created and thus destroy the child to mantain _childList.count == transform.childCount.
        //     if (newColumn)
        //     {
        //         newColumn.transform.SetParent(transform,false);
        //         _columnList.Add(newColumn.transform);   
        //     }
        //     else
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }
    }

    private void CreateSimpleContainer(Transform child, int containerIndex)
    {
        var newColumn = new GameObject($"Column_{containerIndex}", typeof(RectTransform), typeof(VerticalLayoutGroup));
        newColumn.transform.SetParent(transform,false);
        child.SetParent(newColumn.transform,false);
        _columnList.Add(newColumn.transform);
        //OnAddChild?.Invoke(child);
    }

    public override void CalculateLayoutInputHorizontal()
    {
        
        base.CalculateLayoutInputHorizontal();
        CalcAlongAxis(0, false);
    }
    
    public override void CalculateLayoutInputVertical()
    {
        CalcAlongAxis(1, false);
    }

    public override void SetLayoutHorizontal()
    {
        SetChildrenAlongAxis(0, false);
    }

    public override void SetLayoutVertical()
    {
        SetChildrenAlongAxis(1, false);
    }

    public Transform[] GetColumns()
    {
        return _columnList?.ToArray();
    }
}





