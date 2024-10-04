using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DialTones", menuName = "ScriptableObjects/DialTones", order = 1)]
public class DialTones : ScriptableObject
{
    [InfoBox("Deve conter 12 sons, para as 12 teclas do telefone")]
    public List<AudioClip> clips = new List<AudioClip>();
}

