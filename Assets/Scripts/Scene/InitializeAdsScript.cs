using UnityEngine;
using UnityEngine.Monetization;

public class InitializeAdsScript : MonoBehaviour { 

    string gameId = "3373368";
    bool testMode = false;

    void Start () {
        Monetization.Initialize (gameId, testMode);
    }
}
