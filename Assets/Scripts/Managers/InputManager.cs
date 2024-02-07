using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//this was used as a reference
//https://github.com/trevermock/ink-dialogue-system/blob/8-ink-external-functions-example/Assets/Scripts/Input/InputManager.cs
//https://www.youtube.com/watch?v=vY0Sk93YUhA&list=PL3viUl9h9k78KsDxXoAzgQ1yRjhm7p8kl&index=2


public class InputManager : MonoBehaviour
{

    public static InputManager instance;

    private Vector2 moveDir = Vector2.zero;
    private bool interactPressed;
    private bool submitPressed;

    public PlayerActionsScript playerActionsScript;
    private InputAction move;
    private InputAction fire;
    private InputAction interact;
    private InputAction submit;

    [SerializeField] Rigidbody2D playerRigidBody;

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

        playerActionsScript = new PlayerActionsScript();
    }

    private void OnEnable()
    {
        move = playerActionsScript.Player.Move;
        move.Enable();

        fire = playerActionsScript.Player.Fire;
        fire.Enable();

        interact = playerActionsScript.Player.Interact;
        interact.Enable();
        interact.performed += Interact;
        interact.performed += InteractButtonPressed;

        submit = playerActionsScript.Player.Submit;
        submit.Enable();
        submit.performed += SubmitButtonPressed;
    }
    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
        interact.Disable();
    }

    private void Update()
    {
        moveDir = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if(DialogueManager.instance.dialogueIsPlaying) //cancels movement if dialogue is playing
        {
            playerRigidBody.velocity = Vector2.zero;
            return;
        }
        playerRigidBody.velocity = new Vector2(moveDir.x * PlayerManager.instance.moveSpeed, 0);
    }


    public static InputManager GetInstance()
    {
        return instance;
    }


    public void InteractButtonPressed(InputAction.CallbackContext context) //this might need to be named InteractPressed
    {
        if (context.performed)
        {
            interactPressed = true;
        }
        else if (context.canceled)
        {
            interactPressed = false;
        }
    }

    public void SubmitButtonPressed(InputAction.CallbackContext context) //this might need to be named InteractPressed
    {
        if (context.performed && DialogueManager.instance.dialogueIsPlaying)
        {
            Debug.Log("submit has been pressed");
            submitPressed = true; //this can be called in the dialouge manaager regardless of if we are interacting
            Debug.Log("submit is" + submitPressed);
        }
        else if (context.canceled)
        {
            Debug.Log("submit has been cancelled");
            submitPressed = false;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interact pressed");
    }


    public Vector2 GetMoveDirection()
    {
        return moveDir;
    }


    public bool GetInteractPressed() //checks if interact has been pressed, and sets it to false so we don't get multiple readings
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    public bool GetSubmitPressed() //checks if interact has been pressed, and sets it to false so we don't get multiple readings
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }
}
