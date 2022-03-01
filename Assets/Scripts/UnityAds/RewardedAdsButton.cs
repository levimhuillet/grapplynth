﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using Grapplynth;

public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener {
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOsAdUnitId = "Rewarded_iOS";
    string _adUnitId;

    void Awake() {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;

        //Disable button until ad is ready to show
        _showAdButton.interactable = true;
        _showAdButton.onClick.AddListener(HandleNoAds);
    }

    void Start() {
        LoadAd();
    }

    // Load content to the Ad Unit:
    public void LoadAd() {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        //Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId) {
        //Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId)) {
            _showAdButton.onClick.RemoveListener(HandleNoAds);
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            _showAdButton.interactable = true;
        }
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd() {
        // Disable the button: 
        // _showAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);

        _showAdButton.onClick.RemoveListener(ShowAd);
        _showAdButton.onClick.AddListener(HandleNoAds);
    }

    // Implement a method to execute when the user clicks the button.
    public void HandleNoAds() {
        EventManager.OnNoAds.Invoke();
        //m_noAdsMenu.SetActive(true);

        // Disable the button: 
        _showAdButton.interactable = false;
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED)) {
            //Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            EventManager.OnAdReward.Invoke();
            // m_adsRewardMenu.SetActive(true);
            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }

        if (showCompletionState.Equals(UnityAdsShowCompletionState.SKIPPED)) {
            //Debug.Log("You skipped the ad");
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message) {
        //Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message) {
        //Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) {
        //Debug.Log("Starting advertisement");
    }
    public void OnUnityAdsShowClick(string adUnitId) {
        //Debug.Log("Advertismenet clicked");
    }

    void OnDestroy() {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }
}