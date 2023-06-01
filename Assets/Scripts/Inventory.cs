using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public TMP_Text candlestxt;
    public TMP_Text batteriestxt;

    public int candles = 0;
    public int batteries = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        candlestxt.text = "x" + candles;
        batteriestxt.text = "x" + batteries;
    }
}
