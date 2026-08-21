using HG;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;
using RoR2.Networking;
using RoR2BepInExPack.GameAssetPaths;
using ShaderSwapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using static RoR2.Console;
using static UnityEngine.UI.Image;

namespace DesolateCoast.Content
{
    public static class DesolateCoastContent
    {

        internal const string ScenesAssetBundleFileName = "coastscenes";
        internal const string AssetsAssetBundleFileName = "coastassets";

        private static AssetBundle _scenesAssetBundle;
        private static AssetBundle _assetsAssetBundle;

        internal static UnlockableDef[] UnlockableDefs;
        internal static SceneDef[] SceneDefs;

        //Broadcast Perch
        internal static SceneDef coastSceneDef;
        internal static Sprite coastSceneDefPreviewSprite;
        internal static Material coastBazaarSeer;

        //Simulacrum
        internal static SceneDef simuSceneDef;
        internal static Sprite simuSceneDefPreviewSprite;
        internal static Material simuBazaarSeer;
        internal static Material terrainMaterial;
        internal static Material itTerrainMaterial;


        public static List<Material> SwappedMaterials = new List<Material>();

        internal static IEnumerator LoadAssetBundlesAsync(AssetBundle scenesAssetBundle, AssetBundle assetsAssetBundle, IProgress<float> progress, ContentPack contentPack)
        {
            _scenesAssetBundle = scenesAssetBundle;
            _assetsAssetBundle = assetsAssetBundle;
            
            var upgradeStubbedShaders = _assetsAssetBundle.UpgradeStubbedShadersAsync();
            while (upgradeStubbedShaders.MoveNext())
            {
                yield return upgradeStubbedShaders.Current;
            }

            yield return LoadAllAssetsAsync(assetsAssetBundle, progress, (Action<UnlockableDef[]>)((assets) =>
            {
                contentPack.unlockableDefs.Add(assets);
            }));

            /*
            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<Sprite[]>)((assets) =>
            {
                coastSceneDefPreviewSprite = assets.First(a => a.name == "texDCoastScenePreview");
                simuSceneDefPreviewSprite = assets.First(a => a.name == "texDCoastScenePreview");
            }));
            */

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<Material[]>)((assets) =>
            {
                terrainMaterial = assets.First(a => a.name == "matCoastTerrain");
                itTerrainMaterial = assets.First(a => a.name == "matITCoastTerrain");

            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<SceneDef[]>)((assets) =>
            {
                SceneDefs = assets;
                coastSceneDef = SceneDefs.First(sd => sd.baseSceneNameOverride == DesolateCoast.mapName);
                simuSceneDef = SceneDefs.First(sd => sd.baseSceneNameOverride == DesolateCoast.simuName);
                Log.Debug(coastSceneDef.nameToken);
                contentPack.sceneDefs.Add(assets);
            }));

            coastSceneDef.portalMaterial = R2API.StageRegistration.MakeBazaarSeerMaterial((Texture2D)coastSceneDef.previewTexture);

            var mainTrackDefRequest = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/MusicTrackDefs/muFULLSong07.asset");
            while (!mainTrackDefRequest.IsDone)
            {
                yield return null;
            }
            var bossTrackDefRequest = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/DLC2/Common/muSong_Lakes&HabitatBoss.asset");
            while (!bossTrackDefRequest.IsDone)
            {
                yield return null;
            }

            coastSceneDef.mainTrack = mainTrackDefRequest.Result;
            coastSceneDef.bossTrack = bossTrackDefRequest.Result;

            simuSceneDef.mainTrack = coastSceneDef.mainTrack;
            simuSceneDef.bossTrack = coastSceneDef.bossTrack;


            if (DesolateCoast.enableRegular.Value)
            {
                R2API.StageRegistration.RegisterSceneDefToNormalProgression(coastSceneDef);
            }
            
            if (DesolateCoast.enableSimulacrum.Value && DesolateCoast.stage1Simulacrum.Value)
            {
                Simulacrum.RegisterSceneToSimulacrum(simuSceneDef, true);
            } else if (DesolateCoast.enableSimulacrum.Value && !DesolateCoast.stage1Simulacrum.Value)
            {
                Simulacrum.RegisterSceneToSimulacrum(simuSceneDef, false);
            }
            

        }

        internal static void Unload()
        {
            _assetsAssetBundle.Unload(true);
            _scenesAssetBundle.Unload(true);
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
