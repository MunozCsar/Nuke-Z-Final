using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_UX : MonoBehaviour
{ 
    [SerializeField] GameObject options;
    [SerializeField] GameObject graphics;
    [SerializeField] GameObject controls;
    [SerializeField] GameObject volume;
    [SerializeField] GameObject credits;
    public Image loadingBar;
    public GameObject loadingScreen;
    private void Start()
    {
        options.gameObject.SetActive(false);
        graphics.gameObject.SetActive(false);
        controls.gameObject.SetActive(false);
        volume.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
    }
    public void Play(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.fillAmount = progressValue;

            yield return null;
        }
    }
    public void Options()
    {
        options.gameObject.SetActive(true);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void BackOptions()
    {
        options.gameObject.SetActive(false);
    }
    public void BackGraphisAndVolumeAndControlsAndCredits()
    {
        graphics.gameObject.SetActive(false);
        volume.gameObject.SetActive(false);
        controls.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
    }
    
    public void Credits()
    {
        credits.gameObject.SetActive(true);
    }
    public void Graphics()
    {
        graphics.gameObject.SetActive(true);
    }
    public void Volume()
    {
        volume.gameObject.SetActive(true);
    }
    public void Controls()
    {
       controls.gameObject.SetActive(true);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
