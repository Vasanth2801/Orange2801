using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scene References")]
    public Transform cardContainer;        // drag the CardContainer Panel here

    [Header("Gameplay")]
    public float mismatchDelay = 0.8f;

    List<Card> cards = new List<Card>();
    Card firstCard = null;
    Card secondCard = null;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (cardContainer == null)
        {
            Debug.LogError("GameManager: cardContainer not assigned.");
            return;
        }

        // collect cards (instances) under the container
        Card[] arr = cardContainer.GetComponentsInChildren<Card>();
        cards = new List<Card>(arr);

        if (cards.Count == 0)
        {
            Debug.LogWarning("No card instances found under cardContainer.");
            return;
        }

        if (cards.Count % 2 != 0)
        {
            Debug.LogWarning("Odd number of cards found. Make card count even for pairs.");
        }

        AssignRandomLetters();
        HideAllCards();
    }

    void AssignRandomLetters()
    {
        int pairCount = cards.Count / 2;
        List<string> letters = new List<string>();

        for (int i = 0; i < pairCount; i++)
        {
            char ch = (char)('A' + (i % 26)); // wraps after Z back to A if needed
            string s = ch.ToString();
            letters.Add(s);
            letters.Add(s);
        }

        // Fisher-Yates shuffle
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

    public void OnCardClicked(Card clicked)
    {
        if (firstCard == null)
        {
            firstCard = clicked;
            firstCard.RevealInstant();
            return;
        }

        if (secondCard == null && clicked != firstCard)
        {
            secondCard = clicked;
            secondCard.RevealInstant();
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        // allow player to see the second card
        yield return new WaitForSeconds(mismatchDelay);

        if (firstCard == null || secondCard == null)
        {
            ResetSelection();
            yield break;
        }

        if (firstCard.letter == secondCard.letter)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();
        }
        else
        {
            firstCard.HideInstant();
            secondCard.HideInstant();
        }

        ResetSelection();
    }

    void ResetSelection()
    {
        firstCard = null;
        secondCard = null;
    }
}
