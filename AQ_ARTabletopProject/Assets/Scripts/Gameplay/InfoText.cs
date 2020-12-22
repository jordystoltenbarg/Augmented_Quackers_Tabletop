using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New InfoText", menuName = "InfoText")]
public class InfoText : ScriptableObject
{
    public string line1;
    public string line2;
    public string line3;
    public string line4;
    public string line5;

    public string text
    {
        get
        {
            if (string.IsNullOrEmpty(line2))
            {
                return $"{line1}";
            }
            else if (string.IsNullOrEmpty(line3))
            {
                return $"{line1}\n{line2}";
                
            }
            else if (string.IsNullOrEmpty(line4))
            {
                return $"{line1}\n{line2}\n{line3}";

            }
            else if (string.IsNullOrEmpty(line5))
            {
                return $"{line1}\n{line2}\n{line3}\n{line4}";

            }
            else
            {
                return $"{line1}\n{line2}\n{line3}\n{line4}\n{line5}";
            }

        }
    }
}
