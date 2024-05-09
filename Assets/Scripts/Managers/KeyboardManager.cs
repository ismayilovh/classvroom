using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;

public class KeyboardManager : MonoBehaviour
{
    [SerializeField] private float distance = 0.5f;
    [SerializeField] private float verticalOffset = -0.5f;

    [SerializeField] private Transform positionSource;

    public void ShowKeyboard(TMP_InputField inputField)
    {
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);
        NonNativeKeyboard.Instance.InputField = inputField;

        Vector3 direction = positionSource.forward;
        direction.y = 0;
        direction.Normalize();

        Vector3 targetPosition = positionSource.position + direction * distance + Vector3.up * verticalOffset;
        NonNativeKeyboard.Instance.RepositionKeyboard(targetPosition);
    }
}
