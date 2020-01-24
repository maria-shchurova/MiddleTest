using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using Zenject;

public class RestartHandler : IInitializable, IDisposable, ITickable
{
	readonly SignalBus _signalBus;
	readonly Settings _settings;

	bool _isDelaying;
	float _delayStartTime;

	public RestartHandler(
		Settings settings,
		SignalBus signalBus)
	{
		_signalBus = signalBus;
		_settings = settings;
	}

	public void Initialize()
	{
		_signalBus.Subscribe<PlayerDeadSignal>(OnPlayerDied);
	}

	public void Dispose()
	{
		_signalBus.Unsubscribe<PlayerDeadSignal>(OnPlayerDied);
	}

	public void Tick()
	{
		if (_isDelaying)
		{
			if (Time.realtimeSinceStartup - _delayStartTime > _settings.RestartDelay)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
	}

	void OnPlayerDied()
	{
		// Wait a bit before restarting the scene
		_delayStartTime = Time.realtimeSinceStartup;
		_isDelaying = true;
	}

	[Serializable]
	public class Settings
	{
		public float RestartDelay;
	}

}
