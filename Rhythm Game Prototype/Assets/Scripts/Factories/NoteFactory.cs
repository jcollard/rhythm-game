using System;
using UnityEngine;

public class NoteFactory : MonoBehaviour
{

    public String displayName = "Normal";
    public GameObject noteController;

    public static NoteFactory noteFactory;
    public static HoldNoteFactory holdNoteFactory;

    private void Start()
    {
        NoteFactory.noteFactory = this;
    }

    public static NoteController getNoteController(Note n, Beat b, BeatMapper beatMapper)
    {
        if (n is HoldNote)
        {
            return holdNoteFactory.drawNote(n, b, beatMapper);
        }
        return noteFactory.drawNote(n,b,beatMapper);
    }

    public virtual NoteFactory addNote(NoteInput type, BeatMap beatMap)
    {
        Note n = new Note(type);
        beatMap.addNote(n);
        return this;
    }

    public virtual NoteController drawNote(Note n, Beat b, BeatMapper beatMapper)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(noteController);
        NoteController newNote = gameObject.GetComponent<NoteController>();

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

    public virtual void initialize()
    {
        
    }

}
