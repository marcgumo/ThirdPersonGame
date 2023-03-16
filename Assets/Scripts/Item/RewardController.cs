using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardController : MonoBehaviour, iTakeItem
{
    void iTakeItem.TakeItem()
    {
        GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>().UpdateTotalPuzzles();
        Destroy(gameObject);
    }
}
