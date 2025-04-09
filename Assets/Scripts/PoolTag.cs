using UnityEngine;
using System.Collections.Generic;

public class PoolTag : MonoBehaviour
{
    public List<string> poolTags = new List<string>(); // Birden fazla etiket destekleniyor

    // Belirli bir etiketi içerip içermediğini kontrol eden bir yardımcı metot
    public bool HasTag(string tag)
    {
        return poolTags.Contains(tag);
    }
}