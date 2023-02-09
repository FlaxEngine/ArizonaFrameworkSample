using Flax.Build;
using Flax.Build.NativeCpp;

public class Game : GameModule
{
    /// <inheritdoc />
    public override void Setup(BuildOptions options)
    {
        base.Setup(options);

        BuildNativeCode = false; // Enable this to use C++ scripts
        Tags["Network"] = string.Empty; // Hint usage of networking features in code (eg. Replication/RPC)
        options.ScriptingAPI.IgnoreMissingDocumentationWarnings = true;
        options.PublicDependencies.Add("ArizonaFramework");
        options.PrivateDependencies.Add("Networking");
    }
}
