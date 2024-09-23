using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Data", menuName = "Scriptable Objects/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [SerializeField][TextArea] private string[] dialogue;
    [SerializeField] int numberOfAudioQueues = 1;
    [SerializeField] AudioClip audio;

    public string[] Dialogue => dialogue;
}
