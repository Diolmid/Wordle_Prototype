using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Serializable]
    public class State
    {
        public Color fillColor;
        public Color outlineColor;
    }

    public State CurrentState { get; private set; }
    public char Letter { get; private set; }

    private TextMeshProUGUI _text;
    private Image _fill;
    private Outline _outline;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _fill = GetComponent<Image>();
        _outline = GetComponent<Outline>();
    }

    public void SetLetter(char letter)
    {
        Letter = letter;
        _text.text = letter.ToString();
    }

    public void SetState(State newState)
    {
        CurrentState = newState;
        _fill.color = newState.fillColor;
        _outline.effectColor = newState.outlineColor;
    }
}
