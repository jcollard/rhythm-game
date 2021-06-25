using System;
public class Note
{

    public NoteInput input;

    public Note(NoteInput input)
    {
        this.input = input;
    }

}

public class ScratchNote : Note
{
    public ScratchNote() : base(NoteInput.Null) { }
}

public class DoubleNote : Note
{
    public readonly NoteInput input2;

    public DoubleNote(NoteInput first, NoteInput second): base(first)
    {
        this.input2 = second;
    }
}

public class HoldNote : Note
{
    public readonly float duration;

    public HoldNote(NoteInput input, float duration): base(input)
    {
        this.duration = duration;
    }
}