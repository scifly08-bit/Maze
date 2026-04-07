using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EXIT : MonoBehaviour
{
    public void Exit()
    {
        Image ExitIMG = GetComponent<Image>();
        if (ExitIMG != null)
        {
            ExitIMG.transform.localScale = new Vector3 (0.8f, 0.8f, 1f);
        }
        else
        {
            Debug.Log("No IMG found");
        }
        Invoke("GoToMainMenu", 0.1f);
    }

    void GoToMainMenu()
    {
        //SceneManager.LoadScene("MainMenu");
    }
}
