using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageButton : MonoBehaviour
{
    public void ChangeLanguage()
    {
        int index = 0;
        bool found = false;
        while(index < LocalizationSettings.AvailableLocales.Locales.Count && !found)
        {
            if (LocalizationSettings.AvailableLocales.Locales[index] != LocalizationSettings.SelectedLocale)
            {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
                found = true;
            }
            index++;
        }
        FindObjectOfType<AssetLocalizer>().LoadAsset();
    }
}
