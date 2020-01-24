using UnityEngine;
public class CameraFollow : MonoBehaviour
{
	public Transform target;
	private Vector3 startOffset;

	private void Awake()
	{
		startOffset = transform.position;
	}

	private void Update()
	{
		if (target)
		{
			transform.position = target.position + startOffset;
		}
	}
}
