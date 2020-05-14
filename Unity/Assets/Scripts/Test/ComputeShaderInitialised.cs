using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderInitialised : MonoBehaviour
{
    [SerializeField] private ComputeShader myComputeShader;
    private int indexOfKernel;

    void Start()
    {
        indexOfKernel = myComputeShader.FindKernel("CSMain");
    }
    void UseShaderAnytime()
    {
        int x = 16;
        int y = 16;
        int z = 4;
        myComputeShader.Dispatch(indexOfKernel, x, y, z);
    }
}
