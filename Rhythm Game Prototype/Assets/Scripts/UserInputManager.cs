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

    private InputEventFactory inputEventFactory = InputEventFactory.GetInputEventFactory();
    private String pressed = null;


    // Update is called once per frame
    void Update()
    {

        foreach (UserInputEvent e in inputEventFactory.factories.Values)
        {
            if (Input.GetKeyDown(e.GetKeyCode()))
            {
                e.HandlePressedEvent(this);
            }

            if (Input.GetKeyUp(e.GetKeyCode()))
            {
                e.HandleReleasedEvent(this);
            }
            String newPressed = "Current Pressed (" + inputs.Keys.Count + "):\n";
            foreach (NoteInput input in inputs.Keys)
            {
                newPressed += " " + input + "@ " + inputs[input] + "\n";
            }
            if (newPressed != pressed)
            {
                print(newPressed);
                pressed = newPressed;
            }
        }

    }

}

public class InputEventFactory
{
    private static readonly InputEventFactory instance = new InputEventFactory();
    public static InputEventFactory GetInputEventFactory()
    {
        return instance;
    }

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

        factories.Add("Next Beat", new ChangeCursorEvent("5", BeatMap.BEAT));
        factories.Add("Next Half Beat", new ChangeCursorEvent("4", BeatMap.HALF));
        factories.Add("Next Quarter Beat", new ChangeCursorEvent("3", BeatMap.QUARTER));
        factories.Add("Previous Beat", new ChangeCursorEvent("=", -BeatMap.BEAT));
        factories.Add("Previous Half Beat", new ChangeCursorEvent("1", -BeatMap.HALF));
        factories.Add("Previous Quarter Beat", new ChangeCursorEvent("2", -BeatMap.QUARTER));

        factories.Add("Play", new PlayEvent("q"));
        factories.Add("Pause", new PauseEvent("w"));
        factories.Add("Stop", new StopEvent("e"));

    }

}

public interface UserInputEvent
{

    public String GetKeyCode();
    public void HandlePressedEvent(UserInputManager inputManager);
    public void HandleReleasedEvent(UserInputManager inputManager);

}

public abstract class BasicUserEvent : UserInputEvent
{

    private String keycode;

    protected BasicUserEvent(String keycode)
    {
        this.keycode = keycode;
    }

    public string GetKeyCode()
    {
        return this.keycode;
    }

    public abstract void HandlePressedEvent(UserInputManager inputManager);
    public abstract void HandleReleasedEvent(UserInputManager inputManager);
}

public class ChangeCursorEvent : BasicUserEvent
{
    private long amount;

    public ChangeCursorEvent(String keycode, long amount) : base(keycode)
    {
        this.amount = amount;
    }

    public override void HandlePressedEvent(UserInputManager inputManager)
    {
        if (amount > 0)
        {
            inputManager.beatMapper.beatMap.next(amount);
        }
        else
        {
            inputManager.beatMapper.beatMap.prev(-amount);
        }
    }

    public override void HandleReleasedEvent(UserInputManager inputManager)
    {
        // Do nothing
    }
}

public class PlayEvent : BasicUserEvent
{

    public PlayEvent(String keycode) : base(keycode) { }

    public override void HandlePressedEvent(UserInputManager inputManager)
    {
        inputManager.beatMapper.Play();
    }

    public override void HandleReleasedEvent(UserInputManager inputManager)
    {
        //Do nothing
    }
}


public class PauseEvent : BasicUserEvent
{

    public PauseEvent(String keycode) : base(keycode) { }

    public override void HandlePressedEvent(UserInputManager inputManager)
    {
        inputManager.beatMapper.Stop();
    }

    public override void HandleReleasedEvent(UserInputManager inputManager)
    {
        //Do nothing
    }
}

public class StopEvent : BasicUserEvent
{

    public StopEvent(String keycode) : base(keycode) { }

    public override void HandlePressedEvent(UserInputManager inputManager)
    {
        inputManager.beatMapper.Stop();
        inputManager.beatMapper.beatMap.setCursor(0, true);
    }

    public override void HandleReleasedEvent(UserInputManager inputManager)
    {
        //Do nothing
    }
}


public class NoteEvent : BasicUserEvent
{
    private NoteInput type;


    public NoteEvent(String keycode, NoteInput type) : base(keycode)
    {
        this.type = type;
    }

    public override void HandlePressedEvent(UserInputManager inputManager)
    {
        inputManager.inputs[type] = inputManager.beatMapper.beatMap.getCursor();
    }

    public override void HandleReleasedEvent(UserInputManager inputManager)
    {
        if (inputManager.inputs.ContainsKey(type))
        {
            inputManager.inputs.Remove(type);
        }
    }
}
