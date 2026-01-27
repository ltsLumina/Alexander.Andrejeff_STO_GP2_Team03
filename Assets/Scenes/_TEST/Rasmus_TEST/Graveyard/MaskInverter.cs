using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public sealed class MaskInverter : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material material = new Material(base.material);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return material;
        }
    }
}
