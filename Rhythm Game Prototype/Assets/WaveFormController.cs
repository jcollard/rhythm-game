using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFormController : MonoBehaviour
{

    public BeatMapper beatMapper;


    private void OnMouseDown()
    {
        UpdateCursor();
    }

    private void OnMouseDrag()
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (beatMapper.trackSource.clip == null)
        {
            return;
        }
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //TODO: 8 Should be the width of the box
        float percentage = (mousePosition.x - this.gameObject.transform.position.x) / 8;
        float length = beatMapper.trackSource.clip.length;
        float songPosition = length * percentage;
        long cursorPosition = beatMapper.beatMap.SecondsToCursorPosition(songPosition);
        beatMapper.beatMap.setCursor(cursorPosition);
        beatMapper.beatMap.prevQuarter();
    }

}
