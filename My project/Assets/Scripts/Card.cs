using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public GameObject front;               // assign the Front panel (contains TMP)
    public GameObject back;                // assign the Back panel (face-down)
    public TextMeshProUGUI letterText;     // assign the TMP text inside front

    [HideInInspector] public string letter;
    [HideInInspector] public bool isRevealed = false;
    [HideInInspector] public bool isMatched = false;

    Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnCardClicked);
        }

        if (letterText == null && front != null)
            letterText = front.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetLetter(string s)
    {
        letter = s;
        if (letterText != null) letterText.text = s;
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
        if (isMatched) return; // matched stays visible
        isRevealed = false;
        if (front != null) front.SetActive(false);
        if (back != null) back.SetActive(true);
    }

    public void SetMatched()
    {
        isMatched = true;
        // optionally disable button so it can't be clicked
        if (btn != null) btn.interactable = false;
    }
}
