using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpinner : MonoBehaviour
{
    [SerializeField]
    private float spinSpeed;
    [SerializeField]
    private float bobSpeed;
    [SerializeField]
    private float bobHeight;
    
    private Vector3 bobPosition;
    private float bob_Y;

    // Start is called before the first frame update
    void Start()
    {
        bobPosition = transform.position;
        bob_Y = transform.position.y;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Spins the powerups
        transform.Rotate(0, spinSpeed, 0, Space.World);
        bobPosition.y = bob_Y + bobHeight * Mathf.Sin(bobSpeed * Time.time);
        transform.position = bobPosition;
    }
}
