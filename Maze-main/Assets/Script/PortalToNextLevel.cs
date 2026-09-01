using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToNextLevel : MonoBehaviour
{
  [SerializeField] private string nextSceneName = "Level2";

  private void OnTriggerEnter(Collider collision){
    if(collision.CompareTag ("Player")){
        SceneManager.LoadScene(nextSceneName);
    }
  }
}
