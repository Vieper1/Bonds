using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace CompleteProject {
	public class Purchaser : MonoBehaviour, IStoreListener {
		private static IStoreController m_StoreController;          // The Unity Purchasing system.
		private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

		public static string kProductRemoveAds = "com.vishanstudios.bonds.remove_ads";
		//public static string kProductRemoveAds = "remove_ads";

		void Start() {
			if (m_StoreController == null) {
				InitializePurchasing();
			}
		}

		public void InitializePurchasing() {
			if (IsInitialized()) {
				return;
			}

			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			builder.AddProduct(kProductRemoveAds, ProductType.NonConsumable);

			UnityPurchasing.Initialize(this, builder);
			Debug.LogWarning("Purchase Init Called!");
		}


		private bool IsInitialized() {
			return m_StoreController != null && m_StoreExtensionProvider != null;
		}


		public void BuyRemoveAds() {
			BuyProductID(kProductRemoveAds);
		}


		void BuyProductID(string productId) {
			if (IsInitialized()) {
				Product product = m_StoreController.products.WithID(productId);

				if (product != null && product.availableToPurchase) {
					Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
					m_StoreController.InitiatePurchase(product);
				} else {    // Lookup failure 
					Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			} else {
				Debug.Log("BuyProductID FAIL. Not initialized.");
			}
		}



		public void RestorePurchases() {
			if (!IsInitialized()) {
				Debug.Log("RestorePurchases FAIL. Not initialized.");
				return;
			}

			if (Application.platform == RuntimePlatform.IPhonePlayer ||
				Application.platform == RuntimePlatform.OSXPlayer) {
				Debug.Log("RestorePurchases started ...");

				var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
				apple.RestoreTransactions((result) => {
					Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
				});
			} else {    // Not on Apple Device
				Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}


		//  
		// --- IStoreListener
		//

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
			Debug.Log("OnInitialized: PASS");
			m_StoreController = controller;
			m_StoreExtensionProvider = extensions;

			// Check for prev purchases
			if (m_StoreController.products.WithID("com.vishanstudios.bonds.remove_ads").hasReceipt) {
				PlayerPrefs.SetInt("RemoveAds", 1911);
				transform.Find("RemoveAdsButton").GetComponent<Button>().interactable = false;
			}
		}


		public void OnInitializeFailed(InitializationFailureReason error) {
			Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
		}


		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
			if (String.Equals(args.purchasedProduct.definition.id, kProductRemoveAds, StringComparison.Ordinal)) {
				Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
				PlayerPrefs.SetInt("RemoveAds", 1911);
				transform.Find("RemoveAdsButton").GetComponent<Button>().interactable = false;
			} else {
				Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
			}

			return PurchaseProcessingResult.Complete;
		}


		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
			Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
		}
	}
}