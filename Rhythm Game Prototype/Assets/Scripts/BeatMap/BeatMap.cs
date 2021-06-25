using System;
using System.Collections.Generic;

/// <summary>
/// A BeatMap can be thought of as a sparsely populated array of Beats at given
/// positions in a song. Each beat can be subdivided into BeatMap.BEAT
/// pieces which can then be processed based on the BeatMap's BPM (beats per minute).
/// <br/>
/// A Cursor tracks the current position within the BeatMap which can be used to add
/// or remove Beats as well as to see what beats are present at that location.
/// <br/>
/// A list of all of the Beats for this BeatMap can be obtained using the BeatMap.getBeats()
/// method which returns a sorted list.
/// </summary>
public class BeatMap
{
    /// <summary>
    /// The resolution of a full beat
    /// </summary>
    public static readonly long BEAT = 1000;

    /// <summary>
    /// The resolution of a half beat (half as long as a BEAT)
    /// </summary>
    public static readonly long HALF = BEAT / 2;

    /// <summary>
    /// The resolution of a quarter beat (1/4 as long as a BEAT)
    /// </summary>
    public static readonly long QUARTER = BEAT / 4;

    /// <summary>
    /// The beats per minute of the BeatMap
    /// </summary>
    private int bpm;

    /// <summary>
    /// The current position of the cursor
    /// </summary>
    private long cursor;

    /// <summary>
    /// A Dictionary mapping cursor indices to Beats
    /// </summary>
    private readonly SortedList<long, Beat> beats = new SortedList<long, Beat>();
    //TODO Consider using sorted list

    /// <summary>
    /// A set of observers to notify when the BeatMap is modified
    /// </summary>
    private HashSet<Observer> observers = new HashSet<Observer>();

    /// <summary>
    /// Notifies all of the observers that a change has been made.
    /// </summary>
    public void notify()
    {
        foreach (Observer o in observers)
        {
            o.doUpdate();
        }
    }

    /// <summary>
    /// Get the current position of the cursor
    /// </summary>
    /// <returns>The current position of the cursor</returns>
    public long getCursor()
    {
        return cursor;
    }

    /// <summary>
    /// Sets the position of the cursor. If a value less than 0 is specified,
    /// the cursor is set to 0.
    /// If the cursor was updated, all observers are notified.
    /// </summary>
    /// <param name="cursor">The new position of the cursor</param>
    public void setCursor(long cursor)
    {
        if (this.cursor == cursor)
        {
            return;
        }

        if (cursor < 0)
        {
            this.cursor = 0;
        }
        else
        {
            this.cursor = cursor;
        }
        notify();
    }

    /// <summary>
    /// Sets the BPM for this BeatMap clamped at 0. If the value was changed, all observers are notified.
    /// </summary>
    /// <param name="bpm">The new bpm</param>
    public void setBPM(int bpm)
    {
        if(bpm == this.bpm)
        {
            return;
        }

        if(bpm < 0)
        {
            this.bpm = 0;
        }

        this.bpm = bpm;
        notify();
    }

    /// <summary>
    /// Returns the current BPM of this BeatMap.
    /// </summary>
    /// <returns>The current BPM of this BeatMap</returns>
    public int getBPM()
    {
        return bpm;
    }

    /// <summary>
    /// Moves to the nearest cursor position that is a subdivision of the
    /// specified duration that is immediately before the current cursor
    /// </summary>
    /// <param name="duration">The duration to jump to</param>
    private void prev(long duration)
    {

        long diff = cursor % duration;
        if (diff != 0)
        {
            cursor -= diff;
        }
        else
        {
            cursor -= duration;
        }

        if (cursor < 0)
        {
            cursor = 0;
        }
        notify();
    }

    /// <summary>
    /// Moves the cursor to the next closest subdivision of the specified duration.
    /// </summary>
    /// <param name="duration">The duration to jump</param>
    private void next(long duration)
    {
        long diff = cursor % duration;
        if (diff != 0)
        {
            cursor += (duration - diff);
        }
        else
        {
            cursor += duration;
        }
        notify();
    }

    /// <summary>
    /// Moves the cursor to the next closest BEAT
    /// </summary>
    public void nextBeat()
    {
        next(BEAT);
    }

    /// <summary>
    /// Moves the cursor to the next closest HALF_BEAT
    /// </summary>
    public void nextHalf()
    {
        next(HALF);
    }

    /// <summary>
    /// Moves the cursor to the next closes QUARTER
    /// </summary>
    public void nextQuarter()
    {
        next(QUARTER);
    }

