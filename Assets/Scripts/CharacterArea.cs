using UnityEngine;

public class CharacterArea : MonoBehaviour
{
	public Builder builder; 
	public MeshCollider coll;

	private void Awake()
	{
		coll = gameObject.AddComponent<MeshCollider>();
	}

}
