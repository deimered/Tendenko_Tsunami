using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Cinemachine.CinemachineBlendDefinition;

public class UIHelpControler : MonoBehaviour
{
    private VisualElement root;
    private Button helpButton;

    //[TextArea(1, 4)]
    //public string text;

    //public string[] container_names;

    private List<VisualElement> helpContainers;
    private Vector3[] translationInformation;

    private bool isShowing = true;
    private bool helpLoaded = false;

    // Start is called before the first frame update
    void Awake()
    {
        helpContainers = new List<VisualElement>();
        root = GetComponent<UIDocument>().rootVisualElement;

        foreach (VisualElement container in root.Children())
        {
            if (container.name != "help_button_container")
                helpContainers.Add(container);
        }

        translationInformation = new Vector3[helpContainers.Count];

        helpButton = root.Q<Button>("help_button");

        helpButton.clicked += ShowHelp;

        if(helpContainers.Count > 0)
            StartCoroutine(UILoaded());
    }

    void ShowHelp()
    {
        if (helpLoaded)
        {
            for (int i = 0; i < helpContainers.Count; i++)
            {
                helpContainers[i].style.translate = !isShowing ? new StyleTranslate(new Translate(0, 0, 0)) :
                new StyleTranslate(new Translate(translationInformation[i].x, translationInformation[i].y, translationInformation[i].z));
            }
            //informationBox.style.translate = !isShowing ? new StyleTranslate(new Translate(0, 0, 0)) :
            //new StyleTranslate(new Translate(0, informationBox.resolvedStyle.height * (-2), 0));

            isShowing = !isShowing;

            if (isShowing)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
            }
            else
            {
                Time.timeScale = 1;
                AudioListener.pause = false;
            }
        }
    }

    IEnumerator UILoaded()
    {
        while (helpContainers[0].resolvedStyle.height.ToString() == "NaN" || helpContainers[0].resolvedStyle.height.ToString() == "0")
        {
            yield return null;
        }

        for(int i = 0; i < helpContainers.Count; i++)
            translationInformation[i] = helpContainers[i].resolvedStyle.translate;
        
        helpLoaded = true;
        ShowHelp();
    }

    public void HideUI()
    {
        if (isShowing)
            ShowHelp();
        //helpButton.style.display = DisplayStyle.None;
        //informationBox.style.display = DisplayStyle.None;
        root.style.display = DisplayStyle.None;
    }

    public void ShowUI()
    {
        root.style.display = DisplayStyle.Flex;
        //helpButton.style.display = DisplayStyle.Flex;
        //informationBox.style.display = DisplayStyle.Flex;
    }
}
