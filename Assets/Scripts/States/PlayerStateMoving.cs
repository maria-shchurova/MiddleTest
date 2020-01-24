using System;
using UnityEngine;
using Zenject;

public class PlayerStateMoving : PlayerState
{
	readonly SignalBus _signalBus;
	readonly Camera _mainCamera;
	readonly Player _player;

	public PlayerStateMoving( Player player,
		[Inject(Id = "Main")]
		Camera mainCamera,
		SignalBus signalBus)
	{
		_player = player;
		_mainCamera = mainCamera;
		_signalBus = signalBus;
	}

	public override void Update()
	{
		Move();
	}


	void Move()
	{

		_player.rb.AddForce(_player.transform.forward * _player.speed, ForceMode.VelocityChange);

		if (_player.curDir != Vector3.zero)
		{
			_player.targetRot = Quaternion.LookRotation(_player.curDir);
			if (_player.rb.rotation != _player.targetRot)
			{
				_player.rb.rotation = Quaternion.RotateTowards(_player.rb.rotation, _player.targetRot, _player.turnSpeed);
			}
		}

		var mousePos = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
		{
			_player.mouseStartPos = mousePos;
		}
		else if (Input.GetMouseButton(0))
		{
			float distance = (mousePos - _player.mouseStartPos).magnitude;
			if (distance > _player.turnTreshold)
			{
				if (distance > _player.sensitivity)
				{
					_player.mouseStartPos = mousePos - (_player.curDir * _player.sensitivity / 2f);
				}

				var curDir2D = -(_player.mouseStartPos - mousePos).normalized;
				_player.curDir = new Vector3(curDir2D.x, 0, curDir2D.y);
			}
		}
		else
		{
			_player.curDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		}
	}

	public override void Start()
	{
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 8)
		{
			_signalBus.Fire<PlayerDeadSignal>();
		}
	}

	public class Factory : PlaceholderFactory<PlayerStateMoving>
	{
	}
}
