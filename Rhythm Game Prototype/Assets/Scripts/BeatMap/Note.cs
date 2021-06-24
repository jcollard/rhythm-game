using System;
public class Note
{
    public static readonly Note UP = new Note(NoteInput.Up);
    public static readonly Note LEFT = new Note(NoteInput.Up);
    public static readonly Note DOWN = new Note(NoteInput.Up);
    public static readonly Note TRIANGLE = new Note(NoteInput.Triangle);
    public static readonly Note CIRCLE = new Note(NoteInput.Circle);
    public static readonly Note X = new Note(NoteInput.X);
    public NoteInput input;

    public Note(NoteInput input)
    {
        this.input = input;
    }

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