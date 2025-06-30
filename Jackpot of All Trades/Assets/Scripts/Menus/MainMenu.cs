using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;

    //splash text below title
    public TextMeshProUGUI splashText;
    [SerializeField] private List<string> splashPhrases;    //list of phrases
    [SerializeField] private List<string> recommendedGames; //list of game titles for the "also try" phrase (reduces too many of one message type)
    [SerializeField] private string gameRecommendationTemplate = "Also try {0}";
    private const string RecommendationKey = "GAME_RECOMMENDATION"; //key for the phrase string in splashPhrases

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
        string selected = splashPhrases[index];

        if (selected == RecommendationKey && recommendedGames.Count > 0)
        {
            string randomGame = recommendedGames[Random.Range(0, recommendedGames.Count)];
            splashText.text = string.Format(gameRecommendationTemplate, randomGame);
        }
        else
        {
            splashText.text = selected;
        }
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