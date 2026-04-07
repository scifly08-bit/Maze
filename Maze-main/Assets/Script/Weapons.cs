using UnityEngine;
using DG.Tweening;

using DG.Tweening;
    
[RequireComponent(typeof(AudioSource))]
public class Weapons : Interactable
{
    

    [SerializeField] private Sprite icon;
    [SerializeField] private Vector3 holdPosition = new Vector3(-4, 0, 8);
    [SerializeField] private Vector3 holdRotation = new Vector3(-15, -10, 0);
    
    [Header("Shooting")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private Vector3 recoilStrength = new Vector3(0, 0, -0.2f);
    [SerializeField] private Vector3 recoilRotationStrength = new Vector3(-10, 5, 0);
    [SerializeField] private float recoilDuration = 0.1f;
    
    private bool isHeld = false;
    private float nextFireTime = 0f;
    private AudioSource audioSource;
    private Vector3 currentRecoil;
    private Vector3 currentRecoilRotation;

    public override void Interact(InventorSlot Slot, Transform holder)
    {
        // 0. Stop idle animation
        transform.DOKill();

        // 1. Parent/Hold logic
        transform.SetParent(holder);
        transform.localPosition = holdPosition;
        transform.localRotation = Quaternion.Euler(holdRotation);
        
        // 2. Disable Physics
        if (TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
        if (TryGetComponent(out Collider col)) col.enabled = false;

        // 3. UI logic
        Slot.Item.sprite = icon;
        Slot.Item.enabled = true;
        Slot.ConnectedItem = gameObject;
        
        isHeld = true;
        if(audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isHeld)
        {
            HandleShooting();
            transform.localPosition = holdPosition + currentRecoil;
            transform.localRotation = Quaternion.Euler(holdRotation + currentRecoilRotation);
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // Sound
        if (fireSound) audioSource.PlayOneShot(fireSound);
        
        // Recoil
        // Punch the currentRecoil vector (kick back and return)
        // We kill any previous recoil to keep it snappy
        DOTween.Kill(this, "recoil"); 
        currentRecoil = Vector3.zero;
        currentRecoilRotation = Vector3.zero;
        
        // Position Recoil
        DOTween.Punch(() => currentRecoil, x => currentRecoil = x, recoilStrength, recoilDuration, 10, 1)
            .SetId("recoil");
            
        // Rotation Recoil
        DOTween.Punch(() => currentRecoilRotation, x => currentRecoilRotation = x, recoilRotationStrength, recoilDuration, 10, 1)
            .SetId("recoil");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    // Update is called once per frame
    
    
        
    
}
