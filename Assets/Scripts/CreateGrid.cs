using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.UIElements;

public class CreateGrid : MonoBehaviour
{
   
    public int rows = 4;
    public int columns = 4;
    public RectTransform canvasRect;
    public float _spaceBetweenCells;

    public float width, height;

    public GameObject prefab;

    private float _cellWidth, _cellHeight;
    private RectTransform _recTransform;

    private List<Cell> _cells;

    private int _childCount;
    private Vector3 _lastPosition = Vector3.zero;
    private Vector2 _lastSize = Vector2.zero;
    
    private Vector2 _arrangeDirection;
    private Vector2 _size = Vector2.zero;

    private CustomLayoutGroupColumnConfig _customLayoutGroupColumn;
    private RectChildrenManager _rectChildrenManager;

    public bool test;
    public float speed = 0;
    
    private Vector2 _movement = Vector2.zero;

    private MovableElement[] _movableElements;
    private NativeArray <Rect> _movableElementRects;
    private NativeArray <int> _visibleElements;
    private int lastRectCount;

    private ContainerOverlapElementJob _containerOverlapElementJob;
    private JobHandle _containerOverlapElementJobHandle;
    

    public struct ContainerOverlapElementJob : IJobParallelFor
    {
        [ReadOnly]
        public  Rect container;
        [ReadOnly]
        public  NativeArray<Rect> elementArray;
        [WriteOnly]
        public NativeArray<int> results; // element is inside container

       
        public void Execute(int index)
        {
            var element = elementArray[index];
            if (container.Overlaps(element)) results[index] = 1;
            else results[index] = 0;
        }
    }

    // public struct visibleElementListJob : IJob
    // {
    //     [ReadOnly]
    //     public  NativeArray<Rect> elementArray;
    //     
    //     [ReadOnly]
    //     public  NativeArray<int> visibleElements;
    //
    //     [WriteOnly] public NativeArray<int> results;
    //     public void Execute()
    //     {
    //         var resultIndex = 0;
    //         for (var i = 0; i < elementArray.Length; i++)
    //         {
    //             if (resultIndex == results.Length)
    //                 break;
    //
    //             if (visibleElements[i] == 1)
    //             {
    //                 results[resultIndex] = 1;
    //                 resultIndex++;
    //             }
    //         }
    //     }
    // }
    
    public enum ArrangementDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    public ArrangementDirection arrangementDirection;

    private void Awake()
    {
        _recTransform = GetComponent<RectTransform>();
        _size = GetSize();
    }

    // Start is called before the first frame update
    void Start()
    {
       _cells = new List<Cell>();
       _customLayoutGroupColumn = GetComponent<CustomLayoutGroupColumnConfig>();
       _rectChildrenManager = GetComponent<RectChildrenManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (test)
        {
            
            for (int i = 0; i < 10; i++)
            {
                var go = Instantiate(prefab, transform);
            }
            //go.transform.SetAsFirstSibling();
            test = false;
        }
        
        // var rectArray = new Rect[mova]
        //
        // if (lastRectCount != _rectChildrenManager.rectList.Count)
        // {
        //     lastRectCount = _rectChildrenManager.rectList.Count;
        //     if (_movableElementRects.IsCreated)
        //     {
        //         _movableElementRects.Dispose();
        //         _movableElementRects = default;
        //     }
        //
        //     if (_visibleElements.IsCreated)
        //     {
        //         _visibleElements.Dispose();
        //         _visibleElements = default;
        //     }
        //         
        //     
        //     _movableElementRects = new NativeArray<Rect>(_rectChildrenManager.rectList.ToArray(), Allocator.Persistent);
        //     _visibleElements = new NativeArray<int>(lastRectCount, Allocator.Persistent);
        // }

        // _containerOverlapElementJob = new ContainerOverlapElementJob()
        // {
        //     container = _recTransform.GetWorldRect(),
        //     elementArray = _movableElementRects,
        //     results = _visibleElements
        // };
        // _containerOverlapElementJobHandle = _containerOverlapElementJob.Schedule(_movableElementRects.Length, 64);

       
        var contentArray = _customLayoutGroupColumn.GetColumnsContentObject();

        var factor = 1;
        foreach (var content in contentArray)
        {
            content.isVerticalAxis = true;
            content.MoveChildren((factor++)*speed * Time.deltaTime);
        }
        return;
        
        Vector3 currPosition = GetPosition();
        Vector2 currSize = GetSize();
        var currChildCount = transform.GetFirstLevelChildCount();
        var positionHasChanged = _lastPosition !=currPosition;
        var sizeHasChanged = _lastSize != currSize;
        if (positionHasChanged || sizeHasChanged || _childCount != currChildCount)
        {
            _childCount = currChildCount;
            _lastPosition = currPosition;
            _size = _lastSize =currSize;
            UpdateChildrenArrangement();
        }
        Draw();
    }

