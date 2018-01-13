using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletLogic : MonoBehaviour {
	
	private float distance;

	[SerializeField] GameHandler handler;
	[SerializeField] List<BoxCollider2D> pocketColliders;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {
		foreach (BoxCollider2D pocketCollider in pocketColliders) {
			float tempDistance = Vector3.Distance (transform.position, pocketCollider.transform.position);
			if (tempDistance < distance) {
				distance = tempDistance;
			}
		}
		handler.UpdateDistance (distance);
	}
}
