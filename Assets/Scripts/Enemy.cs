using System;
using UnityEngine;
using Zenject;
public class Enemy : MonoBehaviour
{
	Rigidbody _rigidbody;
	MeshRenderer _meshRenderer;
	Settings _settings;

	public Builder _builder;
	public Color color;
	public Quaternion targetRot;

	public Vector3 curDir;

	public int positionsTillReturning = 2;
	public int curPosition;


	public Vector3 targetPos
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	[Inject]
	public void Construct(Settings settings)
	{
		_settings = settings;
		_rigidbody = GetComponent<Rigidbody>();
		_builder = GetComponent<Builder>();
		_meshRenderer = GetComponent<MeshRenderer>();
	}


	void Move()
	{
		var transPos = transform.position; //высчитывается слишком рано
		if ((targetPos - transPos).magnitude < .5f)
		{
			if (curPosition < positionsTillReturning)
			{
				curPosition++;
				float clamp = _settings.ClampMagnitue;
				targetPos = new Vector3(UnityEngine.Random.Range(transPos.x - clamp, transPos.x + clamp), 0, UnityEngine.Random.Range(transPos.z - clamp, transPos.z + clamp));
			}
			else
			{
				curPosition = 0;
				targetPos = _builder.areaVertices[_builder.GetClosestAreaVertice(transPos)];
			}
		}

		curDir = (targetPos - transPos).normalized;

		_rigidbody.AddForce(transform.forward * _settings.maxSpeed, ForceMode.VelocityChange);

		if (curDir != Vector3.zero)
		{
			targetRot = Quaternion.LookRotation(curDir);
			if (_rigidbody.rotation != targetRot)
			{
				_rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRot, _settings.turnSpeed);
			}
		}

		transform.position = Vector3.ClampMagnitude(transPos, _settings.ClampMagnitue);
	}

	public void FixedTick()
	{
		if (_rigidbody)
		{
			var speed = _rigidbody.velocity.magnitude;

			if (speed > _settings.maxSpeed)
			{
				var dir = _rigidbody.velocity / speed;
				_rigidbody.velocity = dir * _settings.maxSpeed;
			}
		}
	}

	public void Update()
	{
		Move();
	}

	[Serializable]
	public class Settings
	{
		public float ClampMagnitue;
		public float turnSpeed;
		public float maxSpeed;
	}

	public class Factory : PlaceholderFactory<Enemy>
	{
	}

}
