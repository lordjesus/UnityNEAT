using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class BallController : MonoBehaviour {

    IBlackBox Brain;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {


        GetComponent<Rigidbody>().AddForce(10 * Time.deltaTime, 0, 0);
	}
}
