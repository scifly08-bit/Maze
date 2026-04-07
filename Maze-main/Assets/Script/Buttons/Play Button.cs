using UnityEngine;
using UnityEngine.SceneManagement;
 public class PlayButton : Button
{
   protected override void OnClick()
   {
       SceneManager.LoadScene("Level1");
   }
}
   
        
    

