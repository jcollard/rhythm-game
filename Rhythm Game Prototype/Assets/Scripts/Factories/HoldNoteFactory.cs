using System;
using UnityEngine;

/// <summary>
/// A HoldNoteFactory creates HoldNotes. It expects the user to make two entries.
/// Both entries must be on the same input. The second entry must occur later
/// than the first entry.
/// </summary>
public class HoldNoteFactory : NoteFactory
{

    private NoteInput first = NoteInput.Null;
    private long start = -1;

    private void Start()
    {
        this.displayName = "Hold";
        NoteFactory.registerFactory(typeof(HoldNote), this);
    }

    override public NoteFactory addNote(NoteInput type, BeatMap beatMap)
    {
        if (first != type || start > beatMap.getCursor())
        {
            first = type;
            start = beatMap.getCursor();
            //TODO place Hold on selected spot
        }
        else
        {
            Note n = new HoldNote(type, beatMap.getCursor() - start);
            beatMap.setCursor(start);
            beatMap.addNote(n);
            first = NoteInput.Null;
            start = -1;
        }

        return this;
    }

    public override NoteController createNoteController(Note _n, Beat b, BeatMapper beatMapper)
    {
        HoldNote n = (HoldNote)_n;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(noteController);
        HoldNoteController newNote = gameObject.GetComponent<HoldNoteController>();

        newNote.model = new Tuple<Note, Beat>(n, b);
        newNote.gameObjectRef = gameObject;
        newNote.transform.parent = beatMapper.notes.transform;
        newNote.name = "Hold: " + n.input + " @" + b.position;
        newNote.startPosition = beatMapper.positions.CENTER.position;
        newNote.endPosition = beatMapper.noteToPosition[n.input];

        newNote.startTime = ((b.position - BeatMap.BEAT * beatMapper.beatsVisible) * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));
        newNote.endTime = (b.position * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));

        newNote.secondStartTime = ((b.position + n.duration - BeatMap.BEAT * beatMapper.beatsVisible) * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));
        newNote.secondEndTime = ((b.position + n.duration) * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));
        newNote.beatMapper = beatMapper;

        return newNote;
    }

    override public void initialize()
    {
        first = NoteInput.Null;
    }

}