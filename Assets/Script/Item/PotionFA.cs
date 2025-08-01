using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionFA : MonoBehaviour
{
    public Image potionFA;

    private void Update()
    {
        if (potionFA.fillAmount != 0) {
            potionFA.fillAmount -= 0.01f;
        }
    }
}
