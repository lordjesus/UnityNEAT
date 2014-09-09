using UnityEngine;
using System.Collections;

public class GoalPiece : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag.Equals("Car"))
        {
            col.collider.GetComponent<CarController>().NewLap();           
        }
    }
}