    private void LateUpdate()
    {
        // _containerOverlapElementJobHandle.Complete();
        // var results = _containerOverlapElementJob.results;
        // if(!results.Any()) return;
        //
        // UpdateMovableElementsVisibility(results);
    }

    
    private void UpdateMovableElementsVisibility(NativeArray<int> visibility)
    {
        // _movableElements = GetComponentsInChildren<MovableElement>();
        // // if (_movableElements.Count() == visibility.Length)
        // // {
        // //     Debug.Log("As coleções tem tamanho diferente");
        // // }
        // for (var i = 0; i < _movableElements.Length; i++)
        // {
        //     _movableElements[i].gameObject.SetActive(visibility[i]>0);
        // }
    }
    
    private void OnDestroy()
    {
        // _movableElementRects.Dispose();
        // _visibleElements.Dispose();
    }

    private void SetChildCount(int count)
    {
        _childCount = count;
    }

    private void OnValidate()
    {
        switch (arrangementDirection)
        {
            case ArrangementDirection.Left:
                _arrangeDirection = Vector2.left;
                break;
            case ArrangementDirection.Right:
                _arrangeDirection =Vector2.right;
                break;
            case ArrangementDirection.Up:
                _arrangeDirection = Vector2.up;
                break;
            case ArrangementDirection.Down:
                _arrangeDirection = Vector2.down;
                break;
        }
        UpdateChildrenArrangement();
    }

    private void UpdateChildrenArrangement()
    {
        if(_childCount==0) return;
        
        
        var children =  transform.GetFirstLevelChild<Transform>();


        var widthFactor = Mathf.Max(1, Mathf.Abs(_childCount * _arrangeDirection.x));
        var heightFactor = Mathf.Max(1, Mathf.Abs(_childCount * _arrangeDirection.y));
        
        var dimensionFactor = new Vector2(widthFactor, heightFactor);
        
        /***************************************************************************************************************
         * Para aplicar espaçamento entre as celulas de forma adequada sem que as mesmas escapem do container pai e suas
         * larguras se ajustem automaticamente de forma correta, ententa o seguinte conceito:
         *
         * Considera as afirmações:
         * 
         *  A) larguraDoContainerPai = x;
         *  B) larguraDeCadaFilhoDentroDoContainerPai = larguraDoContainerPai / numeroDeFilhosDoContainerPai;
         *
         * Levando em consideração A) e B) temos como resultado um containerPai preenchido por celulas filhas onde o soma-
         * tório de suas larguras é igual larguraDoContainerPai.
         *
         * Ao introduzir o conceito de espaçamento entre as células precisamos entender que cada novo espaçamento retira
         * um pouco da largura das células filhas. Sendo assim o espaço disponivel para ser dividido entre as células
         * filhas do container pai não será mais igual a x como na afirmação A), mas sim como nas proximas afirmações:
         *
         *  C) espaçamentoEntreCelulas = k;
         *  D) larguraDoContainerPai = x - (numeroDeFilhosDoContainerPai*espaçamentoEntreCelulas);
         *  E) larguraDoContainerPai = larguraDoContainerPai - espaçamentoEntreCelulas;
         *
         * Afirmação E) Atende o requisito de não haver espaçamento antes da primeira célula e depois da ultima célula.
         *          
         **************************************************************************************************************/
        
        var remainingSize = (_childCount * _spaceBetweenCells) - _spaceBetweenCells;

        var ratio = _size.y / _size.x;
        
        _cellWidth = (_size.x -remainingSize* Mathf.Abs(_arrangeDirection.x))/ dimensionFactor.x;
        _cellHeight = (_size.y -remainingSize* Mathf.Abs(_arrangeDirection.y))/ dimensionFactor.y;

        _cellHeight = _cellWidth*ratio;
        _cells.Clear();
        var childIndex = 0;

        foreach (var child in children)
        {
            if(child == transform) continue;

            var updateChild = GetUpdateChildComponent(child.gameObject);
            var xOffset = (_childCount > 1 ? (_size.x -_cellWidth)/2 : 0) - childIndex*_spaceBetweenCells;
            var yOffset = (_childCount > 1 ? (_size.y -_cellHeight)/2 : 0) - childIndex*_spaceBetweenCells;

            var posX = (_arrangeDirection.x * childIndex * _cellWidth) - xOffset * _arrangeDirection.x;
            var posY =  (_arrangeDirection.y * childIndex * _cellHeight) - yOffset*_arrangeDirection.y;
            
            //var space =  _childCount > 1 ? _spaceBetweenCells : 0;
            var pos = new Vector2(posX, posY); 
            var size = new Vector2(_cellWidth, _cellHeight);
            var cell = new Cell(pos, size);
            _cells.Add(cell);
            updateChild.RefreshData(cell); 
            childIndex++;

        }
    }

