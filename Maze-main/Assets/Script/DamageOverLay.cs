using UnityEngine;
using UnityEngine.UI;
public class DamageOverLay : MonoBehaviour
{
    [Header("OverLaySettings")]
    [SerializeField] private  CanvasGroup canvasGroup;
    [SerializeField] private  float FadeSpeed=2f;
    [SerializeField] private  float LowHealthThreshold=0.3f;
    [SerializeField] private  float MaxOverlayAlpha=0.6f;
    private float TargetAlpha=0f;
    void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, TargetAlpha, FadeSpeed * Time.deltaTime);
        }
    }

    public void SetOverLayIntesity(float healthpercentage)
    {
        if (healthpercentage < LowHealthThreshold)
        {
            float intensity = 1f-(healthpercentage / LowHealthThreshold);
            TargetAlpha = intensity*MaxOverlayAlpha;
        }
        else
        {
            TargetAlpha = 0f;
        }
    }

    public void FlashDamage()
    {
        canvasGroup.alpha = MaxOverlayAlpha;
    }
}
