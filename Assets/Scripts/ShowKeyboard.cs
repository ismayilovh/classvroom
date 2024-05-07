using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class ShowKeyboard : MonoBehaviour
{

    private TMP_InputField _inputField;
    void Start()
    {
        _inputField = GetComponent<TMP_InputField>();
        _inputField.onSelect.AddListener(x => OpenKeyboard());
    }

    void OpenKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = _inputField;    
        NonNativeKeyboard.Instance.PresentKeyboard(_inputField.text);
    }
}
