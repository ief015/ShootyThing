using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	public Character character;
	public Text txtHP;

	///////////////////////////////////////////////////////////////////////////
	void Start()
	{
		
	}

	///////////////////////////////////////////////////////////////////////////
	void Update()
	{
		txtHP.text = "HP " + Mathf.CeilToInt(character.currentHealth) + "%";
	}
}
