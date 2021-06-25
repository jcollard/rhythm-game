using System;
using UnityEngine;

/// <summary>
/// A DoubleNoteFactory creates DoubleNotes. It expects the user to make two entries.
/// Each entry must be on a unique input type. The note is added at the cursor of
/// the BeatMap at the time of the second input.
/// </summary>
public class DoubleNoteFactory : NoteFactory
{

    private NoteInput first = NoteInput.Null;

    private void Start()
    {
        this.displayName = "Double";
        NoteFactory.registerFactory(typeof(DoubleNote), this);
    }

    override public void handleUserInput(NoteInput type, BeatMapper beatMapper)
    {

        Beat b = beatMapper.beatMap.getBeat();

        // Check to see if there is already a HoldNote here that needs
        // to be removed
        if (b != null)
        {
            Note toRemove = null;
            foreach (Note n in b.notes)
            {
                if (n is DoubleNote && (n.input == type || ((DoubleNote)n).input2 == type))
                {
                    toRemove = n;
                    break;
                }
            }
            if (toRemove != null)
            {
                beatMapper.beatMap.removeNote(toRemove);
                beatMapper.removeNoteController(toRemove, b);
                first = NoteInput.Null;
                return;
            }
        }

        if (first == NoteInput.Null)
        {
            first = type;
            //TODO place Double on selected spot
        }
        else if (first != type)
        {
            Note n = new DoubleNote(first, type);
            beatMapper.beatMap.addNote(n);
            first = NoteInput.Null;
        }

    }

    public override NoteController createNoteController(Note _n, Beat b, BeatMapper beatMapper)
    {
        DoubleNote n = (DoubleNote)_n;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(noteController);
        DoubleNoteController newNote = gameObject.GetComponent<DoubleNoteController>();

        newNote.model = new Tuple<Note, Beat>(n, b);
        newNote.gameObjectRef = gameObject;
        newNote.transform.parent = beatMapper.notes.transform;
        newNote.name = "" + n.input + " @" + b.position;
        newNote.startPosition = beatMapper.positions.CENTER.position;
        newNote.endPosition = beatMapper.noteToPosition[n.input];
        newNote.endPosition2 = beatMapper.noteToPosition[n.input2];

        newNote.startTime = ((b.position - BeatMap.BEAT * beatMapper.beatsVisible) * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));
        newNote.endTime = (b.position * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));
        newNote.beatMapper = beatMapper;

        return newNote;
    }

    override public void initialize()
    {
        first = NoteInput.Null;
    }

}