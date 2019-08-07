using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class MainMenuScript : MonoBehaviour
    {
        public void NewGame()
        {
            SceneManager.LoadScene("WorldScene");
        }

        public void ResumeGame()
        {
            SceneManager.LoadScene("WorldScene");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
