using UnityEngine;

public class Resume : Button
{
    protected override void OnClick()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameSettings.Pause = false;
        transform.parent.gameObject.SetActive(false);
    } 
}
