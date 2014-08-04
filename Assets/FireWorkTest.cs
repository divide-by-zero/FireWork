using UnityEngine;
using System.Collections;

public class FireWorkTest : MonoBehaviour {
	public GameObject fireWork;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Instantiate(fireWork);
		}
	}
}
