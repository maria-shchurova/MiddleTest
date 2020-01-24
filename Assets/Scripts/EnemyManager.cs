using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
public class EnemyManager : ITickable, IFixedTickable
{
	readonly List<Enemy> _enemies = new List<Enemy>();
	readonly Settings _settings;
	readonly Enemy.Factory __enemyFactory;

	float _timeToNextSpawn;
	float _timeIntervalBetweenSpawns;
	bool _started;

	[InjectOptional]
	bool _autoSpawn = true;
	[Inject]
	public EnemyManager(Settings settings, Enemy.Factory enemyFactory)
	{
		_settings = settings;
		_timeIntervalBetweenSpawns = _settings.maxSpawnTime / (_settings.maxSpawns - _settings.startingSpawns);
		_timeToNextSpawn = _timeIntervalBetweenSpawns;
		__enemyFactory = enemyFactory;
	}

	public IEnumerable<Enemy> Asteroids
	{
		get { return _enemies; }
	}

	public void Start()
	{
		Assert.That(!_started);
		_started = true;

		ResetAll();

		for (int i = 0; i < _settings.startingSpawns; i++)
		{
			SpawnNext();
		}
	}
	
	void ResetAll()
	{
		foreach (var asteroid in _enemies)
		{
			GameObject.Destroy(asteroid.gameObject);
		}

		_enemies.Clear();
	}

	public void Stop()
	{
		Assert.That(_started);
		_started = false;
	}

	public void FixedTick()
	{
		for (int i = 0; i < _enemies.Count; i++)
		{
			_enemies[i].FixedTick();
		}
	}

	public void Tick()
	{
		if (_started && _autoSpawn)
		{
			_timeToNextSpawn -= Time.deltaTime;

			if (_timeToNextSpawn < 0 && _enemies.Count < _settings.maxSpawns)
			{
				_timeToNextSpawn = _timeIntervalBetweenSpawns;
				SpawnNext();
			}
		}
	}

	public void SpawnNext()
	{
		var enemy = __enemyFactory.Create();

		enemy.transform.position = GetRandomStartPosition();
		enemy._builder.color = GetRandomColor();
		_enemies.Add(enemy);
	}

	Vector3 GetRandomDirection()
	{
		var theta = Random.Range(0, Mathf.PI * 2.0f);
		return new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
	}

	Vector3 GetRandomStartPosition()
	{
		float groundRadius = _settings.groundRadius;
		return new Vector3(UnityEngine.Random.Range(-groundRadius, groundRadius) , 0, UnityEngine.Random.Range(-groundRadius, groundRadius));
	}

	Color GetRandomColor()
	{
		return new Color(
	  Random.Range(0f, 1f),
	  Random.Range(0f, 1f),
	  Random.Range(0f, 1f)
	  );
	}

	[Serializable]
	public class Settings
	{
		public float groundRadius;
		public int startingSpawns;
		public int maxSpawns;
		public float maxSpawnTime;
	}
}

