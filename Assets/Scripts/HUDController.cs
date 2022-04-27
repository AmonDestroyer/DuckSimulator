using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public int featherCount;
    public TextMeshProUGUI featherCountText;

    // Start is called before the first frame update
    void Start()
    {
        featherCount = 0;
        SetCountText();
    }

    public void SetCountText()
    {
        featherCountText.text = "Feathers: " + featherCount.ToString();
    }

}
