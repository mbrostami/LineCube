using UnityEngine;
// using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class Menu : MonoBehaviour 
{
    public static bool GoogleServicesChecked = false;
    void Awake() 
    {
        if (Menu.GoogleServicesChecked == false) {
            if (!Social.localUser.authenticated) {
                EnableGooglePlayServices();
                Social.localUser.Authenticate((bool success) => {
                    // TODO Make this mandatory 
                });
            }
            Menu.GoogleServicesChecked = true;
        }
        SaveGame.LoadSettings(); // initialize settings
        SaveGame.LoadLevel();
        if (TilemapHandler.initialized == false) {
            new TilemapHandler(); // initialize tilemaphandler static instance
        }
    }

    private void EnableGooglePlayServices()
    {
        // PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //     // enables saving game progress.
        //     // .EnableSavedGames()
        //     // registers a callback to handle game invitations received while the game is not running.
        //     // .WithInvitationDelegate(<callback method>)
        //     // registers a callback for turn based match notifications received while the
        //     // game is not running.
        //     // .WithMatchDelegate(<callback method>)
        //     // requests the email address of the player be available.
        //     // Will bring up a prompt for consent.
        //     // .RequestEmail()
        //     // requests a server auth code be generated so it can be passed to an
        //     //  associated back end server application and exchanged for an OAuth token.
        //     // .RequestServerAuthCode(false)
        //     // requests an ID token be generated.  This OAuth token can be used to
        //     //  identify the player to other services such as Firebase.
        //     // .RequestIdToken()
        //     .Build();

        // PlayGamesPlatform.InitializeInstance(config);
        // // recommended for debugging:
        // // PlayGamesPlatform.DebugLogEnabled = true;
        // // Activate the Google Play Games platform
        // PlayGamesPlatform.Activate();
    }
}