using UnityEngine;
using UnityEngine.SceneManagement;

public class Gatekeeper : MonoBehaviour
{
    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    public void LoadLevelAdditive(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Additive);
    }

    public void UnloadLevel(string name)
    {
        SceneManager.UnloadSceneAsync(name);
    }
}
