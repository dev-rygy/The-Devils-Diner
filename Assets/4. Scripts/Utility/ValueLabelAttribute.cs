using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class ValueLabelAttribute : PropertyAttribute
{
    public string labelText;

    public ValueLabelAttribute(string labelText)
    {
        this.labelText = labelText;
    }
}