using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoR2_Roomba
{
    public class ContentProvider : IContentPackProvider
    {
        public string identifier => RoombaPlugin.GUID + "." + nameof(ContentProvider);

        private readonly ContentPack _contentPack = new ContentPack();

        public const string AssetBundleName = "roomba";

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(_contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            _contentPack.identifier = identifier;

            var assetsFolderPath = Path.Combine(Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), "assetbundles");
            AssetBundle assetbundle = null;
            yield return LoadAssetBundle(
                Path.Combine(assetsFolderPath, AssetBundleName),
                args.progressReceiver,
                (resultAssetBundle) => assetbundle = resultAssetBundle);

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            {
                var roombaFactory = new RoombaFactory();

                // body
                var roombaBody = assets.First(bp => bp.name == "RoombaBody");
                roombaBody = roombaFactory.CreateRoombaBody(roombaBody);
                _contentPack.bodyPrefabs.Add(new GameObject[] {roombaBody });

                // master
                var roombaMaster = assets.First(mp => mp.name == "RoombaMaster");
                roombaMaster = roombaFactory.CreateRoombaMaster(roombaMaster, roombaBody);
                _contentPack.masterPrefabs.Add(new GameObject[] {roombaMaster});

                roombaFactory.CreateCharacterSpawnCard(roombaMaster);
            }));

            yield break;
        }

        private IEnumerator LoadAssetBundle(string assetBundleFullPath, IProgress<float> progress, Action<AssetBundle> onAssetBundleLoaded)
        {
            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundleFullPath);
            while (!assetBundleCreateRequest.isDone)
            {
                progress.Report(assetBundleCreateRequest.progress);
                yield return null;
            }

            onAssetBundleLoaded(assetBundleCreateRequest.assetBundle);

            yield break;
        }

        private static IEnumerator LoadAllAssetsAsync<T>(AssetBundle assetBundle, IProgress<float> progress, Action<T[]> onAssetsLoaded) where T : UnityEngine.Object
        {
            var sceneDefsRequest = assetBundle.LoadAllAssetsAsync<T>();
            while (!sceneDefsRequest.isDone)
            {
                progress.Report(sceneDefsRequest.progress);
                yield return null;
            }

            onAssetsLoaded(sceneDefsRequest.allAssets.Cast<T>().ToArray());

            yield break;
        }
    }
}
