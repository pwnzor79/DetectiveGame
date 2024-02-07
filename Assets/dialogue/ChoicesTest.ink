EXTERNAL endConversation()

-> main
VAR isDone = "false"

===main===
mic check mic check {isDone}
    *[we've got tarnation]
        -> chosen("1")
    *[the fast and the few in this coronation]
        ->chosen("2")
    *[politricks who rabbit hole down the throne]
        ->chosen("3")
    + -> 
        We have nothing else to discuss
        ~ endConversation()
        .
        ->main
        
== chosen(response)
{response}
~isDone = true

//call dialogueManager.exitDialogueMode
~ endConversation()
. // this is crucial to the next line not being skipped over
->main

->END