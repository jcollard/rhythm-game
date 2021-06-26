using System;
using UnityEngine;

/// <summary>
/// A ScratchNoteFactory creates ScratchNotes. The user may click on any input
/// to add a ScratchNote.
/// </summary>
public class ScratchNoteFactory : NoteFactory
{

    private void Start()
    {
        this.displayName = "Scratch";
        NoteFactory.registerFactory(typeof(ScratchNote), this);
    }

    override public void handleUserInput(NoteInput type, BeatMapper beatMapper)
    {

        Note n = new ScratchNote();
        Beat b = beatMapper.beatMap.getBeat();
        if (b != null && b.notes.Contains(n))
        {
            beatMapper.beatMap.removeNote(n);
            beatMapper.removeNoteController(n, b);
        }
        else
        {
            beatMapper.beatMap.addNote(n);
        }

    }

    public override NoteController createNoteController(Note _n, Beat b, BeatMapper beatMapper)
    {
        ScratchNote n = (ScratchNote)_n;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(noteController);
        ScratchNoteController newNote = gameObject.GetComponent<ScratchNoteController>();

        newNote.model = new Tuple<Note, Beat>(n, b);
        newNote.transform.parent = beatMapper.notes.transform;
        newNote.name = "Scratch @" + b.position;
        newNote.startPosition = beatMapper.positions.CENTER.position;

        

        newNote.beatMapper = beatMapper;

        return newNote;
    }

}