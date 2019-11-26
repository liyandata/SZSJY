using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptsManager : MonoBehaviour {

    public GameObject[] Objs;
	// Use this for initialization
	void Start () {
        Objs[0].SetActive(true);
        Objs[1].SetActive(true);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
