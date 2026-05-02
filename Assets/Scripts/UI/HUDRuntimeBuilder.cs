using Simonshouse.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Simonshouse.UI
{
    public static class HUDRuntimeBuilder
    {
        private const string HudSceneName = "HUD";
        private const string HudRootName = "HUD_Root";
        private static bool initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            SceneManager.sceneLoaded += OnSceneLoaded;

            Scene active = SceneManager.GetActiveScene();
            if (active.name == HudSceneName)
            {
                BuildHud(active);
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != HudSceneName)
            {
                return;
            }

            BuildHud(scene);
        }

        private static void BuildHud(Scene targetScene)
        {
            if (FindInScene(targetScene, HudRootName) != null)
            {
                return;
            }

            GameObject hudRoot = new GameObject(HudRootName);
            SceneManager.MoveGameObjectToScene(hudRoot, targetScene);

            Canvas canvas = hudRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;

            CanvasScaler scaler = hudRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            hudRoot.AddComponent<GraphicRaycaster>();

            GameObject dialogPanel = CreatePanel(hudRoot.transform, "DialogPanel", new Color(0f, 0f, 0f, 0.78f));
            RectTransform dialogRt = dialogPanel.GetComponent<RectTransform>();
            dialogRt.anchorMin = new Vector2(0f, 0f);
            dialogRt.anchorMax = new Vector2(1f, 0f);
            dialogRt.pivot = new Vector2(0.5f, 0f);
            dialogRt.sizeDelta = new Vector2(0f, 250f);
            dialogRt.anchoredPosition = Vector2.zero;

            GameObject characterFrame = CreatePanel(hudRoot.transform, "CharacterFrame", new Color(0.12f, 0.12f, 0.12f, 0.92f));
            RectTransform frameRt = characterFrame.GetComponent<RectTransform>();
            frameRt.anchorMin = new Vector2(0f, 0f);
            frameRt.anchorMax = new Vector2(0f, 0f);
            frameRt.pivot = new Vector2(0f, 0f);
            frameRt.sizeDelta = new Vector2(280f, 360f);
            frameRt.anchoredPosition = new Vector2(30f, 220f);

            GameObject portrait = CreatePanel(characterFrame.transform, "Portrait", new Color(0.30f, 0.30f, 0.30f, 0.60f));
            RectTransform portraitRt = portrait.GetComponent<RectTransform>();
            portraitRt.anchorMin = new Vector2(0f, 0f);
            portraitRt.anchorMax = new Vector2(1f, 1f);
            portraitRt.offsetMin = new Vector2(12f, 12f);
            portraitRt.offsetMax = new Vector2(-12f, -12f);

            GameObject inventoryPanel = CreatePanel(hudRoot.transform, "InventoryPanel", new Color(0f, 0f, 0f, 0.86f));
            RectTransform inventoryRt = inventoryPanel.GetComponent<RectTransform>();
            inventoryRt.anchorMin = new Vector2(0.15f, 0.14f);
            inventoryRt.anchorMax = new Vector2(0.85f, 0.86f);
            inventoryRt.offsetMin = Vector2.zero;
            inventoryRt.offsetMax = Vector2.zero;
            inventoryPanel.SetActive(false);

            CreateLabel(inventoryPanel.transform, "InventoryTitle", "INVENTARIO", 44f, TextAlignmentOptions.Center);
            RectTransform inventoryTitleRt = FindRequiredRect(inventoryPanel.transform, "InventoryTitle");
            inventoryTitleRt.anchorMin = new Vector2(0f, 0.86f);
            inventoryTitleRt.anchorMax = new Vector2(1f, 1f);
            inventoryTitleRt.offsetMin = new Vector2(0f, 0f);
            inventoryTitleRt.offsetMax = new Vector2(0f, -14f);

            GameObject notesButton = CreateButton(hudRoot.transform, "NotesButton", "Notes");
            RectTransform notesRt = notesButton.GetComponent<RectTransform>();
            notesRt.anchorMin = new Vector2(0f, 0f);
            notesRt.anchorMax = new Vector2(0f, 0f);
            notesRt.pivot = new Vector2(0f, 0f);
            notesRt.sizeDelta = new Vector2(210f, 64f);
            notesRt.anchoredPosition = new Vector2(25f, 25f);

            GameObject helpButton = CreateButton(hudRoot.transform, "HelpButton", "Help");
            RectTransform helpRt = helpButton.GetComponent<RectTransform>();
            helpRt.anchorMin = new Vector2(1f, 1f);
            helpRt.anchorMax = new Vector2(1f, 1f);
            helpRt.pivot = new Vector2(1f, 1f);
            helpRt.sizeDelta = new Vector2(210f, 64f);
            helpRt.anchoredPosition = new Vector2(-25f, -25f);

            GameObject inventoryButton = CreateButton(hudRoot.transform, "InventoryButton", "Inventario");
            RectTransform invBtnRt = inventoryButton.GetComponent<RectTransform>();
            invBtnRt.anchorMin = new Vector2(0f, 1f);
            invBtnRt.anchorMax = new Vector2(0f, 1f);
            invBtnRt.pivot = new Vector2(0f, 1f);
            invBtnRt.sizeDelta = new Vector2(260f, 64f);
            invBtnRt.anchoredPosition = new Vector2(25f, -25f);

            GameObject continueButton = CreateButton(dialogPanel.transform, "ContinueButton", "Continuar");
            RectTransform continueRt = continueButton.GetComponent<RectTransform>();
            continueRt.anchorMin = new Vector2(1f, 0f);
            continueRt.anchorMax = new Vector2(1f, 0f);
            continueRt.pivot = new Vector2(1f, 0f);
            continueRt.sizeDelta = new Vector2(220f, 64f);
            continueRt.anchoredPosition = new Vector2(-20f, 20f);

            GameObject dialogText = CreateLabel(dialogPanel.transform, "DialogText", "", 34f, TextAlignmentOptions.TopLeft);
            RectTransform dialogTextRt = dialogText.GetComponent<RectTransform>();
            dialogTextRt.anchorMin = new Vector2(0f, 0f);
            dialogTextRt.anchorMax = new Vector2(1f, 1f);
            dialogTextRt.offsetMin = new Vector2(360f, 26f);
            dialogTextRt.offsetMax = new Vector2(-260f, -24f);

            HUDInventoryToggle inventoryToggle = hudRoot.AddComponent<HUDInventoryToggle>();
            inventoryToggle.SetInventoryRoot(inventoryPanel);
            inventoryButton.GetComponent<Button>().onClick.AddListener(inventoryToggle.ToggleInventory);
        }

        private static GameObject FindInScene(Scene scene, string objectName)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                if (roots[i].name == objectName)
                {
                    return roots[i];
                }
            }

            return null;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            return panel;
        }

        private static GameObject CreateButton(Transform parent, string name, string label)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.15f, 0.15f, 0.15f, 0.88f);

            CreateLabel(buttonObject.transform, "Label", label, 30f, TextAlignmentOptions.Center);
            RectTransform labelRt = FindRequiredRect(buttonObject.transform, "Label");
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;

            return buttonObject;
        }

        private static GameObject CreateLabel(Transform parent, string name, string text, float fontSize, TextAlignmentOptions alignment)
        {
            GameObject label = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            label.transform.SetParent(parent, false);

            TextMeshProUGUI tmp = label.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = alignment;
            tmp.enableWordWrapping = true;

            return label;
        }

        private static RectTransform FindRequiredRect(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            return child.GetComponent<RectTransform>();
        }

    }
}
