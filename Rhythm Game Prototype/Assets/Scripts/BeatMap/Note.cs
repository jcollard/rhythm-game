using System;
using System.Collections.Generic;

/// <summary>
/// A Note represents 
/// </summary>
public class Note
{
    
    public string type = "Normal";
    public NoteInput input;

    public Note(NoteInput input)
    {
        this.input = input;
    }
}

public class ScratchNote : Note
{
    public ScratchNote() : base(NoteInput.Null) { type = "Scratch"; }
}

public class DoubleNote : Note
{
    public readonly NoteInput input2;

    public DoubleNote(NoteInput first, NoteInput second): base(first)
    {
        this.input2 = second;
        this.type = "Double";
    }
}

public class HoldNote : Note
{
    public readonly float duration;

    public HoldNote(NoteInput input, float duration): base(input)
    {
        this.duration = duration;
        this.type = "Hold";
    }
}