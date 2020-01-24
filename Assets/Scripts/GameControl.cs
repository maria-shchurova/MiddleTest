using System;
using ModestTree;
using UnityEngine;
using Zenject;
public enum GamingStates
{
	WaitingToStart,
	Playing,
	GameOver
}

public class GameControl : IInitializable, ITickable, IDisposable
{
	readonly SignalBus _signalBus;
	readonly Player _player;
	readonly EnemyManager _enemySpawner;

	GamingStates _state = GamingStates.WaitingToStart;
	float _elapsedTime;

	public GameControl(
		Player player,
		EnemyManager enemySpawner,
		SignalBus signalBus)
	{
		_enemySpawner = enemySpawner;
		_signalBus = signalBus;
		_player = player;
	}

	public float ElapsedTime
	{
		get { return _elapsedTime; }
	}

	public GamingStates State
	{
		get { return _state; }
	}

	public void Initialize()
	{
		Cursor.visible = false;

		_signalBus.Subscribe<PlayerDeadSignal>(OnPlayerIsDead);
	}

	public void Dispose()
	{
		_signalBus.Unsubscribe<PlayerDeadSignal>(OnPlayerIsDead);
	}

	public void Tick()
	{
		switch (_state)
		{
			case GamingStates.WaitingToStart:
				{
					UpdateStarting();
					break;
				}
			case GamingStates.Playing:
				{
					UpdatePlaying();
					break;
				}
			case GamingStates.GameOver:
				{
					UpdateGameOver();
					break;
				}
			default:
				{
					Assert.That(false);
					break;
				}
		}
	}

	void UpdateGameOver()
	{
		Assert.That(_state == GamingStates.GameOver);

		if (Input.GetMouseButtonDown(0))
		{
			StartGame();
		}
	}

	void OnPlayerIsDead()
	{
		Assert.That(_state == GamingStates.Playing);
		_state = GamingStates.GameOver;
		_enemySpawner.Stop();
		Debug.Log("bus Fired");
	}

	void UpdatePlaying()
	{
		Assert.That(_state == GamingStates.Playing);
		_elapsedTime += Time.deltaTime;
	}

	void UpdateStarting()
	{
		Assert.That(_state == GamingStates.WaitingToStart);

		StartGame();
	}

	void StartGame()
	{
		Assert.That(_state == GamingStates.WaitingToStart || _state == GamingStates.GameOver);

		_player.Position = Vector3.zero;
		_elapsedTime = 0;
		_player.ChangeState(PlayerStates.Moving);
		_state = GamingStates.Playing;
		_enemySpawner.Start();
	}
}