using UnityEngine;
using UnityEngine.UI;

public class PlayerUISystem : MonoBehaviour
{
    [SerializeField] private Image staminaFillamount;
    [SerializeField] private Image hpFillamount;
    private PlayerStatus status;

    void Start()
    {
        status = PlayerStatus.Instance;
    }
    void Update()
    {
        staminaFillamount.fillAmount = status.curruntStamina / status.maxStamina;
        hpFillamount.fillAmount = status.curruntHP / status.maxHp;
    }
}
