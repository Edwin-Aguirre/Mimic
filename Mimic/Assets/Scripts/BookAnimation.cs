using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAnimation : MonoBehaviour
{
    private Animation animationComponent;

    [SerializeField]
    private string book;
    
    [SerializeField]
    private string reverseBook;

    // Start is called before the first frame update
    void Start()
    {
        animationComponent = GetComponent<Animation>();
    }

    public void PlayBookAnimation()
    {
        animationComponent.Play(book);
    }

    public void PlayReverseBookAnimation()
    {
        animationComponent.Play(reverseBook);
    }
}
