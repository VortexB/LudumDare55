using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    public static GameController instance;
    [SerializeField] GameObject PauseMenu;
    bool isPaused;
    [SerializeField] Animator anim;
    [SerializeField] string optionsSceneName = "OptionsScene";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void UnPause()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void LoadOptions()
    {
        SceneManager.LoadSceneAsync(optionsSceneName, LoadSceneMode.Additive);
    }
    public void LoadLevel(int index)
    {
        LoadAnimation(index);
    }
    public void NextLevel()
    {
        LoadAnimation(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public int GetLevel()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public void RestartLevel()
    {
        LoadLevel(GetLevel());
    }
    void LoadAnimation(int index)
    {
        StartCoroutine(AnimLoad(index));
    }
    public void UnloadOptions()
    {
        SceneManager.UnloadSceneAsync(optionsSceneName);
    }
    public void Quit()
    {
        Application.Quit();
    }
    IEnumerator AnimLoad(int index)
    {
        if (anim == null) { SceneManager.LoadScene(index); yield break; }

        anim.SetTrigger("fadeOut");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(index);
    }
}
