using Flax.Build;

public class GameTarget : GameProjectTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        Modules.Add("Game");
        Modules.Add("ArizonaFramework");
    }
}
