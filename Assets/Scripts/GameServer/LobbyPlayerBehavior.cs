using Fusion;

public class LobbyPlayerBehavior : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();
    }

    public override void Render()
    {
        base.Render();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
    }
}
