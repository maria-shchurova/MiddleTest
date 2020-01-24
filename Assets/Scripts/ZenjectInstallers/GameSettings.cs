using System;
using Zenject;

public class GameSettings : ScriptableObjectInstaller<GameSettings>
{
	public PlayerSettings Player;
	public EnemySettings Enemy;
	public RestartSetting Restart;

	public MyInstaller.GameSettings Installer;

	// We use nested classes here to group related settings together
	[Serializable]
	public class PlayerSettings
	{
		public Player.Settings playerSettings;
	}

	[Serializable]
	public class EnemySettings
	{
		public Enemy.Settings enemySettings;
		public EnemyManager.Settings Spawner;
	}

	[Serializable]
	public class RestartSetting
	{
		public RestartHandler.Settings restartSetting;
	}

	public override void InstallBindings()
	{
		Container.BindInstance(Player.playerSettings);
		Container.BindInstance(Enemy.Spawner);
		Container.BindInstance(Enemy.enemySettings);
		Container.BindInstance(Restart.restartSetting);
		Container.BindInstance(Installer);
	}
}

