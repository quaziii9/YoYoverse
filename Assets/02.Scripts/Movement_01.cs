using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement_01 : MonoBehaviour
{
    float a;
    float b;
    Vector3 mv;
   public float speed;
    Rigidbody rigid;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        a = Input.GetAxisRaw("Horizontal");
        b = Input.GetAxisRaw("Vertical");

        mv = new Vector3(a,0,b).normalized;
       // transform.Translate(mv * speed);
        rigid.velocity = mv
                      * speed;

        
    }
}
