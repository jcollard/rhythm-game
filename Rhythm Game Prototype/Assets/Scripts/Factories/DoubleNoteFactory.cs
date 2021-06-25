using UnityEngine;

public class DoubleNoteFactory : NoteFactory
{

    private NoteInput first = NoteInput.Null;

    private void Start()
    {
        this.displayName = "Double";
        NoteFactory.doubleNoteFactory = this;
    }

    override public NoteFactory addNote(NoteInput type, BeatMap beatMap)
    {
        if (first == NoteInput.Null)
        {
            first = type;
            //TODO place Double on selected spot
        }
        else if (first != type)
        {
            Note n = new DoubleNote(first, type);
            beatMap.addNote(n);
            first = NoteInput.Null;
        }

        return this;
    }

    public override NoteController drawNote(Note n, Beat b, BeatMapper beatMapper)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(noteController);
        DoubleNoteController newNote = gameObject.GetComponent<DoubleNoteController>();

        newNote.model = n;
        newNote.gameObjectRef = gameObject;
        newNote.transform.parent = beatMapper.notes.transform;
        newNote.name = "" + n.input + " @" + b.position;
        newNote.startPosition = beatMapper.positions.CENTER.position;
        newNote.endPosition = beatMapper.noteToPosition[n.input];

        newNote.startTime = ((b.position - BeatMap.BEAT * beatMapper.beatsVisible) * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.bpm));
        newNote.endTime = (b.position * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.bpm));
        newNote.beatMapper = beatMapper;

        return newNote;
    }

    override public void initialize()
    {
        first = NoteInput.Null;
    }

}