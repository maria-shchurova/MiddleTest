using UnityEngine;
using Zenject;
using System;
public class MyInstaller : MonoInstaller
{
	[Inject]
	GameSettings _settings = null;
	public override void InstallBindings()
    {
		InstallEnemies();
		InstallPlayer();
		InstallMisc();
		InstallSignals();
	}

	void InstallSignals()
	{
		SignalBusInstaller.Install(Container);

		Container.DeclareSignal<PlayerDeadSignal>();
	}

	void InstallMisc()
	{
		Container.BindInterfacesAndSelfTo<GameControl>().AsSingle();
		Container.BindInterfacesTo<RestartHandler>().AsSingle();
	}

	void InstallPlayer()
	{
		Container.Bind<PlayerStateFactory>().AsSingle();

		Container.BindFactory<PlayerStateDead, PlayerStateDead.Factory>().WhenInjectedInto<PlayerStateFactory>();
		Container.BindFactory<PlayerStateMoving, PlayerStateMoving.Factory>().WhenInjectedInto<PlayerStateFactory>();
	}

	void InstallEnemies()
	{
		Container.BindInterfacesAndSelfTo<EnemyManager>().AsSingle().NonLazy();

		Container.BindFactory<Enemy, Enemy.Factory>()

		.FromComponentInNewPrefab(_settings.EnemyPrefab)
		.WithGameObjectName("Enemy")
		.UnderTransformGroup("Enemies");
	}

	[Serializable]
	public class GameSettings
	{
		public GameObject PlayerPrefab;
		public GameObject EnemyPrefab;
	}
}

