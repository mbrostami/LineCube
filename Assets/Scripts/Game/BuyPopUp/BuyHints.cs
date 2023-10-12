using System;
using UnityEngine;
using UnityEngine.UI;

public class BuyHints : MonoBehaviour//, IStoreListener
{
    // private static IStoreController m_StoreController;          // The Unity Purchasing system.
    // private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.
    public static string kProductID5Hints = "buy5hints";   
    public static string kProductID10Hints = "buy10hints";   
    public static string kProductID20Hints = "buy20hints";   
    // public static string kProductIDNonConsumable = "nonconsumable";
    // public static string kProductIDSubscription = "subscription"; 

    // Apple App Store-specific product identifier for the subscription product.
    // private static string kProductNameAppleSubscription =  "com.unity3d.subscription.new";

    // Google Play Store-specific product identifier subscription product.
    // private static string kProductNameGooglePlaySubscription =  "com.unity3d.subscription.original"; 

    public void Buy5Hints()
    {
        BuyConsumable(kProductID5Hints);
    }

    public void Buy10Hints()
    {
        BuyConsumable(kProductID10Hints);
    }
    
    public void Buy20Hints()
    {
        BuyConsumable(kProductID20Hints);
    }

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        // if (m_StoreController == null)
        // {
            // Begin to configure our connection to Purchasing
            // InitializePurchasing();
        // }
    }

    public void InitializePurchasing() 
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        // var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        // builder.AddProduct(kProductID5Hints, ProductType.Consumable);
        // builder.AddProduct(kProductID10Hints, ProductType.Consumable);
        // builder.AddProduct(kProductID20Hints, ProductType.Consumable);
        // Continue adding the non-consumable product.
        // builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 
        // builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
        //     { kProductNameAppleSubscription, AppleAppStore.Name },
        //     { kProductNameGooglePlaySubscription, GooglePlay.Name },
        // });

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        // UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return false;
        // Only say we are initialized if both the Purchasing references are set.
        // return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyConsumable(string productId)
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(productId);
    }


    // public void BuyNonConsumable()
    // {
    //     // Buy the non-consumable product using its general identifier. Expect a response either 
    //     // through ProcessPurchase or OnPurchaseFailed asynchronously.
    //     BuyProductID(kProductIDNonConsumable);
    // }


    // public void BuySubscription()
    // {
    //     // Buy the subscription product using its the general identifier. Expect a response either 
    //     // through ProcessPurchase or OnPurchaseFailed asynchronously.
    //     // Notice how we use the general product identifier in spite of this ID being mapped to
    //     // custom store-specific identifiers above.
    //     BuyProductID(kProductIDSubscription);
    // }


    void BuyProductID(string productId)
    {

    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {

    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized()
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        // Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        // m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        // m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed()
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        // Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public void ProcessPurchase() 
    {

    }

    private void ShowSuccessMessage()
    {
        Text message = GameObject.Find("PaymentMessage").GetComponent<Text>();
        Color successColor = new Color();
        ColorUtility.TryParseHtmlString("#00B014", out successColor);
        message.color = successColor;
        message.text = "Thank you! purchase has been completed!";
    }
    private void ShowFailureMessage()
    {
        Text message = GameObject.Find("PaymentMessage").GetComponent<Text>();
        Color successColor = new Color();
        ColorUtility.TryParseHtmlString("#B01300", out successColor);
        message.color = successColor;
        message.text = "Oh no! purchase failed!";
    }

    public void OnPurchaseFailed()
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        // Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}