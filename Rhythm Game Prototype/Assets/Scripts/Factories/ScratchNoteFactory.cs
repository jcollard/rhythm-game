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
        beatMapper.beatMap.addNote(n);

    }

    public override NoteController createNoteController(Note _n, Beat b, BeatMapper beatMapper)
    {
        ScratchNote n = (ScratchNote)_n;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(noteController);
        ScratchNoteController newNote = gameObject.GetComponent<ScratchNoteController>();

        newNote.model = new Tuple<Note,Beat>(n, b);
        newNote.gameObjectRef = gameObject;
        newNote.transform.parent = beatMapper.notes.transform;
        newNote.name = "Scratch @" + b.position;
        newNote.startPosition = beatMapper.positions.CENTER.position;

        newNote.startTime = ((b.position - BeatMap.BEAT * beatMapper.beatsVisible) * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));
        newNote.endTime = (b.position * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));

        newNote.beatMapper = beatMapper;

        return newNote;
    }

}