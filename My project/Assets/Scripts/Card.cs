using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    [Header("UI References")]
    public GameObject front;               // panel that shows the letter
    public GameObject back;                // panel that shows face-down
    public TextMeshProUGUI letterText;     // TMP text inside front (shows letter)

    [HideInInspector] public string letter;
    [HideInInspector] public bool isRevealed = false;
    [HideInInspector] public bool isMatched = false;

    private Button button;

    private void Awake()
    {
        // wire button click safely (removes other listeners to avoid dupes)
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnCardClicked);
        }

        // try auto-assign TMP if not set
        if (letterText == null && front != null)
            letterText = front.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetLetter(string newLetter)
    {
        letter = newLetter;
        if (letterText != null)
            letterText.text = letter;
    }

    public void OnCardClicked()
    {
        if (isMatched || isRevealed) return;
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnCardClicked(this);
    }

    public void RevealInstant()
    {
        isRevealed = true;
        if (front != null) front.SetActive(true);
        if (back != null) back.SetActive(false);
    }

    public void HideInstant()
    {
        if (isMatched) return; // matched cards stay revealed
        isRevealed = false;
        if (front != null) front.SetActive(false);
        if (back != null) back.SetActive(true);
    }

    public void SetMatched()
    {
        isMatched = true;
        // disable button so it can't be clicked
        if (button != null) button.interactable = false;
    }
}
