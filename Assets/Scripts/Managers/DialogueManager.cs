using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private Story currentStory;
    private Story[] usedStories = new Story[100];
    private int usedStoriesIndex = 0;

    [Header("choices UI")]
    [SerializeField] private GameObject[] choices;

    private TextMeshProUGUI[] choicesText;

    public bool dialogueIsPlaying { get; private set; } //this makes dialo1gueIsPlaying read only to outside scripts (public get, private set)

    private int choiceIndex; //this is used to determine our current choice

    private void Awake()
    {
        if (instance != null) //if we already have a game manager destroy this one
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        
        if (InputManager.GetInstance().GetSubmitPressed())
        {
            try
            {
                currentStory.ChooseChoiceIndex(choiceIndex);
            }
            catch
            {
                Debug.Log("something went wrong");
            }

            ContinueStory();
        }
        
    }

    public void EnterDialogueMode(TextAsset inkJSON, bool isNew)
    {
        if(isNew)//create and play the story
        {
            Debug.Log("is new");
            currentStory = new Story(inkJSON.text);

            Story tempStory = currentStory;
            usedStories[usedStoriesIndex] = tempStory;
        }
        else//find and play the story (create an array of stories, and search for this one)
        {
            Debug.Log("is not new");
            for (int i = 0; i<usedStories.Length; i++)
            {   
                if (usedStories[i] == new Story(inkJSON.text));
                {
                    Debug.Log("found it!");
                    currentStory = usedStories[i];
                    i += usedStories.Length;
                }
            }
        }
 
        //use a boolean that we set to true so this conversation cannot pop up again
        //(CHECK GETTING INK TO INTERACT WITH VARIABLES, we can probably set our variable in ink
        //we will also want to be able to switch to a new conversation after finishing one
        if (currentStory.canContinue)
        {
            dialogueIsPlaying = true;
            dialoguePanel.SetActive(true);

            currentStory.BindExternalFunction("endConversation", () =>
            {
                Debug.Log("EXTERNAL FUNCTION IS WORKING");
                ExitDialogueMode();
            });

            ContinueStory();
        }

        //WORKING VERSION MUSEUM
        /*
        currentStory = new Story(inkJSON.text);
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
        */

    }

    private void ExitDialogueMode()
    {
        currentStory.UnbindExternalFunction("endConversation");

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if(currentChoices.Count > choices.Length) //send error if we have more choices than we can hold
        {
            Debug.LogError("more choices given than available: " + currentChoices.Count);
        }

        //display our choices
        int index = 0;
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        //hide leftover choice slots
        for (int i = index; i< choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choice) //set a variable and call currentStory.choosechoiceIndex only when we're making our final selection
    {
        Debug.Log("choice has been made " + choice);
        choiceIndex = choice;
        //currentStory.ChooseChoiceIndex(choiceIndex);  (moved to update)
    }


}


