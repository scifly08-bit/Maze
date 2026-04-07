using System;
using UnityEngine;
using DG.Tweening;
public abstract class Interactable: MonoBehaviour
{
    [SerializeField] private float Duration = 2;
    public abstract void Interact(InventorSlot Slot, Transform holder);

    private void Start()
    {
        Rotation();
    }

    private void Update()
    {
        
    }

    private void Rotation()
    {
        transform.DORotate(new Vector3(0, 360, 0), Duration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }
}