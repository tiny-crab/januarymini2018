using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour {

	private Rigidbody2D rb2d;
	private Rigidbody2D wallet = null;
	private float mouseOffsetX, mouseOffsetY;
	private Transform transform;
	[SerializeField] float speed = 1;

	void Start () { 
		rb2d = GetComponent<Rigidbody2D> ();
		transform = GetComponent<Transform> ();
	}

	void Update () {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 force = (mousePos - transform.position) * speed * Time.deltaTime;
		rb2d.AddForce (force, ForceMode2D.Force);
		// TODO don't rely on the assumption that wallet and player have the same rigidbody physics attrs
		if (wallet != null) { wallet.AddForce (force, ForceMode2D.Force); }
	}

	void OnCollisionEnter2D (Collision2D collider) {
		Debug.Log (collider.gameObject.name);

		if (collider.gameObject.name == "Wallet") {
			if (Input.GetMouseButtonDown (0)) {
				Debug.Log ("clicking");
				wallet = collider.gameObject.GetComponent<Rigidbody2D> ();
			}
		}
	}

	void OnMouseDown () { 

	}
}
