using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(CustomLayoutGroup))]
public class CustomLayoutGroupColumnConfig : MonoBehaviour
{

   
    private CustomLayoutGroup _customLayoutGroup;

    private int _previousColumnCount;

    // Start is called before the first frame update
    private void Start()
    {
        _previousColumnCount = 0;
        _customLayoutGroup = GetComponent<CustomLayoutGroup>();
        //_customLayoutGroup.OnAddNewColumn += ConfigureColumn;
        _customLayoutGroup.OnDetectNewChildren += HandleNewChildren;
    }
    
    
    /// <summary>
    /// When called, a child named column_columnIndex will be created.
    /// A GameObject, child of column_columnIndex, named content, will be create as well.
    /// The child parameter will become a son of the content game object variable. 
    /// </summary>
    /// <param name="child">This parameter will be made child of the content game object variable</param>
    /// <param name="columnIndex">This parameter will be concatenated with the column's name</param>
    /// <returns>returns the newly create and configured column</returns>
    private Transform ConfigureColumn(Transform child, int columnIndex)
    {
        var newColumn = new GameObject($"Column_{columnIndex}", typeof(RectTransform), typeof(VerticalLayoutGroup));
        var columnLayoutGroup = newColumn.GetComponent<VerticalLayoutGroup>();
        columnLayoutGroup.childForceExpandWidth = columnLayoutGroup.childForceExpandHeight = false;
        
        var content = new GameObject("content", typeof(RectTransform), typeof(MovableLayoutGroup));
        content.transform.SetParent(newColumn.transform, false);
        
        var contentLayoutGroup = content.GetComponent<MovableLayoutGroup>(); 
        contentLayoutGroup.childForceExpandWidth = contentLayoutGroup.childForceExpandHeight = false; // Faz o content se adaptar ao tamanho do filho.
        
        child.SetParent(content.transform, false);
        return newColumn.transform;
    }

    private void HandleNewChildren(IEnumerable<Transform> transforms)
    {
        var newChildrenTransforms = transforms.ToList();
        ConfigureChildren(newChildrenTransforms);
        ConfigureColumns(newChildrenTransforms);
    }

    private void ConfigureColumns(List<Transform> transforms)
    {
        var newChildrenTransforms = transforms;
        var childIndex = 0;
       
        foreach (var childTransform in newChildrenTransforms)
        {
            var containerIndex = (childIndex++) + _previousColumnCount;
            //OnAddChild?.Invoke(child);
            var newColumn = ConfigureColumn(childTransform, containerIndex);
            newColumn.transform.SetParent(transform,false);
        }
        _previousColumnCount += newChildrenTransforms.Count;
    }

    private void ConfigureChildren(List<Transform> transforms)
    {
        foreach (var childTransform in transforms)
        {
            var movableElement = childTransform.GetComponent<MovableElement>();
            if (movableElement is null) childTransform.gameObject.AddComponent<MovableElement>();
        }
    }
    
    public IEnumerable<MovableLayoutGroup> GetColumnsContentObject()
    {
        var columnList = _customLayoutGroup.GetColumns();
        return columnList?.Select(child => child.GetChild(0).GetComponent<MovableLayoutGroup>()).Where(content => content).ToArray();
    }
}
