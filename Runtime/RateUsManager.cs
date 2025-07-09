using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace alexnikolaou.RateUs
{
    public class RateUsManager
    {
        public RateUsConfigHandler configHandler = null;
        private static RateUsManager _instance;
        private RateUsConfig _config;
        private bool _initialized = false;

        private int _session;
        private int _sessionWins;
        private int _sessionTimesShown;

        private VersionRates _versionRates;

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
            SetSession();
            SetVersionRates();

            //User default if none found
            if (configHandler == null)
            {
                configHandler = new RateUsConfigHandler();
            }

            _initialized = false;

            configHandler.GetConfig((success, msg, config)=>
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
    }
}
