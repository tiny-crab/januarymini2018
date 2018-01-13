using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour {

	private Rigidbody2D rb2d;
	private float mouseOffsetX, mouseOffsetY;
	private Transform transform;
	[SerializeField] float speed = 1;


	void Start () { 
		rb2d = GetComponent<Rigidbody2D> ();
		transform = GetComponent<Transform> ();
	}
	
	// non physics logic here
	void Update () {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		rb2d.AddForce ((mousePos - transform.position) * speed * Time.deltaTime, ForceMode2D.Force);
	}

	// physics logic here
	void FixedUpdate () {

	}

	void OnCollisionEnter2D (Collision2D collider) {
		Debug.Log (collider.gameObject.name);
	}
}
