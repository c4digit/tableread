using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
// one of the existing Survival Shooter scripts.
	// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : MonoBehaviour, IStoreListener
{

//public Text	_buttonTXT;

		private static IStoreController m_StoreController;          // The Unity Purchasing system.
		private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

		// Product identifiers for all products capable of being purchased: 
		// "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
		// counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
		// also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

		// General product identifiers for the consumable, non-consumable, and subscription products.
		// Use these handles in the code to reference which product to purchase. Also use these values 
		// when defining the Product Identifiers on the store. Except, for illustration purposes, the 
		// kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
		// specific mapping to Unity Purchasing's AddProduct, below.
//	public static string kProductIDConsumable =    "consumable";   
//	public static string kDevonianNonConsumable = "devonian_full";
//	public static string kPompeiinNonConsumable = "pompeii_full";
	public static string kProductIDAndroidSubscription =  "com.android.tableread.tablereadgopro"; 
	public static string kProductIDiOSSubscription = "com.gmail.tablereadtech.tableread.tablereadGoPro";

		// Apple App Store-specific product identifier for the subscription product.
	//private static string kDevonianAppleNonConsumable=  "devonian_full";

	// Google Play Store-specific product identifier subscription product.
//	public List<string> MoviesGooglePlayNonConsumable = new List<string>();
//	public static List<string> kMoviesGooglePlayNonConsumable = new List<string>();
	//private static string kDevonianGooglePlayNonConsumable =  "devonian_full"; 
	//private static string kPompeiiGooglePlayNonConsumable =  "pompeii_full";

	public void getIAPData()
	{
			// If we haven't set up the Unity Purchasing reference
		if (m_StoreController == null) {
			// Begin to configure our connection to Purchasing
			InitializePurchasing ();
		} else {
			updateDetails ();
		}
	}

	void Start () {
		getIAPData ();
	}

	/*public static void clearCosumables() {
		kMoviesGooglePlayNonConsumable.Clear ();
	}

	public static void addConsumable(string s) {
	//	MoviesGooglePlayNonConsumable.Add (s);
		kMoviesGooglePlayNonConsumable.Add (s);
		for (int i = 0; i < kMoviesGooglePlayNonConsumable.Count; i++) {
			trglobals.instance.DebugLog("kMoviesGooglePlayNonConsumable " + kMoviesGooglePlayNonConsumable[i]);
		}
	}*/

	void updateDetails() {
	//	string m = "";
	//	for (int i = 0; i < m_StoreController.products.all.Length; i++) {
		//	m += m_StoreController.products.all [i].definition.storeSpecificId + m_StoreController.products.all [i].metadata.localizedPriceString + "\n";
		//	trglobals.instance.DebugLog("updateDetails " + m_StoreController.products.all [i].availableToPurchase + ":" +
			//	m_StoreController.products.all [0].hasReceipt + ":" + m_StoreController.products.all [0].receipt);

		//	menuManager.instance.updateMoviePrice (m_StoreController.products.all [i].definition.storeSpecificId,
			//	m_StoreController.products.all [i].metadata.localizedPriceString,m_StoreController.products.all[i].hasReceipt);
	//	}
	//	trglobals.instance.DebugLog("PURCHASER updateDetails " + m_StoreController.products.all [0].metadata);
		if (m_StoreController.products.all [0].availableToPurchase)
			trglobals.instance._trsub._subscGO.SetActive (true);
		else
			trglobals.instance._trsub._subscGO.SetActive (false);
		if (m_StoreController.products.all [0].hasReceipt) {
			PlayerPrefs.SetInt ("tablreadhaspro", 1);
			trglobals.instance.DebugLog ("Pirchaser turnOnPro");
			trglobals.instance._trsub.turnOnPro ();
		} else {
			PlayerPrefs.SetInt ("tablreadhaspro", 0);
			trglobals.instance.DebugLog ("Pirchaser turnOffPro");
			trglobals.instance._trsub.turnOffPro ();
		}
		PlayerPrefs.Save ();
		trglobals.instance._trsub._subscTXT.text = m_StoreController.products.all [0].metadata.localizedTitle + " " + m_StoreController.products.all [0].metadata.localizedPriceString;
		//trglobals.instance._trfp.randomvoice ();
	}

	public void InitializePurchasing() 
	{
			// If we have already connected to Purchasing ...
		if (IsInitialized())
		{
			updateDetails ();
				// ... we are done here.
			return;
		}

			// Create a builder, first passing in a suite of Unity provided stores.
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		#if UNITY_ANDROID
		builder.AddProduct(kProductIDAndroidSubscription, ProductType.Subscription, new IDs(){
			{ kProductIDAndroidSubscription, GooglePlay.Name },
		});
		#endif
		#if UNITY_IOS
		builder.AddProduct(kProductIDiOSSubscription, ProductType.Subscription, new IDs(){
			{ kProductIDiOSSubscription, AppleAppStore.Name },
		});
		#endif
	//	trglobals.instance.DebugLog ("InitializePurchasing kMoviesGooglePlayNonConsumable COUNT is " + kMoviesGooglePlayNonConsumable.Count);
	//	for (int i = 0; i < kMoviesGooglePlayNonConsumable.Count; i++) {
			//trglobals.instance.DebugLog ("ADDING " + kMoviesGooglePlayNonConsumable [i]);
			//builder.AddProduct(kMoviesGooglePlayNonConsumable[i], ProductType.NonConsumable, new IDs(){
			//	{ kMoviesGooglePlayNonConsumable[i], GooglePlay.Name },
			//});
		//}

		/*builder.AddProduct(kDevonianNonConsumable, ProductType.NonConsumable, new IDs(){
			{ kDevonianGooglePlayNonConsumable, GooglePlay.Name },
		});
		builder.AddProduct(kPompeiinNonConsumable, ProductType.NonConsumable, new IDs(){
			{ kPompeiiGooglePlayNonConsumable, GooglePlay.Name },
		});(*/
			UnityPurchasing.Initialize(this, builder);
	}


		private bool IsInitialized()
		{
			// Only say we are initialized if both the Purchasing references are set.
			return m_StoreController != null && m_StoreExtensionProvider != null;
		}

	public void BuyPRO()
	{
		#if UNITY_ANDROID
		trglobals.instance.DebugLog ("BuyPRO " + kProductIDAndroidSubscription);
		BuyProductID(kProductIDAndroidSubscription);
		#endif
		#if UNITY_IOS
		trglobals.instance.DebugLog ("BuyPRO IOS" + kProductIDiOSSubscription);
		BuyProductID(kProductIDiOSSubscription);
		#endif
	}


		void BuyProductID(string productId)
		{
			// If Purchasing has been initialized ...
			if (IsInitialized())
			{
				// ... look up the Product reference with the general product identifier and the Purchasing 
				// system's products collection.
				Product product = m_StoreController.products.WithID(productId);

				// If the look up found a product for this device's store and that product is ready to be sold ... 
				if (product != null && product.availableToPurchase)
				{
				//	trglobals.instance.DebugLog(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
					// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
					// asynchronously.
					m_StoreController.InitiatePurchase(product);
				}
				// Otherwise ...
				else
				{
					// ... report the product look-up failure situation  
					trglobals.instance.DebugLog("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			// Otherwise ...
			else
			{
				// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
				// retrying initiailization.
				trglobals.instance.DebugLog("BuyProductID FAIL. Not initialized.");
			trglobals.instance.ShowError ("Failed to initialize Subscription product.\nPlease check you internet connection.");
			}
		}


		// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
		// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
		public void RestorePurchases()
		{
			// If Purchasing has not yet been set up ...
			if (!IsInitialized())
			{
				// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
				trglobals.instance.DebugLog("RestorePurchases FAIL. Not initialized.");
				trglobals.instance.ShowError ("Failed to initialize Subscription product.\nPlease check you internet connection.");
				return;
			}

			// If we are running on an Apple device ... 
			if (Application.platform == RuntimePlatform.IPhonePlayer || 
				Application.platform == RuntimePlatform.OSXPlayer)
			{
				// ... begin restoring purchases
				trglobals.instance.DebugLog("RestorePurchases started ...");

				// Fetch the Apple store-specific subsystem.
				var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
				// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
				// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
				apple.RestoreTransactions((result) => {
					// The first phase of restoration. If no more responses are received on ProcessPurchase then 
					// no purchases are available to be restored.
					trglobals.instance.DebugLog("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
				});
			}
			// Otherwise ...
			else
			{
				// We are not running on an Apple device. No work is necessary to restore purchases.
				trglobals.instance.DebugLog("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
				trglobals.instance.ShowError ("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}

	//  
	// --- IStoreListener
	//

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
			// Purchasing has succeeded initializing. Collect our Purchasing references.
	//	trglobals.instance.DebugLog("OnInitialized: PASS");

			// Overall Purchasing system, configured with products for this application.
		m_StoreController = controller;
			// Store specific subsystem, for accessing device-specific store features.
		m_StoreExtensionProvider = extensions;
		if (m_StoreController == null) {
			Debug.Log ("ERRROR IN HERE OnInitialized");
			trglobals.instance.DebugLog ("OnInitialized: WTF");
	//		_buttonTXT.text = "m_StoreController is NULL";
		} else {
			updateDetails ();
		}
	}


	public void OnInitializeFailed(InitializationFailureReason error) {
			// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
		trglobals.instance.DebugLog("OnInitializeFailed InitializationFailureReason:" + error);
		trglobals.instance.ShowError ("Please try again later to subscribe to\ntableread Pro.");
	//	_buttonTXT.text = error.ToString();
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) 
	{
		trglobals.instance.DebugLog ("PurchaseProcessingResult CHECKING" + kProductIDAndroidSubscription);
		if (String.Equals(args.purchasedProduct.definition.id, kProductIDAndroidSubscription, StringComparison.Ordinal)) {
			trglobals.instance.DebugLog(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
			updateDetails ();
		}
		if (String.Equals(args.purchasedProduct.definition.id, kProductIDiOSSubscription, StringComparison.Ordinal)) {
			trglobals.instance.DebugLog(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
			updateDetails ();
		}
		return PurchaseProcessingResult.Complete;
	}


		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
			// this reason with the user to guide their troubleshooting actions.
			trglobals.instance.DebugLog(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
			trglobals.instance.ShowError ("Please try again later to subscribe to\ntableread Pro.","PURCHASE ERROR");
		}
	}
