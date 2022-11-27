using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("UI")] //[SerializeField] //private Slider slider;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Image healOverlay;
        [SerializeField] private float healOverlayTime = 0.5f;
        private float healOTimer;
        
        [SerializeField] private Image hurtOverlay;
        [SerializeField] private float hurtOverlayTime = 0.5f;
        private float hurtOTimer;

        [Tooltip("Images for each of the weapon slots")]
        [SerializeField] private Image[] weaponSlots;
        [Tooltip("Highlighted colour when weapon slot is selected")]
        [SerializeField] private Color equippedWeaponCol = new Color(1f, 1f, 1f, 1f);
        [Tooltip("Colour for when weapon slot is available but not selected")]
        [SerializeField] private Color unequippedWeaponCol = new Color(0.5f, 0.5f, 0.5f, 1f);
        [Tooltip("Colour for when weapon slot is unavailable")]
        [SerializeField] private Color unavailableWeaponCol = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        [SerializeField] private GameObject deathScreen;
        
        [Tooltip("Images for each of the weapon slots")]
        [SerializeField] private Image[] otherUIElements;

        private float savedMaxHealth;
        private bool oldState = true;

        public void SetUI(bool state)
        {
            //They are the same
            if (oldState ^ state)
                return;
            oldState = state;
            
            //state is on
            if (state)
            {
                foreach (Image i in weaponSlots)
                {
                    i.gameObject.SetActive(true);
                }
                
                foreach (Image i in otherUIElements)
                {
                    i.gameObject.SetActive(true);
                }

                return;
            }
            
            //state is off
            foreach (Image i in weaponSlots)
            {
                StartCoroutine(FadeOverlay(i, 0.4f));
            }
            
            foreach (Image i in otherUIElements)
            {
                StartCoroutine(FadeOverlay(i, 0.4f));
            }
        }

        private IEnumerator FadeOverlay(Image overlay, float time)
        {
            print("Fading UI for " + overlay.gameObject.name);
            overlay.gameObject.SetActive(true);
            overlay.color = new Color(1, 1, 1, 1);
            float ct = time;
            while (ct > 0)
            {
                ct -= Time.deltaTime;
                overlay.color = new Color(1, 1, 1, ct/time);
                yield return null;
            }
            overlay.gameObject.SetActive(false);
        }

        public void SetHealth(float maxHealth, float health)
        {
            savedMaxHealth = maxHealth;
            healthBar.value = Mathf.Clamp(health / maxHealth,0,1);
        }

        public void UpdateHealth(float remainingHealth, float change)
        {
            healthBar.value = Mathf.Clamp(remainingHealth / savedMaxHealth,0,1);
            if (!oldState || change == 0) return;
            StopAllCoroutines();
            if (remainingHealth < 0)
            {
                deathScreen.SetActive(true);
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }
            if (change < 0)
            {
                StartCoroutine(FadeOverlay(healOverlay, healOverlayTime));
                return;
            }
            StartCoroutine(FadeOverlay(hurtOverlay, hurtOverlayTime));
        }

        public void UpdateUpgradeUI()
        {
            
        }

        public void SetCurrentWeapon(int old, int weaponIndex)
        {
            weaponSlots[old].color = unequippedWeaponCol;
            weaponSlots[weaponIndex].color = equippedWeaponCol;
        }

        public void SetWeapon(int idx, Sprite img)
        {
            
        }
    }
}
