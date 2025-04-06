using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Game_Manager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        [TextArea(2, 5)] public string message;
        public Sprite characterSprite;
        public Sprite background;
        public bool hasChoices;
        public List<Choice> choices;
        public bool isEnd;
        public string nextScene;
    }

    [System.Serializable]
    public class Choice
    {
        public string choiceText;
        public ActionType actionType;
        public string actionValue;
    }

    public enum ActionType
    {
        NextLine,
        LoadScene,
        ExitGame
    }

    [Header("UI")]
    public Image backgroundImage;
    public Image characterImage;
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TMP_Text[] choiceButtonTexts;
    public Button skipButton;

    [Header("dialog")]
    public List<DialogueLine> dialogues;

    [Header("Typing Settings")]
    public float typingSpeed = 0.02f;

    // Glitch Effect
    public Image glitchImage; // Reference to the glitch effect image

    private int currentDialogueIndex = 0;
    private int skipCounter = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool textFullyDisplayed = false;

    void Start()
    {
        ShowDialogue(currentDialogueIndex);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !choicePanel.activeSelf)
        {
            if (isTyping)
            {
                SkipTypingEffect();
            }
            else
            {
                OnNextDialogue(); // Ensure this advances the dialogue
            }
        }
    }

    void ShowDialogue(int index)
    {
        if (index >= dialogues.Count) return;

        DialogueLine line = dialogues[index];

        characterNameText.text = line.characterName;
        backgroundImage.sprite = line.background;
        characterImage.sprite = line.characterSprite;
        characterImage.gameObject.SetActive(line.characterSprite != null);

        choicePanel.SetActive(false);
        dialogueText.text = "";

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line.message));

        // Якщо є вибори, відобразимо панель виборів
        if (line.hasChoices && line.choices != null && line.choices.Count > 0)
        {
            choicePanel.SetActive(true);
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < line.choices.Count) // Перевіряємо, чи є вибір
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    choiceButtonTexts[i].text = line.choices[i].choiceText;

                    // Оновлюємо слухачів для кнопок вибору
                    int choiceIndex = i; // Локальна копія
                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(line.choices[choiceIndex]));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator TypeText(string message)
    {
        isTyping = true;
        textFullyDisplayed = false;
        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        textFullyDisplayed = true;
    }

    void SkipTypingEffect()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = dialogues[currentDialogueIndex].message;
        isTyping = false;
        textFullyDisplayed = true;
    }

    public void OnNextDialogue()
    {
        if (currentDialogueIndex < dialogues.Count - 1)
        {
            currentDialogueIndex++;
            ShowDialogue(currentDialogueIndex);
        }
        else
        {
            // Do something when you reach the end, e.g., load next scene or finish the game
            if (dialogues[currentDialogueIndex].isEnd)
            {
                LoadNextScene(); // You can load the next scene if isEnd is true
            }
            else
            {
                ShowDialogue(currentDialogueIndex); // Stay on last dialogue
            }
        }
    }

    void OnChoiceSelected(Choice choice)
    {
        // Обробляємо вибір залежно від actionType
        switch (choice.actionType)
        {
            case ActionType.NextLine:
                if (int.TryParse(choice.actionValue, out int nextIndex))
                {
                    currentDialogueIndex = nextIndex;
                    ShowDialogue(currentDialogueIndex);
                }
                else
                {
                    Debug.LogWarning("Invalid action value for NextLine");
                }
                break;

            case ActionType.LoadScene:
                StartCoroutine(GlitchEffectThenLoadScene(choice.actionValue));
                break;

            case ActionType.ExitGame:
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
        }
    }

    public void OnSkipDialogue()
    {
        skipCounter++;
        if (skipCounter >= 5)
        {
            skipCounter = 0;
            currentDialogueIndex = FindSpecialDialogue("AntiSkip");
        }
        else
        {
            currentDialogueIndex++;
        }

        ShowDialogue(currentDialogueIndex);
    }

    int FindSpecialDialogue(string marker)
    {
        for (int i = 0; i < dialogues.Count; i++)
        {
            if (dialogues[i].characterName == marker)
                return i;
        }
        return currentDialogueIndex;
    }

    // Glitch effect before loading the next scene
    IEnumerator GlitchEffectThenLoadScene(string sceneName)
    {
        // Activate the glitch image
        glitchImage.gameObject.SetActive(true);

        // Simulate glitch (you can animate the glitchImage or apply some distortion here)
        float timePassed = 0f;
        while (timePassed < 1f)  // Duration of the glitch effect (1 second)
        {
            // Random color flicker for glitch effect (this can be replaced with animation or shader)
            glitchImage.color = new Color(Random.value, Random.value, Random.value);
            timePassed += Time.deltaTime;
            yield return null;
        }

        // Deactivate the glitch image after the effect is done
        glitchImage.gameObject.SetActive(false);

        // Load the next scene
        SceneManager.LoadScene(sceneName);
    }

    void LoadNextScene()
    {
        StartCoroutine(GlitchEffectThenLoadScene(dialogues[currentDialogueIndex].nextScene));
    }
}
