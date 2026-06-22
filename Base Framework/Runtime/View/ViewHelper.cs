using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Tidz
{
    public static class ViewHelper
    {
        public static UniTask<View> PushAsync(AssetReference viewAsset)
        {
            return ViewContainer.Instance.PushAsync(viewAsset);
        }
    }
}