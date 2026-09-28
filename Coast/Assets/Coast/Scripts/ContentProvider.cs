using R2API;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Path = System.IO.Path;

namespace DesolateCoast.Content
{
    public class ContentProvider : IContentPackProvider
    {
        public string identifier => DesolateCoast.GUID + "." + nameof(ContentProvider);

        private readonly ContentPack _contentPack = new ContentPack();

        public static String assetDirectory;

        internal const string MusicSoundBankFileName = "WormCoastMusic.bnk";
        internal const string InitSoundBankFileName = "WormCoastMusicInit.bnk";
        //internal const string SoundsSoundBankFileName = "WormAOSounds.bnk";

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            _contentPack.identifier = identifier;

            var assetsFolderFullPath = Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location);
            assetDirectory = assetsFolderFullPath;

            var musicFolderFullPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), "Soundbanks");
            LoadSoundBanks(musicFolderFullPath);


            AssetBundle scenesAssetBundle = null;
            yield return LoadAssetBundle(
                Path.Combine(assetsFolderFullPath, DesolateCoastContent.ScenesAssetBundleFileName),
                args.progressReceiver,
                (assetBundle) => scenesAssetBundle = assetBundle);

            AssetBundle assetsAssetBundle = null;
            yield return LoadAssetBundle(
                Path.Combine(assetsFolderFullPath, DesolateCoastContent.AssetsAssetBundleFileName),
                args.progressReceiver,
                (assetBundle) => assetsAssetBundle = assetBundle);

            yield return DesolateCoastContent.LoadAssetBundlesAsync(
                scenesAssetBundle, assetsAssetBundle,
                args.progressReceiver,
                _contentPack);

            yield break;
        }

        // copied from Remote Village
        public static void SetupMusic()
        {

            var mainCustomTrack = ScriptableObject.CreateInstance<SoundAPI.Music.CustomMusicTrackDef>();
            (mainCustomTrack as ScriptableObject).name = "DesolateCoastMainMusic";
            mainCustomTrack.cachedName = "DesolateCoastMainMusic";
            mainCustomTrack.comment = "\r\nCane B - Castle of Memories\r\ncoast_wormsworms";
            mainCustomTrack.CustomStates = new List<SoundAPI.Music.CustomMusicTrackDef.CustomState>();

            var cstate1 = new SoundAPI.Music.CustomMusicTrackDef.CustomState();
            cstate1.GroupId = 1999820196U; // gameplay song choice group - gathered from the MOD's Init bank txt file
            cstate1.StateId = 369072990U; // stage music
            mainCustomTrack.CustomStates.Add(cstate1);

            var cstate2 = new SoundAPI.Music.CustomMusicTrackDef.CustomState();
            cstate2.GroupId = 792781730U; // gathered from the GAME's Init bank txt file
            cstate2.StateId = 89505537U; // gathered from the GAME's Init bank txt file
            mainCustomTrack.CustomStates.Add(cstate2);

            DesolateCoastContent.coastSceneDef.mainTrack = mainCustomTrack;
            DesolateCoastContent.simuSceneDef.mainTrack = mainCustomTrack;

            /*
            var bossCustomTrack = ScriptableObject.CreateInstance<SoundAPI.Music.CustomMusicTrackDef>();
            (bossCustomTrack as ScriptableObject).name = "STWormsBossMusic";
            bossCustomTrack.cachedName = "STWormsBossMusic";
            bossCustomTrack.comment = "Chris Christodoulou - Hailstorm\r\nnsunkentomb_wormsworms";
            bossCustomTrack.CustomStates = new List<SoundAPI.Music.CustomMusicTrackDef.CustomState>();

            var cstate11 = new SoundAPI.Music.CustomMusicTrackDef.CustomState();
            cstate11.GroupId = 1810963621U; // gathered from the MOD's Init bank txt file
            cstate11.StateId = 514064485U; // Hailstorm
            bossCustomTrack.CustomStates.Add(cstate11);

            var cstate12 = new SoundAPI.Music.CustomMusicTrackDef.CustomState();
            cstate12.GroupId = 792781730U; // gathered from the GAME's Init bank txt file
            cstate12.StateId = 580146960U; // gathered from the GAME's Init bank txt file
            bossCustomTrack.CustomStates.Add(cstate12);

            PromenadeContent.PromenadeSceneDef.bossTrack = bossCustomTrack;
            */
        }

        public static void LoadSoundBanks(string soundbanksFolderPath)
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

            akResult = AkSoundEngine.LoadBank(InitSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {InitSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {InitSoundBankFileName} " +
                    $"Error code : {akResult}");
            }

            akResult = AkSoundEngine.LoadBank(MusicSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {MusicSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {MusicSoundBankFileName} " +
                    $"Error code : {akResult}");
            }
            /*
            akResult = AkSoundEngine.LoadBank(SoundsSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {SoundsSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {SoundsSoundBankFileName} " +
                    $"Error code : {akResult}");
            }
            */
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

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(_contentPack, args.output);

            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}