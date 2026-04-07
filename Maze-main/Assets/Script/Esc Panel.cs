using UnityEngine;

public class EscPanel : MonoBehaviour
{
    [SerializeField] GameObject escPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            escPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameSettings.Pause = true;
        } 
    }
}
