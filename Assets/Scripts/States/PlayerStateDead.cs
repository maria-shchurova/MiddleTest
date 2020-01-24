using Zenject;
using UnityEngine;
using System;

public class PlayerStateDead : PlayerState
{
	readonly Player _player;

	public PlayerStateDead(Player player)
	{
		_player = player;
	}

	public override void Start()
	{
		Debug.Log("Die from PlayerStateDead");
	}

	public override void Dispose()
	{
		Debug.Log("DeadState Dispose");
	}

	public override void Update()
	{
	}

	public class Factory : PlaceholderFactory<PlayerStateDead>
	{
	}
}
