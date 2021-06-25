using UnityEngine;

public class ScratchNoteFactory : NoteFactory
{

    private void Start()
    {
        this.displayName = "Scratch";
        NoteFactory.scratchNoteFactory = this;
    }

    override public NoteFactory addNote(NoteInput type, BeatMap beatMap)
    {

        Note n = new ScratchNote();
        beatMap.addNote(n);

        return this;
    }

    public override NoteController drawNote(Note _n, Beat b, BeatMapper beatMapper)
    {
        ScratchNote n = (ScratchNote)_n;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(noteController);
        ScratchNoteController newNote = gameObject.GetComponent<ScratchNoteController>();

        newNote.model = n;
        newNote.gameObjectRef = gameObject;
        newNote.transform.parent = beatMapper.notes.transform;
        newNote.name = "Scratch @" + b.position;
        newNote.startPosition = beatMapper.positions.CENTER.position;

        newNote.startTime = ((b.position - BeatMap.BEAT * beatMapper.beatsVisible) * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.bpm));
        newNote.endTime = (b.position * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.bpm));

        newNote.beatMapper = beatMapper;

        return newNote;
    }

}