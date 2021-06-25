using UnityEngine;

public class HoldNoteFactory : NoteFactory
{

    private NoteInput first = NoteInput.Null;
    private long start = -1;

    private void Start()
    {
        this.displayName = "Hold";
        NoteFactory.holdNoteFactory = this;
    }

    override public NoteFactory addNote(NoteInput type, BeatMap beatMap)
    {
        if (first != type)
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

    public override NoteController drawNote(Note _n, Beat b, BeatMapper beatMapper)
    {
        HoldNote n = (HoldNote)_n;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(noteController);
        HoldNoteController newNote = gameObject.GetComponent<HoldNoteController>();

        newNote.model = n;
        newNote.gameObjectRef = gameObject;
        newNote.transform.parent = beatMapper.notes.transform;
        newNote.name = "" + n.input + " @" + b.position;
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