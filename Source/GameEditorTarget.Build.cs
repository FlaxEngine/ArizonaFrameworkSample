using Flax.Build;

public class GameEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        Modules.Add("Game");
        Modules.Add("ArizonaFramework");
    }
}
