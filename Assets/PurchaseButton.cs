using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseButton : MonoBehaviour
{
    public string targetProductId;

    public void HandleClick()
    {
        if(targetProductId == IAPManager.ProductCharacterSkin
                || targetProductId == IAPManager.ProductSubscription)
        {
            if(IAPManager.Instance.HadPurchased(targetProductId)){
                Debug.Log("�̹� ������ ��ǰ");
                return;
            }
        }
        //IAPManager.Instance.RestorePurchase();
        IAPManager.Instance.Purchase(targetProductId);
    }
}
