using System.Collections;
using System.Collections.Generic;
using BaseTranslationMod;
using UCDC_Mod_Api.Models.Localization;
using UCDC_Mod_Api.ModInterfaces;
using UMod;
using UnityEngine;

public class TranslationOverride : ModScript, ITranslationsOverrider
{
    private TextAsset menuCsv;
    private TextAsset shopCsv;
    private TextAsset tutorialCsv;
    private TextAsset virusOffenseUiCsv;
    private TextAsset virusOffenseCsv;
    
    public TranslationInfo GetTranslations()
    {
        TranslationInfo translationInfo = new TranslationInfo()
        {
            LoadingMessage = "Loading Test Translations...",
            Translations = new TranslationTable()
        };
        
        menuCsv = ModAssets.Load<TextAsset>("MainUI");
        shopCsv = ModAssets.Load<TextAsset>("Shop");
        tutorialCsv = ModAssets.Load<TextAsset>("Tutorial");
        virusOffenseUiCsv = ModAssets.Load<TextAsset>("VirusOffenseUI");
        virusOffenseCsv = ModAssets.Load<TextAsset>("VirusOffense");

        translationInfo.Translations.MainUi = TranslationsLoader.LoadCsvToDictionary(menuCsv);
        translationInfo.Translations.Shop = TranslationsLoader.LoadCsvToDictionary(shopCsv);
        translationInfo.Translations.Tutorial = TranslationsLoader.LoadCsvToDictionary(tutorialCsv);
        translationInfo.Translations.VirusOffenseUi = TranslationsLoader.LoadCsvToDictionary(virusOffenseUiCsv);
        translationInfo.Translations.VirusOffenseDialogues = TranslationsLoader.LoadCsvToDictionary(virusOffenseCsv);

        return translationInfo;
    }
}
