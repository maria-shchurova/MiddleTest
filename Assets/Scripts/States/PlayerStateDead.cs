using Zenject;
public class PlayerStateDead : PlayerState
{
	readonly Player _player;

	public PlayerStateDead(Player player)
	{
		_player = player;
	}

	public override void Start()
	{
		
	}

	public override void Dispose()
	{
		
	}

	public override void Update()
	{
	}

	public class Factory : PlaceholderFactory<PlayerStateDead>
	{
	}
}
