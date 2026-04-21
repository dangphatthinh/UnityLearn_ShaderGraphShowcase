using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    private UIDocument m_UIDoc;
    private Label m_InfoElement;
    
    private void OnEnable()
    {
        m_UIDoc = GetComponent<UIDocument>();
        m_InfoElement = m_UIDoc.rootVisualElement.Q<Label>("InteractText");
        
        HideText();
    }

    public void SetText(string text)
    {
        m_InfoElement.style.display = DisplayStyle.Flex;
        m_InfoElement.text = text;
    }

    public void HideText()
    {
        m_InfoElement.style.display = DisplayStyle.None;
    }
}