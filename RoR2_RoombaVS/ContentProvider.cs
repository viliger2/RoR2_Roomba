using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2_Roomba.Items;
using RoR2_Roomba.States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RoR2_Roomba
{
    public class ContentProvider : IContentPackProvider
    {
        public string identifier => RoombaPlugin.GUID + "." + nameof(ContentProvider);

        private readonly ContentPack _contentPack = new ContentPack();

        public const string AssetBundleName = "roomba";
        public const string AssetBundleFolder = "AssetBundles";
        public const string SoundbanksFolder = "Soundbanks";
        public const string SoundbankFileName = "RoombaSoundBank.bnk";

        public static Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            {"stubbedror2/base/shaders/hgstandard", "RoR2/Base/Shaders/HGStandard.shader"},
            {"stubbedror2/base/shaders/hgsnowtopped", "RoR2/Base/Shaders/HGSnowTopped.shader"},
            {"stubbedror2/base/shaders/hgtriplanarterrainblend", "RoR2/Base/Shaders/HGTriplanarTerrainBlend.shader"},
            {"stubbedror2/base/shaders/hgintersectioncloudremap", "RoR2/Base/Shaders/HGIntersectionCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgcloudremap", "RoR2/Base/Shaders/HGCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgdistortion", "RoR2/Base/Shaders/HGDistortion.shader" },
            {"stubbedcalm water/calmwater - dx11 - doublesided", "Calm Water/CalmWater - DX11 - DoubleSided.shader" },
            {"stubbedcalm water/calmwater - dx11", "Calm Water/CalmWater - DX11.shader" },
            {"stubbednature/speedtree", "RoR2/Base/Shaders/SpeedTreeCustom.shader"},
            {"stubbeddecalicious/decaliciousdeferreddecal", "Decalicious/DecaliciousDeferredDecal.shader" }
        };

        public static List<Material> SwappedMaterials = new List<Material>(); //apparently you need it because reasons?

        public static class Items
        {
            public static ItemDef Maxwell;
            public static ItemDef Poster;
            public static ItemDef PileOfDirt;
        }

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

            var soundsFolderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), SoundbanksFolder);
            LoadSoundBanks(soundsFolderPath);

            var assetsFolderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), AssetBundleFolder);
            AssetBundle assetbundle = null;
            yield return LoadAssetBundle(
                System.IO.Path.Combine(assetsFolderPath, AssetBundleName),
                args.progressReceiver,
            (resultAssetBundle) => assetbundle = resultAssetBundle);

            Sprite maxwellIcon = null;
            Sprite posterIcon = null;
            Sprite trashIcon = null;

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Sprite[]>)((assets) =>
            {
                maxwellIcon = assets.First(icon => icon.name == "MaxwellIcon");
                posterIcon = assets.First(icon => icon.name == "PosterIcon");
                trashIcon = assets.First(icon => icon.name == "PileOfDirtIcon");
            }));

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Material[]>)((assets) =>
            {
                var materials = assets;

                if (materials != null)
                {
                    foreach (Material material in materials)
                    {
                        if (!material.shader.name.StartsWith("Stubbed")) { continue; }

                        var replacementShader = Addressables.LoadAssetAsync<Shader>(ShaderLookup[material.shader.name.ToLower()]).WaitForCompletion();
                        var oldName = material.shader.name.ToLower();
                        if (replacementShader)
                        {
                            material.shader = replacementShader;
                            SwappedMaterials.Add(material);
                        }
                        else
                        {
                            Log.Warning("Couldn't find replacement shader for " + material.shader.name.ToLower());
                        }
                    }
                }
            }));

            var glassMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matShatteredGlass.mat");
            while (!glassMaterial.IsDone)
            {
                yield return null;
            }

            var bombEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab");
            while (!bombEffect.IsDone)
            {
                yield return null;
            }

            var grenadeExplosion = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab");
            while (!grenadeExplosion.IsDone)
            {
                yield return null;
            }
            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            {
                if (RoombaConfigs.CustomItems.Value)
                {
                    #region Maxwell
                    RoR2_Roomba.Items.Maxwell.MaxwellProcType = ProcTypeAPI.ReserveProcType();

                    var explosionCopy = PrefabAPI.InstantiateClone(grenadeExplosion.Result, "MaxwellExplosionEffect", false);
                    explosionCopy.GetComponent<EffectComponent>().soundName = "Roomba_HL_Explosion_Play";

                    var maxwellPickUp = assets.First(pickup => pickup.name == "PickupMaxwell");
                    Items.Maxwell = Maxwell.CreateItemDef(maxwellPickUp, maxwellIcon);
                   
                    var evilMaxwell = assets.First(prefab => prefab.name == "EvilMaxwellPrefab");
                    Maxwell.EvilMaxwellPrefab = Maxwell.CreateEvilMaxwellPrefab(evilMaxwell, explosionCopy);
                    #endregion

                    #region Poster
                    var posterPickup = assets.First(pickup => pickup.name == "PickupPoster");
                    Items.Poster = Poster.CreateItemDef(posterPickup, posterIcon);
                    #endregion

                    #region PileOfDirt
                    var pileOfDirtPickup = assets.First(pickup => pickup.name == "PickupPileOfDirt");
                    Items.PileOfDirt = PileOfDirt.CreateItemDef(pileOfDirtPickup, glassMaterial.Result, trashIcon);

                    var pileOfDirtSpawn = assets.First(prefab => prefab.name == "PileOfDirtSpawn");
                    pileOfDirtSpawn = PileOfDirt.CreatePileOfDirtSpawnPrefab(pileOfDirtSpawn, glassMaterial.Result);

                    PileOfDirt.scPileOfDirt = PileOfDirt.CreatePileOfDirtSpawnCard(pileOfDirtSpawn);
                    #endregion

                    _contentPack.effectDefs.Add(new EffectDef[] { new EffectDef(explosionCopy) });
                    _contentPack.itemDefs.Add(new RoR2.ItemDef[] { Items.Maxwell, Items.Poster, Items.PileOfDirt });
                    _contentPack.networkedObjectPrefabs.Add(new GameObject[] { Maxwell.EvilMaxwellPrefab, pileOfDirtSpawn });
                }

                var roombaFactory = new RoombaFactory();

                #region RoombaNothing
                // body
                var roombaNothingBody = assets.First(bp => bp.name == "RoombaNothingBody");
                roombaNothingBody = roombaFactory.CreateRoombaBody(roombaNothingBody, bombEffect.Result);

                // master
                var roombaNothingMaster = assets.First(mp => mp.name == "RoombaNothingMaster");
                roombaNothingMaster = roombaFactory.CreateRoombaMaster(roombaNothingMaster, roombaNothingBody);

                RoombaFactory.cscRoomba = roombaFactory.CreateCharacterSpawnCard(roombaNothingMaster);
                #endregion

                #region RoombaMaxwell
                // body
                var roombaMaxwellBody = assets.First(bp => bp.name == "RoombaMaxwellBody");
                roombaMaxwellBody = roombaFactory.CreateRoombaBody(roombaMaxwellBody, bombEffect.Result);

                // master
                var roombaMaxwellMaster = assets.First(mp => mp.name == "RoombaMaxwellMaster");
                roombaMaxwellMaster = roombaFactory.CreateRoombaMaster(roombaMaxwellMaster, roombaMaxwellBody);

                RoombaFactory.cscRoombaMaxwell = roombaFactory.CreateCharacterSpawnCard(roombaMaxwellMaster);
                #endregion

                #region RoombaTV
                // body
                var roombaTVBody = assets.First(bp => bp.name == "RoombaTVBody");
                roombaTVBody = roombaFactory.CreateRoombaBody(roombaTVBody, bombEffect.Result);

                // master
                var roombaTVMaster = assets.First(mp => mp.name == "RoombaTVMaster");
                roombaTVMaster = roombaFactory.CreateRoombaMaster(roombaTVMaster, roombaTVBody);

                RoombaFactory.cscRoombaTV = roombaFactory.CreateCharacterSpawnCard(roombaTVMaster);
                #endregion

                _contentPack.entityStateTypes.Add(new Type[] { typeof(InvincibleRoombaState), typeof(LookBusyFar), typeof(PileOfDirtDeath), typeof(RoombaSpawnState), typeof(WanderFar) });
                _contentPack.bodyPrefabs.Add(new GameObject[] { roombaNothingBody, roombaMaxwellBody, roombaTVBody });
                _contentPack.masterPrefabs.Add(new GameObject[] { roombaNothingMaster, roombaMaxwellMaster, roombaTVMaster });
            }));

            _contentPack.networkSoundEventDefs.Add(new NetworkSoundEventDef[]
            {
                CreateNetworkSoundDef("Roomba_BadToTheBone_Play"),
                CreateNetworkSoundDef("Roomba_HL_Explosion_Play"),
                CreateNetworkSoundDef("Roomba_Breaking_Play")
            });

            yield break;
        }

        public static NetworkSoundEventDef CreateNetworkSoundDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.eventName = eventName;

            return networkSoundEventDef;
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

        internal static void LoadSoundBanks(string soundbanksFolderPath)
        {
            var akResult = AkSoundEngine.AddBasePath(soundbanksFolderPath);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank base path : {soundbanksFolderPath}");
            }
            else
            {
                Log.Error(
                    $"Error adding base path : {soundbanksFolderPath} " +
                    $"Error code : {akResult}");
            }

            akResult = AkSoundEngine.LoadBank(SoundbankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {SoundbankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {SoundbankFileName} " +
                    $"Error code : {akResult}");
            }
        }
    }
}
