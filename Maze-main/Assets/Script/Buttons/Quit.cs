using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit : Button
{
    protected override void OnClick()
    {
        transform.parent.gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    } 
}
