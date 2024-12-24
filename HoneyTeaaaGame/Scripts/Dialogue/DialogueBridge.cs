using Godot;
using System;

//A dialogue bridge I can use in case I decide to swap out the dialogue system later.
public partial class DialogueBridge : Node
{
	[Export] public Control dialogueBox;
    public static DialogueBridge Instance;
    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }
    public void SwapDialogueBoxData(String pathToNewData) {
		dialogueBox.Set("data", GD.Load<Resource>(pathToNewData));
	}
	public void StartDialogueID(String dialogueID) {
		dialogueBox.Call("start", dialogueID);
	}
    public void SetVariable(String varName, int type, dynamic value, int op = 0) => dialogueBox.Call("set_variable", varName, type, value, op);
}
