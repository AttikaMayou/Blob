using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ComputeColor : MonoBehaviour
{
    [Header("Color")]
    public Gradient _gradiantViscious;
    public Gradient _gradiantIdle;
    public Gradient _gradiantLiquid;

    private Color color = new Color(0, 0, 0);
    static public Color newCol = new Color(0, 0, 0);
    private int nbSphere = 0;

    // Update is called once per frame
    void Update()
    {
        nbSphere = BlobUtils.GetBlobsCurrentPositions().Count;

        //Idle
        if (Input.GetKeyDown(KeyCode.E))
        {
            color += _gradiantIdle.Evaluate(1f / Random.Range(1, 100));
            newCol = color / (nbSphere + 1);
        }

        //Liquid
        if (Input.GetKeyDown(KeyCode.L))
        {
            color += _gradiantLiquid.Evaluate(1f / Random.Range(1, 100));
            newCol = color / (nbSphere + 1);
        }

        //Viscious
        if (Input.GetKeyDown(KeyCode.V))
        {
            color += _gradiantViscious.Evaluate(1f / Random.Range(1, 100));
            newCol = color / (nbSphere + 1);
        }

    }

    static public Color getColor()
    {
        return newCol;
    }
}
