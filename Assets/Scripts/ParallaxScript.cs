using UnityEngine;
using System.Collections;

public class ParallaxScript : MonoBehaviour {
   private float xPosition;
   public int offset;
   public bool followCamera;
   private Vector3 newPosition;
	// Use this for initialization
	void Start () {
	  xPosition = Camera.main.transform.position.x;
	  newPosition = new Vector3(transform.position.x,transform.position.y,transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {

	  float tempX;

	  if(followCamera) {
		newPosition.x = (Camera.main.transform.position.x - xPosition)/offset;
	  }
	  else {
		newPosition.x = (xPosition - Camera.main.transform.position.x)/offset;
	  }
	  transform.position = newPosition;
	}
}
