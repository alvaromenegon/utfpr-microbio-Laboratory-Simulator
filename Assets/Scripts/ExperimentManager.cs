using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentManager : MonoBehaviour {


    public GameObject[] requirements;
    public int numberOfRequirements;

	// Use this for initialization
	void Start ()
    {
        for (int num = 0; num <= numberOfRequirements - 1; num++)
        {
            requirements[num].SetActive(true);
        }
    }
	

}
