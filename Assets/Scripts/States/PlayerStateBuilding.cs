using System;
using Zenject;
using UnityEngine;

public class PlayerStateBuilding : PlayerState
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	public override void Update()
	{
		//Build();
	}

	private void OnTriggerEnter(Collider other)
	{
		//CharacterArea characterArea = other.GetComponent<CharacterArea>();
		//if (characterArea && characterArea != area && !attackedCharacters.Contains(characterArea.character))
		//{
		//	attackedCharacters.Add(characterArea.character);
		//}

		//if (other.gameObject.layer == 8)
		//{
		//	characterArea = other.transform.parent.GetComponent<CharacterArea>();
		//}
	}

}
