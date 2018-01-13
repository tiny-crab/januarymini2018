using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour {

	private Rigidbody2D rb2d;
	private Rigidbody2D wallet = null;
	private GameObject walletObject;
	private FixedJoint2D anchor;
	private bool isOnWallet = false;
	[SerializeField] float speed = 1;

	void Start () { 
		rb2d = GetComponent<Rigidbody2D> (); 
		anchor = GetComponent<FixedJoint2D> ();
	}

	void Update () {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 force = (mousePos - transform.position) * speed * Time.deltaTime;
		rb2d.AddForce (force, ForceMode2D.Force);
	}

	void OnTriggerEnter2D (Collider2D collider) { 
		Debug.Log (collider.gameObject.name);
		if (collider.gameObject.name == "Wallet") {
			isOnWallet = true;
			walletObject = collider.gameObject;
			wallet = walletObject.GetComponent<Rigidbody2D> ();
		}
	}

	void OnTriggerExit2D  (Collider2D collider) {
		Debug.Log (collider.gameObject.name);
		if (collider.gameObject.name == "Wallet") {
			isOnWallet = false;
		}
	}

	void OnMouseDown () {
		if (isOnWallet) {
			anchor.enabled = true;
			anchor.connectedBody = wallet;
			walletObject.AddComponent<BoxCollider2D> ();
		}
	}
}
