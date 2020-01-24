using ModestTree;


public enum PlayerStates
{
	Moving,
	Dead,
	Count
}
public class PlayerStateFactory
{
	readonly PlayerStateMoving.Factory _movingFactory;
	readonly PlayerStateDead.Factory _deadFactory;

	public PlayerStateFactory(
		PlayerStateDead.Factory deadFactory,
		PlayerStateMoving.Factory movingFactory)
	{
		_movingFactory = movingFactory;
		_deadFactory = deadFactory;
	}

	public PlayerState CreateState(PlayerStates state)
	{
		switch (state)
		{
			case PlayerStates.Dead:
				{
					return _deadFactory.Create();
				}
			case PlayerStates.Moving:
				{
					return _movingFactory.Create();
				}
		}

		throw Assert.CreateException();
	}
}

