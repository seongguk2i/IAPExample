using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public const string ProductHeart = "heart";//Consumable
    public const string ProductCharacterSkin = "character_skin";//UnConsumable
    public const string ProductSubscription = "premium_subscription";//Subscription

    private const string _iOS_HeartId = "com.studio.app.heart";
    private const string _android_HeartId = "com.studio.app.heart";

    private const string _iOS_SkinId = "com.studio.app.skin";
    private const string _android_SkinId = "com.studio.app.skin";

    private const string _iOS_PremiumSub = "com.studio.app.sub";
    private const string _android_PremiumSub = "com.studio.app.sub";

    private static IAPManager m_instance;

    public static IAPManager Instance
    {
        get
        {
            if (m_instance != null) return m_instance;

            m_instance = FindObjectOfType<IAPManager>();

            if (m_instance == null) m_instance = new GameObject("IAP Manager").AddComponent<IAPManager>();
            return m_instance;
        }
    }

    private IStoreController storeController; // ���� ������ �����ϴ� �Լ� ����
    private IExtensionProvider storeExtensionProvider; // ���� �÷����� ���� Ȯ�� ó���� ����

    public bool isInitialized => storeController != null && storeExtensionProvider != null;
    void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        InitUnityIAP();
    }

    private void InitUnityIAP()
    {
        if (isInitialized) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance()); // ����Ƽ �⺻ ����� ���� ������ ����
        builder.AddProduct(
                ProductHeart, ProductType.Consumable,
                new IDs()
                {
                    {_iOS_HeartId, AppleAppStore.Name },
                    {_android_HeartId, GooglePlay.Name }
                }
            );
        builder.AddProduct(
                ProductCharacterSkin, ProductType.NonConsumable,
                new IDs()
                {
                    {_iOS_SkinId, AppleAppStore.Name },
                    {_android_SkinId, GooglePlay.Name }
                }
            );
        builder.AddProduct(
                ProductSubscription, ProductType.Subscription,
                new IDs()
                {
                    {_iOS_PremiumSub, AppleAppStore.Name },
                    {_android_PremiumSub, GooglePlay.Name }
                }
            );
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        //UnityPurchasing.Initialize(this, builder); ���� �� ����
        Debug.Log("����Ƽ IAP �ʱ�ȭ ����");
        storeController = controller;
        storeExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"����Ƽ IAP �ʱ�ȭ ���� {error}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"���� ���� - ID : {args.purchasedProduct.definition.id}");
        if(args.purchasedProduct.definition.id == ProductHeart)
        {
            Debug.Log("��� ��� ó��...");
        }
        else if (args.purchasedProduct.definition.id == ProductCharacterSkin)
        {
            Debug.Log("��Ų ���...");
        }
        else if (args.purchasedProduct.definition.id == ProductSubscription)
        {
            Debug.Log("���� ���� ����...");
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogWarning($"���� ���� - {product.definition.id}, {reason}");
    }

    /// <summary>
    /// ��ǰ ���� �õ�
    /// </summary>
    public void Purchase(string productId)
    {
        if (!isInitialized) return;
        var product = storeController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"���� �õ� - {product.definition.id}");
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.Log($"���� �õ� �Ұ� - {productId}"); 
        }
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void RestorePurchase()
    {
        if (!isInitialized) return;
        if(Application.platform == RuntimePlatform.IPhonePlayer 
            || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("���� ���� �õ�");
            
            var appleExt = storeExtensionProvider.GetExtension<IAppleExtensions>();
            appleExt.RestoreTransactions(
                    result => Debug.Log($"���� ���� �õ� ��� - {result}")
                ); ;
        }
    }

    /// <summary>
    /// ���� �ϰ� �ִ� ������ ���� 
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    public bool HadPurchased(string productId)
    {
        if (!isInitialized) return false;

        var product = storeController.products.WithID(productId);
        if( product != null)
        {
            return product.hasReceipt;
        }

        return false;
    }
}
