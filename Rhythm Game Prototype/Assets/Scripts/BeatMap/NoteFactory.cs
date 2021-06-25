using System;
using UnityEngine;

public class NoteFactory : MonoBehaviour
{

    public String displayName = "Normal";

    public NoteFactory addNote(NoteInput type, BeatMap beatMap)
    {
        Note n = new Note(type);
        beatMap.addNote(n);
        return this;
    }

    public void initialize()
    {
        
    }

}
