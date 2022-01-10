using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MovableElement : MonoBehaviour
{
    public struct CheckIfInsideRectJob : IJob
    {
        public Rect ContainerRect;
        public Rect ChildElementRect;
        public NativeArray<int> result;

        public void Execute()
        {
            result[0] = ContainerRect.Overlaps(ChildElementRect)?1:0;
        }
    }
    private RectTransform _rectTransform;

    private Rect _containerRect;

    private NativeArray<int> _results;
    private CheckIfInsideRectJob _checkIfInsideJob;
    private JobHandle CheckIfInsideRectJobHandle;
    public bool isVisible;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        _results = new NativeArray<int>(1, Allocator.TempJob);

        _checkIfInsideJob = new CheckIfInsideRectJob()
        {
            ContainerRect = _containerRect,
            ChildElementRect = _rectTransform.GetWorldRect(),
            result = _results
        };
        CheckIfInsideRectJobHandle = _checkIfInsideJob.Schedule();
        
    }

    private void LateUpdate()
    {
        
        CheckIfInsideRectJobHandle.Complete();
        isVisible = _checkIfInsideJob.result[0]>0;
        _results.Dispose();
        gameObject.SetActive(isVisible);
    }

    public void SetContainerRect(Rect container)
    {
        _containerRect = container;
    }

    public Rect GetWorldRect()
    {
        return _rectTransform.GetWorldRect();
    }
    
    
}
