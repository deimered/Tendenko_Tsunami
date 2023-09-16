using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIControler : MonoBehaviour
{
    public Button startButton;
    public Button messageButton;
    public Label messageText;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("firstButton");
        messageButton = root.Q<Button>("button_2");
        messageText = root.Q<Label>("hidden_text");

        startButton.clicked += StartButtonPress;
        messageButton.clicked += StartMessagePress;
    }

    void StartButtonPress()
    {
        messageText.text = "Omae wa mo shindeiru";
        messageText.style.display = DisplayStyle.Flex;
    }

    void StartMessagePress()
    {
        messageText.text = "Nani";
        messageText.style.display = DisplayStyle.Flex;
    }
}
