using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    //fields for the UI
    public Text uiEpochField;
    public Text uiGenerationField;
    public Text uiNestBlockField;
    public Text uiQueenFitField;
    public Text uiTopAntField;


    public void SetEpochField(float ep)
    {
        uiEpochField.text = ep.ToString();
    }
    
    public void SetGenerationField(float gen)
    {
        uiGenerationField.text = gen.ToString();
    }

    public void SetNestBlockField(float blocks)
    {
        uiNestBlockField.text = blocks.ToString();
    }

    public void SetQueenFitField(float queenFit)
    {
        uiQueenFitField.text = queenFit.ToString();
    }

    public void SetTopAntField(float topAnt)
    {
        uiTopAntField.text = topAnt.ToString();
    }
    

}
