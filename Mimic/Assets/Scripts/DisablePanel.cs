using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePanel : MonoBehaviour
{
    public bool shouldBeDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisablePanelAfterAnimation()
    {
        if(shouldBeDisabled)
        {   
            gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}
