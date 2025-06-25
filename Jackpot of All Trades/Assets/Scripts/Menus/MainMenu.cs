using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public TextMeshProUGUI splashText;
    [SerializeField] private List<string> splashPhrases;

    //set options menu to false and all that
    private void Start()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        RandomizeSplash();
    }

    //the little phrasses below title
    public void RandomizeSplash()
    {
        if (splashPhrases == null || splashPhrases.Count == 0 || splashText == null) return;

        int index = Random.Range(0, splashPhrases.Count);
        splashText.text = splashPhrases[index];
    }

    //loads into battle scene currently
    public void StartGame()
    {
        SceneManager.LoadScene("BattleScene");
    }

    //makes options panel visible
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        RandomizeSplash();
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}