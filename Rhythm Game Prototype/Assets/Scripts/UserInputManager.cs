using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UserInputManager : MonoBehaviour
{

    public BeatMapper beatMapper;

    /// <summary>
    /// Map from NoteInputs to the cursor position when the note was first pressed
    /// </summary>
    public readonly Dictionary<NoteInput, long> inputs = new Dictionary<NoteInput, long>();

    private InputEventFactory inputEventFactory = InputEventFactory.GetInputEventFactory();

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
        }

        if (beatMapper.isPlaying)
        {
            foreach(NoteController toCheck in beatMapper.noteControllers.Values){
                if(toCheck.GetIsHit() != HitType.Null)
                {
                    continue;
                }
                toCheck.CheckHit(inputs);
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
        Controls c = Controls.Load();
        factories.Add("Up", new NoteEvent(c.Up, NoteInput.Up));
        factories.Add("Left", new NoteEvent(c.Left, NoteInput.Left));
        factories.Add("Down", new NoteEvent(c.Down, NoteInput.Down));
        factories.Add("Triangle", new NoteEvent(c.Triangle, NoteInput.Triangle));
        factories.Add("Circle", new NoteEvent(c.Circle, NoteInput.Circle));
        factories.Add("X", new NoteEvent(c.X, NoteInput.X));
        factories.Add("Scratch", new NoteEvent(c.Scratch, NoteInput.Scratch));

        factories.Add("Next Beat", new ChangeCursorEvent(c.NextBeat, BeatMap.BEAT));
        factories.Add("Next Half Beat", new ChangeCursorEvent(c.NextHalfBeat, BeatMap.HALF));
        factories.Add("Next Quarter Beat", new ChangeCursorEvent(c.NextQuarterBeat, BeatMap.QUARTER));
        factories.Add("Previous Beat", new ChangeCursorEvent(c.PrevBeat, -BeatMap.BEAT));
        factories.Add("Previous Half Beat", new ChangeCursorEvent(c.PrevHalfBeat, -BeatMap.HALF));
        factories.Add("Previous Quarter Beat", new ChangeCursorEvent(c.PrevQuarterBeat, -BeatMap.QUARTER));

        factories.Add("Play", new PlayEvent(c.Play));
        factories.Add("Pause", new PauseEvent(c.Pause));
        factories.Add("Stop", new StopEvent(c.Stop));

    }

}

[Serializable]
public class Controls
{
    public String Up = "f";
    public String Left = "d";
    public String Down = "s";
    public String Triangle = "j";
    public String Circle = "k";
    public String X = "l";
    public String Scratch = "space";
    public String NextBeat = "5";
    public String PrevBeat = "4";
    public String NextHalfBeat = "3";
    public String PrevHalfBeat = "2";
    public String NextQuarterBeat = "1";
    public String PrevQuarterBeat = "=";
    public String Play = "q";
    public String Pause = "w";
    public String Stop = "e";

    public static Controls Load()
    {
        if (!File.Exists("./inputs.json"))
        {
            Controls c = new Controls();
            File.WriteAllText("./inputs.json", JsonUtility.ToJson(c));
            return c;
        }
        try
        {
            String json = File.ReadAllText("./inputs.json");
            return JsonUtility.FromJson<Controls>(json);
        } catch { 
            return new Controls();
        }
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
