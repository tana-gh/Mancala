using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace tana_gh.Mancala
{
    public partial class RootEntryPoint : IAsyncStartable
    {
        partial void Init();

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            Init();

            await UniTask.Yield();

            Debug.Log("Root entry point started.");
        }
    }
}
