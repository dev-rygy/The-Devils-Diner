using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneManager : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] PlayableDirector cutSceneTimeline;
    [SerializeField][Range(0, 10)] float cutScenePauseTime_1, cutScenePauseTime_2;

    [Header("Dialogue")]
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] float dialSoundDelay = 0.5f;
    [SerializeField] DialogueData cutSceneDialogue_1;
    [SerializeField] DialogueData cutSceneDialogue_2;

    private NavigationManager navigationManager;
    private TypewriterEffect typewriterEffect;
    private AudioSource audioSource;
    private bool buttonClick = false;
    private bool dialoguePrompt_1 = false;
    private bool dialoguePrompt_2 = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        navigationManager = GetComponent<NavigationManager>();
        typewriterEffect = GetComponent<TypewriterEffect>();
    }

    private void Update()
    {
        // Play Dialogue pt. 1
        if (!dialoguePrompt_1 && cutSceneTimeline.time >= cutScenePauseTime_1)
        {
            cutSceneTimeline.Pause();
            PlayDialogue(cutSceneDialogue_1);
            dialoguePrompt_1 = true;
        }
        // Play Dialogue pt. 2
        else if (!dialoguePrompt_2 && cutSceneTimeline.time >= cutScenePauseTime_2)
        {
            cutSceneTimeline.Pause();
            PlayDialogue(cutSceneDialogue_2);
            dialoguePrompt_2 = true;
        }
        // Go to Game Scene
        else if (cutSceneTimeline.time >= cutSceneTimeline.duration)
            navigationManager.ToGame();
    }

    private void PlayDialogue(DialogueData dialogueData)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueData));
    }

    private IEnumerator StepThroughDialogue(DialogueData dialogueData)
    {
        IEnumerator coroutine = DialogueSound();

        foreach (string dialogue in dialogueData.Dialogue)
        {
            StartCoroutine(coroutine);
            yield return typewriterEffect.Run(dialogue, dialogueText);
            StopCoroutine(coroutine);
            yield return new WaitUntil(() => buttonClick); // Need to convert to next button!
            buttonClicked();
        }
        CloseDialogueBox();
        cutSceneTimeline.Play();
    }

    private IEnumerator DialogueSound()
    {
        while (true)
        {
            audioSource.Play();
            yield return new WaitForSeconds(dialSoundDelay);
        }
    }

    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        dialogueText.text = string.Empty;
    }

    public void buttonClicked()
    {
        if (buttonClick == false)
            buttonClick = true;
        else
            buttonClick = false;
    }
}
