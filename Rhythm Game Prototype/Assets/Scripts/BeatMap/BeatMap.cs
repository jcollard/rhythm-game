using System;
using System.Collections.Generic;

public class BeatMap
{
    public static readonly long BEAT    = 1000;
    public static readonly long HALF    = 500;
    public static readonly long QUARTER = 250;

    public int bpm;
    // Uses 1000 to represent 1 beat, 500 to represent 8th note, 250 to represent 16th note, etc.
    private long cursor;
    private readonly Dictionary<long, Beat> beats = new Dictionary<long, Beat>();
    private HashSet<Observer> observers = new HashSet<Observer>();

    public void notify()
    {
        foreach(Observer o in observers)
        {
            o.doUpdate();
        }
    }

    public long getCursor()
    {
        return cursor;
    }

    public void setCursor(long cursor)
    {
        if(this.cursor == cursor)
        {
            return;
        }
        this.cursor = cursor;
        notify();
    }

    public void setBPM(int bpm)
    {
        this.bpm = bpm;
        notify();
    }

    public int getBPM()
    {
        return bpm;
    }

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

    public void nextBeat()
    {
        next(BEAT);
    }

    public void nextHalf()
    {
        next(HALF);
    }

    public void nextQuarter()
    {
        next(QUARTER);
    }

    public void prevBeat()
    {
        prev(BEAT);
    }

    public void prevHalf()
    {
        prev(HALF);
    }

    public void prevQuarter()
    {
        prev(QUARTER);
    }
    

    public Beat getBeat()
    {
        return getBeat(cursor);
    }

    public Beat getBeat(long cursor)
    {
        if (beats.ContainsKey(cursor)){
            return beats[cursor];
        }
        return null;
    }

    public List<Beat> getBeats()
    {
        List<Beat> bs = new List<Beat>(beats.Values);
        bs.Sort();
        return bs;
    }

    public Beat addNote(Note toAdd)
    {

        return addNote(toAdd, cursor);
    }

    public bool removeBeat(long cursor)
    {
        bool rv = beats.Remove(cursor);
        if (rv)
        {
            notify();
        }
        return rv;
    }

    public Beat addNote(Note toAdd, long cursor)
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
        beat.notes.Add(toAdd);
        beats[cursor] = beat;
        notify();
        return beat;
    }

    public BeatMap(int bpm)
    {
        this.bpm = bpm;
    }

    public void registerObserver(Observer o)
    {
        this.observers.Add(o);
    }

}

public class Beat : IComparable<Beat>
{
    public readonly long position;
    public readonly HashSet<Note> notes = new HashSet<Note>();

    public Beat(long position)
    {
        this.position = position;
    }

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