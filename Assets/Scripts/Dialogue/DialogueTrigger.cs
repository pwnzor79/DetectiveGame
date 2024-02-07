using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("icon showing interaction potential")]
    [SerializeField] private GameObject visualCue;

    [SerializeField] public TextAsset inkJSON;

    private bool playerInRange;

    private bool isNew = true; //tells the dialogueManager if this is a conversation it has seen before

    private void Awake()
    {
        visualCue.SetActive(false);
        playerInRange = false;
    }

    private void Update()
    {
        
        if (InputManager.GetInstance().GetInteractPressed() && playerInRange && !DialogueManager.instance.dialogueIsPlaying)
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, isNew);
            isNew = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerInRange = true;
            visualCue.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = false;
            visualCue.SetActive(false);
        }
    }

    public TextAsset GetInkJSON()
    {
        return inkJSON;
    }

}