    private void Draw()
    {
        if(_cells==null) return;
        foreach (var cell in _cells)
        {
            Vector2 min =transform.TransformPoint(cell.Rect.min);
            Vector2 max = transform.TransformPoint(cell.Rect.max);
            
            //Debug.Log($"Min: {min} Max: {max}");
            //Debug.Log($"Global Min: {cell.Rect.min} Max: {cell.Rect.max}");
            DebugUtils.DrawRect(min, max, Color.black);
        }
    }

    private Vector3 GetPosition()
    {
        if (_recTransform)
        {
            return _recTransform.position;
        }
        else
        {
            return transform.position;
        }
    }
    
    private Vector2 GetSize()
    {
        if (_recTransform)
        {
            return _recTransform.sizeDelta;
        }
        else
        {
            return new Vector2(width, height);
        }
    }

    private UpdatePositionSize GetUpdateChildComponent( GameObject child)
    {
        var component = child.GetComponent<UpdatePositionSize>() ?? child.AddComponent<UpdatePositionSize>();
        return component;
    }

    private Vector2 LocalPivotPosition(RectTransform rectTransform, Vector2 parentSize)
    {
        if (rectTransform.anchorMin != rectTransform.anchorMax) return Vector2.zero;

        var aPosX = rectTransform.anchorMin * parentSize.x;
        var aPosy = rectTransform.anchorMin * parentSize.y;
        
        var posX = rectTransform.pivot.x * rectTransform.sizeDelta.x;
        var posY = rectTransform.pivot.y * rectTransform.sizeDelta.y;

        return new Vector2(posX, posY);
    }
    
}

public static class TransformExtensions
{
    public static int GetFirstLevelChildCount(this Transform transform)
    {
        var count = transform.Cast<Transform>().Count();
        
        Debug.Log($"num elementos: {count}");
        
        return count;
    }
    
    public static T[] GetFirstLevelChild<T>(this Transform transform)
    {
        var childList = (from Transform child in transform select child.GetComponent<T>() into component where component != null select component);
       // var firstLevelChildren = transform.Cast<Transform>();
       return childList.ToArray();
    }

    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];

        rectTransform.GetWorldCorners(corners);
        Vector3 pos = corners[0];

        Vector2 size = new Vector2(rectTransform.lossyScale.x * rectTransform.rect.size.x,
            rectTransform.lossyScale.y * rectTransform.rect.size.y);
        return new Rect(pos, size);
    }
}
