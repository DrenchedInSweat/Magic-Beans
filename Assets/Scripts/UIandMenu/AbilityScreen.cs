using Characters;
using Characters.BaseStats;
using Characters.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UIandMenu
{
    public class AbilityScreen : MonoBehaviour
    {

        [SerializeField] private GameObject selections;
    
        public void Init(Player component, UpgradeBaseSo[] generateUpgradeSet, int len)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            Transform parent = transform.GetChild(0);
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Select an Upgrade";
            for (int i = 0; i < len; i++)
            {
                Transform host = Instantiate(selections, parent).transform;
                UpgradeBaseSo upgrade = generateUpgradeSet[i];
                
                host.GetChild(0).GetChild(0).GetComponent<Image>().sprite = upgrade.Sprite;
                host.GetChild(1).GetComponent<TextMeshProUGUI>().text = upgrade.Name;
                host.GetChild(2).GetComponent<TextMeshProUGUI>().text = upgrade.Description;
                //Please god don't lose information doing this
                host.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (upgrade is WeaponUpgradeSo s)
                    {
                        if (s.ApplyToAll)
                        {
                            component.UpgradeAttackCharacter(s, (int)s.MyApplicableWeapons); // Apply to affected weapon... 
                        }
                        else
                        {
                            SelectWeapon(component, s);
                            return;
                        }
                    }
                    else
                    {
                        component.UpgradeCharacter((CharacterUpgradeSo)upgrade);
                    }

                    ResetGame();
                });
            }
        }

        private void ResetGame()
        {
            GameManager.Instance.ToggleStop();
            Destroy(gameObject);
        }

        private void SelectWeapon(Player p, WeaponUpgradeSo upgrade)
        {
            Transform parent = transform.GetChild(0);
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Select a Weapon to Upgrade";
            int num = (int)upgrade.MyApplicableWeapons;

            for (int i = 0; i < parent.childCount; ++i)
            {
                Destroy(parent.GetChild(i).gameObject);
            }


            foreach (Weapon w in p.ShootingCapability.Weapons)
            {
                WeaponStatsSo s = w.GetStats<WeaponStatsSo>();
                if(((int)s.WeaponType & num) == 0) continue;
                Transform host = Instantiate(selections, parent).transform;
                
                host.GetComponent<Button>().onClick.AddListener(() =>
                {
                    w.Upgrade(upgrade);
                    ResetGame();
                });
                
                host.GetChild(0).GetChild(0).GetComponent<Image>().sprite = s.Sprite;
                host.GetChild(1).GetComponent<TextMeshProUGUI>().text = s.Name;
                host.GetChild(2).GetComponent<TextMeshProUGUI>().text = s.Description;
            }
        }
    }
}

