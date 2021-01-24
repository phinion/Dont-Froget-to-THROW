using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvasFrom;
    public GameObject canvasTo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeCanvas()
    {
        canvasTo.SetActive(true);
        canvasFrom.SetActive(false);
    }

}
