using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIPopUpWindowControler : MonoBehaviour, IUIControler
{
    private VisualElement root;

    private Button confirmButton;
    private Button cancelButton;

    [TextArea(1, 4)]
    [SerializeField]
    private string text;
    [SerializeField]
    private string confirmButtonText;
    [SerializeField]
    private string cancelButtonText;

    public event EventHandler OnPause;
    public event EventHandler OnConfirm;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        confirmButton = root.Q<Button>("confirm_button");
        cancelButton = root.Q<Button>("cancel_button");
        Label popUpText = root.Q<Label>("pop_up_text");

        confirmButton.clicked += ConfirmButton;
        cancelButton.clicked += CancelButton;

        popUpText.text = text;
        confirmButton.text = confirmButtonText;
        cancelButton.text = cancelButtonText;

        Deactivate();
    }

    private void ConfirmButton()
    {
        OnConfirm?.Invoke(this, EventArgs.Empty);
    }

    private void CancelButton()
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    public void Activate()
    {
        Time.timeScale = 0;
        root.style.display = DisplayStyle.Flex;
    }

    public void Deactivate()
    {
        Time.timeScale = 1;
        root.style.display = DisplayStyle.None;
    }
}
