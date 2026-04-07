using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEditor.Compilation;

public abstract class Button : MonoBehaviour, IPointerDownHandler,  IPointerUpHandler

{
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (_animation != null && _animation.IsPlaying() && _animation.IsActive())
        {
            _animation.Kill();
        }

        _animation = transform.DOScale(Size, factorTime).OnComplete(()=>OnClick());
    }
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (_animation != null && _animation.IsPlaying() && _animation.IsActive())
        {
            return;
        }
        _animation = transform.DOScale(Size * factor, factorTime);
    }

    protected Vector3 Size;

    [SerializeField] protected float factor = 0.5f;

    [SerializeField] protected float factorTime = 1f;

    private Tween _animation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Size = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected abstract void OnClick();
}

