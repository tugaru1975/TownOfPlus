using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TownOfPlus
{
    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Awake))]
    public class ChangeNameBox
    {
        public static void Postfix(CreateOptionsPicker __instance)
        {
            if (!main.ChangeNameBox.Getbool()) return;

            var scene = SceneManager.GetActiveScene().name;
            if (scene is not "MMOnline" and not "FindAGame") return;
            var editName = DestroyableSingleton<AccountManager>.Instance.accountTab.editNameScreen;
            var nameText = Object.Instantiate(editName.nameText.gameObject, __instance.transform);

            if (scene is "MMOnline")
            {
                nameText.transform.SetPos(y: -2.625f, z: -20);
            }
            if (scene is "FindAGame")
            {
                nameText.transform.SetPos(y: -2.925f, z: -20);
                nameText.transform.localScale *= 0.8f;
            }

            var textBox = nameText.GetComponent<TextBoxTMP>();
            textBox.outputText.alignment = TextAlignmentOptions.CenterGeoAligned;
            textBox.outputText.transform.position = nameText.transform.position;
            textBox.outputText.fontSize = 4f;

            textBox.OnChange.AddListener((Action)(() =>
            {
                SaveManager.PlayerName = textBox.text;
            }));

            textBox.OnEnter = textBox.OnFocusLost = textBox.OnChange;

            textBox.Pipe.GetComponent<TextMeshPro>().fontSize = 4f;
        }
    }

    [HarmonyPatch(typeof(SinglePopHelp), nameof(SinglePopHelp.OnEnable))]
    public class ChangeNameBoxJoin
    {
        public static GameObject nameText;
        public static void Postfix(SinglePopHelp __instance)
        {
            if (!main.ChangeNameBox.Getbool()) return;
            if (nameText != null || !SceneManager.GetActiveScene().name.Equals("MMOnline")) return;
            var editName = DestroyableSingleton<AccountManager>.Instance.accountTab.editNameScreen;
            nameText = Object.Instantiate(editName.nameText.gameObject, __instance.transform);

            nameText.transform.SetPos(y: -1.15f, z: -5);

            var textBox = nameText.GetComponent<TextBoxTMP>();
            textBox.outputText.alignment = TextAlignmentOptions.CenterGeoAligned;
            textBox.outputText.transform.position = nameText.transform.position;
            textBox.outputText.fontSize = 4f;

            textBox.OnChange.AddListener((Action)(() =>
            {
                SaveManager.PlayerName = textBox.text;
            }));

            textBox.OnEnter = textBox.OnFocusLost = textBox.OnChange;

            textBox.Pipe.GetComponent<TextMeshPro>().fontSize = 4f;
        }
    }
}