    /// <summary>
    /// Moves the cursor to the previous closest BEAT
    /// </summary>
    public void prevBeat()
    {
        prev(BEAT);
    }

    /// <summary>
    /// Moves the cursor to the previous closest HALF
    /// </summary>
    public void prevHalf()
    {
        prev(HALF);
    }

    /// <summary>
    /// Moves the cursor to the previous closest QUARTER
    /// </summary>
    public void prevQuarter()
    {
        prev(QUARTER);
    }

    /// <summary>
    /// Gets the Beat associated with the current cursor index.
    /// </summary>
    /// <returns>The Beat at this index or null if there is no Beat</returns>
    public Beat getBeat()
    {
        return getBeat(cursor);
    }


    /// <summary>
    /// Gest the Beat associated with the specified cursor index
    /// </summary>
    /// <param name="cursor"></param>
    /// <returns>The beat at the specified index or null if there is no Beat at that index</returns>
    public Beat getBeat(long cursor)
    {
        if (beats.ContainsKey(cursor))
        {
            return beats[cursor];
        }
        return null;
    }

    /// <summary>
    /// Returns a sorted List containing all of the Beats in this BeatMap.
    /// </summary>
    /// <returns>A sorted List containing all of the Beats in this BeatMap</returns>
    public List<Beat> getBeats()
    {
        List<Beat> bs = new List<Beat>(beats.Values);
        return bs;
    }

    /// <summary>
    /// Adds a note at the current cursor. If a note was added, notifies all
    /// observers.
    /// </summary>
    /// <param name="toAdd">The note to be added</param>
    /// <returns>The Beat containing the note that was added</returns>
    public Beat addNote(Note toAdd)
    {
        return addNote(toAdd, cursor);
    }

    /// <summary>
    /// Adds a note to the specified cursor position. If a note was added, notifies
    /// all observers.
    /// </summary>
    /// <param name="toAdd">The note to be added</param>
    /// <param name="cursor">The cursor position to add the note</param>
    /// <returns>The Beat containing the note that was added</returns>
    public Beat addNote(Note toAdd, long cursor)
    {
        //TODO: Don't add duplicate notes
        Beat beat = null;
        if (beats.ContainsKey(cursor))
        {
            beat = beats[cursor];
        }
        else
        {
            beat = new Beat(cursor);
        }
        beat.notes.Add(toAdd);
        beats[cursor] = beat;
        notify();
        return beat;
    }

    /// <summary>
    /// Remove the Beat at the specified cursor position. If a Beat was removed,
    /// all observers are notified.
    /// </summary>
    /// <param name="cursor">The cursor</param>
    /// <returns></returns>
    public bool removeBeat(long cursor)
    {
        bool rv = beats.Remove(cursor);
        if (rv)
        {
            notify();
        }
        return rv;
    }


    /// <summary>
    /// Create a blank BeatMap with the specified BPM. If the specified BPM is
    /// below 0, the BPM is set to 0.
    /// </summary>
    /// <param name="bpm">The BPM</param>
    public BeatMap(int bpm)
    {
        setBPM(bpm);
    }

    /// <summary>
    /// Register an Observer to be notified when the BeatMap changes.
    /// </summary>
    /// <param name="o">The Observer to register</param>
    public void registerObserver(Observer o)
    {
        this.observers.Add(o);
    }

}

/// <summary>
/// A Beat contains a position and a set of Notes.
/// A Beat can be sorted by its position
/// </summary>
public class Beat : IComparable<Beat>
{
    /// <summary>
    /// The Position of this Beat within a BeatMap
    /// </summary>
    public readonly long position;

    /// <summary>
    /// A Set of Notes at this Beat
    /// </summary>
    public readonly HashSet<Note> notes = new HashSet<Note>();

    /// <summary>
    /// Creates a Beat at the specified Position
    /// </summary>
    /// <param name="position">The position of this Beat within a BeatMap</param>
    public Beat(long position)
    {
        this.position = position;
    }

    /// <summary>
    /// A Beat is ordered by its position
    /// </summary>
    /// <param name="other">The other beat to compare with this</param>
    /// <returns>-1 if this Beat preceeds the other Beat, 0 if they are equal,
    /// and 1 if the other Beat preceeds this Beat</returns>
    public int CompareTo(Beat other)
    {
        if (this.position < other.position)
        {
            return -1;
        }

        if (this.position > other.position)
        {
            return 1;
        }

        return 0;
    }
}