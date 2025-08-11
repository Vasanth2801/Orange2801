using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scene References")]
    public Transform cardContainer;    // drag the CardContainer (panel) here
    public TMP_Text scoreText;         // drag ScoreText (TMP) here
    public GameObject winScreen;       // drag WinScreen panel here

    [Header("Gameplay")]
    public float mismatchDelay = 0.8f;

    List<Card> cards = new List<Card>();
    Card firstCard = null;
    Card secondCard = null;
    int score = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // collect card instances that are children of cardContainer
        if (cardContainer == null)
        {
            Debug.LogError("GameManager: cardContainer not assigned.");
            return;
        }

        cards.Clear();
        foreach (Transform t in cardContainer)
        {
            Card c = t.GetComponent<Card>();
            if (c != null) cards.Add(c);
        }

        if (cards.Count == 0)
        {
            Debug.LogWarning("GameManager: no cards found under cardContainer.");
        }

        AssignRandomLetters();
        HideAllCards();

        UpdateScoreUI();

        if (winScreen != null) winScreen.SetActive(false);
    }

    void AssignRandomLetters()
    {
        int pairCount = cards.Count / 2;
        List<string> letters = new List<string>();

        for (int i = 0; i < pairCount; i++)
        {
            char ch = (char)('A' + (i % 26));
            string s = ch.ToString();
            letters.Add(s);
            letters.Add(s);
        }

        // simple shuffle
        for (int i = letters.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string tmp = letters[i];
            letters[i] = letters[j];
            letters[j] = tmp;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetLetter(letters[i]);
            cards[i].isMatched = false;
            cards[i].isRevealed = false;
        }
    }

    void HideAllCards()
    {
        foreach (var c in cards) c.HideInstant();
    }

    public void OnCardClicked(Card card)
    {
        if (firstCard == null)
        {
            firstCard = card;
            firstCard.RevealInstant();
            return;
        }

        if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            secondCard.RevealInstant();
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        // allow the player to see the second card
        yield return new WaitForSeconds(mismatchDelay);

        if (firstCard == null || secondCard == null)
        {
            ResetSelection();
            yield break;
        }

        if (firstCard.letter == secondCard.letter)
        {
            // correct
            firstCard.SetMatched();
            secondCard.SetMatched();

            score += 10;
        }
        else
        {
            // wrong
            firstCard.HideInstant();
            secondCard.HideInstant();

            score = Mathf.Max(0, score - 2);
        }

        UpdateScoreUI();

        // check win: all cards matched
        bool allMatched = true;
        foreach (var c in cards)
        {
            if (!c.isMatched) { allMatched = false; break; }
        }

        if (allMatched)
        {
            if (winScreen != null) winScreen.SetActive(true);
        }

        ResetSelection();
    }

    void ResetSelection()
    {
        firstCard = null;
        secondCard = null;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    
    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }


    public void SaveGame()
    {
        PlayerPrefs.SetString("SavedLevel", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt("SavedScore", score);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            string savedLevel = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(savedLevel);
        }
    }
}
