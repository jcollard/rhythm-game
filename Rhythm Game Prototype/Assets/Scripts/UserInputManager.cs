using System;
using System.Collections.Generic;
using UnityEngine;

public class UserInputManager : MonoBehaviour
{

    public BeatMapper beatMapper;

    /// <summary>
    /// Map from NoteInputs to the cursor position when the note was first pressed
    /// </summary>
    public readonly Dictionary<NoteInput, long> inputs = new Dictionary<NoteInput, long>();

    private InputEventFactory inputEventFactory = new InputEventFactory();
    private String pressed = null;
    

    // Update is called once per frame
    void Update()
    {
        
        foreach(UserInputEvent e in inputEventFactory.factories.Values){
            if (Input.GetKeyDown(e.GetKeyCode()))
            {
                e.HandlePressedEvent(this);
            }

            if (Input.GetKeyUp(e.GetKeyCode()))
            {
                e.HandleReleasedEvent(this);
            }
            String newPressed = "Current Pressed (" + inputs.Keys.Count + "):\n";
            foreach(NoteInput input in inputs.Keys)
            {
                newPressed += " " + input + "@ " + inputs[input] + "\n";
            }
            if(newPressed != pressed)
            {
                print(newPressed);
                pressed = newPressed;
            }
        }

    }

}

public class InputEventFactory
{

    /// <summary>
    /// A Dictionary from Axes to InputEventFactory
    /// </summary>
    public readonly Dictionary<String, UserInputEvent> factories = new Dictionary<String, UserInputEvent>();

    public InputEventFactory()
    {
        factories.Add("Up", new NoteEvent("d", NoteInput.Up));
        factories.Add("Left", new NoteEvent("s", NoteInput.Left));
        factories.Add("Down", new NoteEvent("a", NoteInput.Down));
        factories.Add("Triangle", new NoteEvent("j", NoteInput.Triangle));
        factories.Add("Circle", new NoteEvent("k", NoteInput.Circle));
        factories.Add("X", new NoteEvent("l", NoteInput.X));
        factories.Add("Scratch", new NoteEvent("space", NoteInput.Scratch));
    }

}

public interface UserInputEvent
{

    public String GetKeyCode(); 
    public void HandlePressedEvent(UserInputManager inputManager);
    public void HandleReleasedEvent(UserInputManager inputManager);

}

public class NoteEvent : UserInputEvent
{
    private NoteInput type;
    private String keycode;

    public NoteEvent(String keycode, NoteInput type)
    {
        this.type = type;
        this.keycode = keycode;
    }

    public String GetKeyCode()
    {
        return this.keycode;
    }

    public void HandlePressedEvent(UserInputManager inputManager)
    {
        inputManager.inputs[type] = inputManager.beatMapper.beatMap.getCursor();
    }

    public void HandleReleasedEvent(UserInputManager inputManager)
    {
        if (inputManager.inputs.ContainsKey(type))
        {
            inputManager.inputs.Remove(type);
        }
    }
}
