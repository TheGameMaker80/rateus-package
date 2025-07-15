using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Google.Play.Review;
using System.Threading.Tasks;
using Google.Play.Common;

namespace alexnikolaou.RateUs
{
    public class RateUsManager
    {
        #if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private extern void RequestReview();
        #endif

        public RateUsConfigHandler configHandler = null;
        private static RateUsManager _instance;
        private RateUsConfig _config;
        private bool _initialized = false;

        private int _session;
        private int _sessionWins;
        private int _sessionTimesShown;

        private VersionRates _versionRates;

        //Create instance of ReviewManager(Android)
        private ReviewManager _reviewManager;

        public bool Initialized
        {
            get
            {
                return _initialized;
            }
        }

        public static RateUsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RateUsManager();

                return _instance;
            }
        }

        private RateUsManager() { }

        public void PrintValues()
        {
            Debug.Log("Session: " + _session);
            Debug.Log("Session Wins: " + _sessionWins);
            Debug.Log("Session Times Shown: " + _sessionTimesShown);
            Debug.Log("App Version Rates: " + PlayerPrefs.GetString("rate_us_app_version"));

        }

        private void SetSession()
        {
            //If first session ever set it to 0
            _session = PlayerPrefs.GetInt("rate_us_session");
            //Increment session by 1
            _session++;
            PlayerPrefs.SetInt("rate_us_session", _session);
            //Set Wins to 0
            _sessionWins = 0;
            //Set timesShown to 0;
            _sessionTimesShown = 0;
        }

        private void AddViewToVersion()
        {
            _versionRates.rates++;

            PlayerPrefs.SetString("rate_us_app_version", JsonUtility.ToJson(_versionRates));
        }

        private void SetVersionRates()
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("rate_us_app_version")))
            {
                PlayerPrefs.SetString("rate_us_app_version", JsonUtility.ToJson(new VersionRates(Application.version, 0)));
            }

            string appVersionRates = PlayerPrefs.GetString("rate_us_app_version");
            _versionRates = JsonUtility.FromJson<VersionRates>(appVersionRates);

            if (string.Compare(_versionRates.version, Application.version) != 0)//versions don't match
            {
                _versionRates.version = Application.version;
                _versionRates.rates = 0;
                PlayerPrefs.SetString("rate_us_app_version", JsonUtility.ToJson(_versionRates));
            }
        }

        public void Initialize(Action<bool, string> onComplete)
        {
            //Android
            _reviewManager = new ReviewManager();

            SetSession();
            SetVersionRates();

            //User default if none found
            if (configHandler == null)
            {
                configHandler = new RateUsConfigHandler();
            }

            _initialized = false;

            configHandler.GetConfig((success, msg, config) =>
            {
                _initialized = success;
                _config = config;
                onComplete.Invoke(success, msg);
            });
        }

        public void CheckForShowingRateUsOnWin(Action<bool> onComplete)
        {
            bool showRateUs = false;

            if (_versionRates.rates < _config.rateMaxTimesShownInVersion)
            {
                if (_sessionTimesShown < _config.rateMaxTimesShownInSession)
                {
                    int sessionWins = _session == 1 ? _config.rateFirstSessionWins : _config.rateNotFirstSessionWinsMod;

                    _sessionWins++;

                    if (_sessionWins >= sessionWins)
                    {
                        _sessionTimesShown++;
                        showRateUs = true;
                        _sessionWins = 0;
                        AddViewToVersion();
                    }
                }
            }

            onComplete.Invoke(showRateUs);
        }

        public async Task<TaskResult> NewCheckForShowingRateUsOnWin()
        {
            TaskResult result = new TaskResult(false, "Rate us not available");

            if (_versionRates.rates < _config.rateMaxTimesShownInVersion)
            {
                if (_sessionTimesShown < _config.rateMaxTimesShownInSession)
                {
                    int sessionWins = _session == 1 ? _config.rateFirstSessionWins : _config.rateNotFirstSessionWinsMod;

                    _sessionWins++;

                    if (_sessionWins >= sessionWins)
                    {
                        #if UNITY_IOS
                        result = await RequestIOSReview();
                        #elif UNITY_ANDROID
                        result = await RequestAndroidReview();
                        #endif
                        _sessionTimesShown++;
                        _sessionWins = 0;
                        AddViewToVersion();
                    }
                }
            }
            return result;
        }

        private Task<TaskResult> RequestIOSReview()
        {        
            var tcs = new TaskCompletionSource<TaskResult>();

            #if UNITY_IOS && !UNITY_EDITOR
            RequestReview();
            #endif

            tcs.SetResult(new TaskResult(true, "iOS: In-App Review requested!"));
            return tcs.Task;
        }

        private async Task<TaskResult> RequestAndroidReview()
        {
            var requestFlowOperation = _reviewManager.RequestReviewFlow();

            await AwaitPlayCoreTask(requestFlowOperation);

            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                return new TaskResult(false, $"Android: RequestReviewFlow failed: {requestFlowOperation.Error}");
            }

            PlayReviewInfo playReviewInfo = requestFlowOperation.GetResult();

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(playReviewInfo);

            await AwaitPlayCoreTask(launchFlowOperation);

            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                return new TaskResult(false, $"Android: LaunchReviewFlow failed: {launchFlowOperation.Error}");
            }

            return new TaskResult(true, "Android: In-app review shown successfully.");
        }

        // Helper to await Play Core operations
        private Task AwaitPlayCoreTask<T>(PlayAsyncOperation<T, ReviewErrorCode> operation)
        {
            var tcs = new TaskCompletionSource<object>();
            operation.Completed += _ => tcs.SetResult(null);
            return tcs.Task;
        }
    }

    public struct TaskResult
    {
        public bool succeed;
        public string msg;

        public TaskResult(bool success, string message)
        {
            succeed = success;
            msg = message;
        }
    }
}
