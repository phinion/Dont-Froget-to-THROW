using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

	public Transform bar;
	private Quaternion rotation;

	// Use this for initialization
	void Start () {
		rotation = gameObject.GetComponentInParent<Transform>().rotation;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		gameObject.GetComponentInParent<Transform> ().rotation = rotation;
	}

	public void SetSize(float sizeNormalized) {
		bar.localScale = new Vector3 (sizeNormalized, 1f);
	}
}
