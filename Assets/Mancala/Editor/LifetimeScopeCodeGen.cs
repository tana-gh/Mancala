using System;
using System.Linq;
using tana_gh.UnityDotG.Editor;

namespace tana_gh.Mancala.Editor
{
    [CodeGen]
    public class SandboxLifetimeScopeCodeGen
    {
        public static void Generate(CodeGenContext context)
        {
            context.AutoGeneratedFolder = UnityDotGConstants.AutoGeneratedFolder;

            foreach (var sceneKind in Enum.GetValues(typeof(SceneKind)).Cast<SceneKind>())
            {
                GenerateOne(context, sceneKind);
            }
        }

        public static void GenerateOne(CodeGenContext context, SceneKind sceneKind)
        {
            context.AddCode($"{sceneKind}LifetimeScope.g.cs",
$@"
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace tana_gh.Mancala
{{
    public partial class {sceneKind}LifetimeScope
    {{
        protected override void Configure(IContainerBuilder builder)
        {{
            base.Configure(builder);

            builder.RegisterEntryPoint<{sceneKind}EntryPoint>();
        }}
    }}
}}
"
            );
        }
    }
}
