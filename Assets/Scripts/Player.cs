using System;
using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	[SerializeField]
	Rigidbody _rigidbody;

	[SerializeField]
	MeshRenderer _meshRenderer;

	PlayerStateFactory _stateFactory;
	PlayerState _state;
	Settings _settings;

	[SerializeField]
	public Builder _builder;

	public List<Vector3> playerAreaVertices = new List<Vector3>();


	[Inject]
	public void Construct(PlayerStateFactory stateFactory, Settings settings)
	{
		_stateFactory = stateFactory;
		_settings = settings;
	}

	public Quaternion targetRot;
	public float speed = 1f;
	public float turnSpeed = 14f;

	public float sensitivity = 300f;
	public float turnTreshold = 15f;
	public Vector3 mouseStartPos;
	public Vector3 curDir;

	public Rigidbody rb
	{
		get { return _rigidbody; }
	}

	public MeshRenderer MeshRenderer
	{
		get { return _meshRenderer; }
	}

	public Vector3 Position
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	public void Start()
	{
		ChangeState(PlayerStates.Moving);
	}

	public void Update()
	{
		_state.Update();
	}
	public void FixedUpdate()
	{
		playerAreaVertices = _builder.areaVertices;
	}

	public void ChangeState(PlayerStates state)
	{
		if (_state != null)
		{
			_state.Dispose();
			_state = null;
		}

		_state = _stateFactory.CreateState(state);
		_state.Start();
	}

	[Serializable]
	public class Settings
	{
		public float Speed;
		public float turnSpeed;
	}

}
