
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// klasa do interakcji gracza z menu gry.
/// </summary>
public class MainMenu : MonoBehaviour
{

    public void PlayWithCP()
    {

        if (GameController.Instance != null)
            Destroy(GameController.Instance.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(1);  

    }
    public void AIvsCP()
    {
        if (GameController.Instance != null)
            Destroy(GameController.Instance.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(2);
    }
    public void PlayervsAIvsCP()
    {
        if (GameController.Instance != null)
        {
            Destroy(GameController.Instance.gameObject);
        }
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(3);
    }
    public void PlayWithAI()
    {
        if (GameController.Instance != null)
            Destroy(GameController.Instance.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(4);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadSceneAsync(0);

        Time.timeScale = 1f;
        if (GameController.Instance.gameIndex == 1)
            GameController.Instance.GetPlayerAI().EndEpisode();
        if (GameController.Instance != null)
            Destroy(GameController.Instance.gameObject);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Learning()
    {
        return ;
    }

}
