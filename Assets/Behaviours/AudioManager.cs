using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;//singleton
	

	private void Start ()
    {
		if (instance == null)
		{
		    Initialise();
		}
		else
		{
		    Destroy(this);
		}
	}


    private void Initialise()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }
}
