using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
[Serializable]
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
    /// The name of this BeatMap track
    /// </summary>
    public String name = "Untitled";

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
    [NonSerialized]
    private HashSet<Observer> observers = new HashSet<Observer>();

    /// <summary>
    /// The path to the Track to be played with this BeatMap.
    /// </summary>
    public String pathToTrack;

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
    public void setCursor(long cursor, bool doNotify = false)
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
        if (doNotify)
        {
            notify();
        }
    }

    /// <summary>
    /// Sets the BPM for this BeatMap clamped at 0. If the value was changed, all observers are notified.
    /// </summary>
    /// <param name="bpm">The new bpm</param>
    public void setBPM(int bpm)
    {
        if (bpm == this.bpm)
        {
            return;
        }

        if (bpm < 0)
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
    public bool addNote(Note toAdd)
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
    public bool addNote(Note toAdd, long cursor)
    {
        Beat beat = null;
        if (beats.ContainsKey(cursor))
        {
            beat = beats[cursor];
        }
        else
        {
            beat = new Beat(cursor);
        }
        if (beat.notes.Contains(toAdd))
        {
            return false;
        }
        beat.notes.Add(toAdd);
        beats[cursor] = beat;
        notify();
        return true;
    }

    /// <summary>
    /// Removes the specified Note at the current cursor, if there exists an
    /// Equal Note at the cursors Beat.
    /// </summary>
    /// <param name="toRemove">The Note to remove</param>
    /// <returns>True if the BeatMap was modified otherwise False</returns>
    public bool removeNote(Note toRemove)
    {
        return removeNote(toRemove, cursor);
    }

    /// <summary>
    /// Removes the specified Note at the specified cursor, if there exists an
    /// Equal Note at the cursors Beat.
    /// </summary>
    /// <param name="toRemove">The Note to remove</param>
    /// <param name="cursor">The cursor position to check</param>
    /// <returns>True if the BeatMap was modified otherwise False</returns>
    public bool removeNote(Note toRemove, long cursor)
    {
        Beat beat = null;
        if (beats.ContainsKey(cursor))
        {
            beat = beats[cursor];
        }
        else
        {
            return false;
        }
        if (!beat.notes.Contains(toRemove))
        {
            return false;
        }
        beat.notes.Remove(toRemove);
        if (beat.notes.Count == 0)
        {
            beats.Remove(cursor);
        }
        notify();
        return true;
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

    /// <summary>
    /// Given a number of seconds, convert to the nearest cursor position
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>The nearest cursor position</returns>
    public long SecondsToCursorPosition(float seconds)
    {
        long position = (long)((bpm * BeatMap.BEAT * seconds) / 60);
        // Handle floating point rounding error
        if (position % 2 == 1)
        {
            position += 1;
        }
        return position;
    }

    /// <summary>
    /// Given a cursor position, convert it to seconds
    /// </summary>
    /// <param name="position">The cursor position</param>
    /// <returns>The time in seconds</returns>
    public float CursorPositionToSeconds(long position)
    {
        return (position * 60) / ((float)(BeatMap.BEAT * bpm)); ;
    }

    public void Serialize(String path)
    {
        Stream s = File.Open(path, FileMode.Create);
        BinaryFormatter b = new BinaryFormatter();
        b.Serialize(s, this);
        s.Close();
    }

    public static BeatMap Deserialize(String path)
    {
        Stream s = File.Open(path, FileMode.Open);
        BinaryFormatter b = new BinaryFormatter();
        BeatMap beatMap = (BeatMap)b.Deserialize(s);
        beatMap.observers = new HashSet<Observer>();
        beatMap.setCursor(0);
        s.Close();
        return beatMap;
    }

}

/// <summary>
/// A Beat contains a position and a set of Notes.
/// A Beat can be sorted by its position
/// </summary>
[Serializable]
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