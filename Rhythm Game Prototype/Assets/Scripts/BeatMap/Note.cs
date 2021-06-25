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

    public override bool Equals(object obj)
    {
        return obj is Note note &&
               type == note.type &&
               input == note.input;
    }

    public override int GetHashCode()
    {
        int hashCode = -68311600;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
        hashCode = hashCode * -1521134295 + input.GetHashCode();
        return hashCode;
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

    public override bool Equals(object obj)
    {
        return obj is DoubleNote note &&
               base.Equals(obj) &&
               type == note.type &&
               input == note.input &&
               input2 == note.input2;
    }

    public override int GetHashCode()
    {
        int hashCode = 251041073;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
        hashCode = hashCode * -1521134295 + input.GetHashCode();
        hashCode = hashCode * -1521134295 + input2.GetHashCode();
        return hashCode;
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

    public override bool Equals(object obj)
    {
        return obj is HoldNote note &&
               base.Equals(obj) &&
               type == note.type &&
               input == note.input &&
               duration == note.duration;
    }

    public override int GetHashCode()
    {
        int hashCode = -1512021421;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
        hashCode = hashCode * -1521134295 + input.GetHashCode();
        hashCode = hashCode * -1521134295 + duration.GetHashCode();
        return hashCode;
    }
}