using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasShaker : MonoBehaviour
{
    private Animation animationComponent;

    private string canvasAnimation = "Cannot Transform";

    // Start is called before the first frame update
    void Start()
    {
        animationComponent = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCanvasShakerAnimation()
    {
        animationComponent.Play(canvasAnimation);
        SoundManager.PlaySound("select 12");
    }
}
