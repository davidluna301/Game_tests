using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simonshouse.UI;

namespace Simonshouse.Editor
{
    public static class PrologueSceneBuilder
    {
        [MenuItem("Simonshouse/Build Prologue Scene (v1.1)")]
        public static void BuildPrologueScene()
        {
            void Stretch(RectTransform rt)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            GameObject CreateImage(Transform parent, string name, Color c, bool raycast = false)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                go.AddComponent<CanvasRenderer>();
                var img = go.AddComponent<Image>();
                img.color = c;
                img.raycastTarget = raycast;
                Stretch(go.GetComponent<RectTransform>());
                return go;
            }

            GameObject CreateTMP(Transform parent, string name, string text, float fontSize, Color color,
                TextAlignmentOptions alignment, bool wrap = true)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                go.AddComponent<CanvasRenderer>();
                var tmp = go.AddComponent<TextMeshProUGUI>();
                tmp.text = text;
                tmp.fontSize = fontSize;
                tmp.color = color;
                tmp.alignment = alignment;
                tmp.enableWordWrapping = wrap;
                tmp.raycastTarget = false;
                return go;
            }

            GameObject CreateButton(Transform parent, string name, string label)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                var img = go.AddComponent<Image>();
                img.color = new Color(0.22f, 0.22f, 0.28f, 1f);
                go.AddComponent<Button>();
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(320, 52);
                var child = new GameObject("Text");
                child.transform.SetParent(go.transform, false);
                child.AddComponent<CanvasRenderer>();
                var tmp = child.AddComponent<TextMeshProUGUI>();
                Stretch(child.GetComponent<RectTransform>());
                tmp.text = label;
                tmp.fontSize = 24;
                tmp.color = Color.white;
                tmp.alignment = TextAlignmentOptions.Center;
                return go;
            }

            GameObject CreatePanel(Transform parent, string name)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                Stretch(go.AddComponent<RectTransform>());
                return go;
            }

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Prologue.unity");
            var canvasGo = GameObject.Find("Canvas");
            if (canvasGo == null)
            {
                Debug.LogError("[PrologueSceneBuilder] No se encontró Canvas.");
                return;
            }

            for (var i = canvasGo.transform.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(canvasGo.transform.GetChild(i).gameObject);

            var canvas = canvasGo.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1
                    | AdditionalCanvasShaderChannels.TexCoord2
                    | AdditionalCanvasShaderChannels.Normal
                    | AdditionalCanvasShaderChannels.Tangent;
            }

            var canvasTr = canvasGo.transform;

            CreateImage(canvasTr, "Img_Background", new Color(0.05f, 0.05f, 0.07f, 1f));

            var panelNarration = CreatePanel(canvasTr, "Panel_Narration");
            var textNarrationGo = CreateTMP(panelNarration.transform, "Text_Narration", "", 32, Color.white,
                TextAlignmentOptions.TopLeft);
            {
                var rt = textNarrationGo.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.08f, 0.18f);
                rt.anchorMax = new Vector2(0.92f, 0.88f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            var btnNarr = CreateButton(panelNarration.transform, "Btn_Continue", "Continuar");
            {
                var rt = btnNarr.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.06f);
                rt.anchorMax = new Vector2(0.5f, 0.06f);
                rt.anchoredPosition = Vector2.zero;
            }

            var panelCharacters = CreatePanel(canvasTr, "Panel_Characters");
            CreateTMP(panelCharacters.transform, "Text_Title", "Los invitados", 40, Color.white,
                TextAlignmentOptions.Center);
            {
                var t = panelCharacters.transform.Find("Text_Title");
                var rt = t.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.88f);
                rt.anchorMax = new Vector2(0.5f, 0.88f);
                rt.sizeDelta = new Vector2(900, 60);
                rt.anchoredPosition = Vector2.zero;
            }

            var charLines = new TextMeshProUGUI[5];
            for (var i = 0; i < 5; i++)
            {
                var lineGo = CreateTMP(panelCharacters.transform, $"CharacterLine_{i}", "", 28, new Color(0.92f, 0.92f, 0.95f),
                    TextAlignmentOptions.Left);
                var lrt = lineGo.GetComponent<RectTransform>();
                var y = 0.72f - i * 0.11f;
                lrt.anchorMin = new Vector2(0.12f, y);
                lrt.anchorMax = new Vector2(0.88f, y + 0.09f);
                lrt.offsetMin = Vector2.zero;
                lrt.offsetMax = Vector2.zero;
                charLines[i] = lineGo.GetComponent<TextMeshProUGUI>();
            }

            var btnChar = CreateButton(panelCharacters.transform, "Btn_Continue", "Continuar");
            {
                var rt = btnChar.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.06f);
                rt.anchorMax = new Vector2(0.5f, 0.06f);
                rt.anchoredPosition = Vector2.zero;
            }

            panelCharacters.SetActive(false);

            var panelMechanics = CreatePanel(canvasTr, "Panel_Mechanics");
            var mechanicsTmpGo = CreateTMP(panelMechanics.transform, "Text_Mechanics", "", 26, new Color(0.9f, 0.9f, 0.93f),
                TextAlignmentOptions.TopLeft);
            {
                var rt = mechanicsTmpGo.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.1f, 0.18f);
                rt.anchorMax = new Vector2(0.9f, 0.88f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            var btnMech = CreateButton(panelMechanics.transform, "Btn_Continue", "Continuar");
            {
                var rt = btnMech.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.06f);
                rt.anchorMax = new Vector2(0.5f, 0.06f);
                rt.anchoredPosition = Vector2.zero;
            }

            panelMechanics.SetActive(false);

            var ctrl = canvasGo.GetComponent<PrologueController>();
            if (ctrl == null)
                ctrl = canvasGo.AddComponent<PrologueController>();

            var so = new SerializedObject(ctrl);
            so.FindProperty("panelNarration").objectReferenceValue = panelNarration;
            so.FindProperty("panelCharacters").objectReferenceValue = panelCharacters;
            so.FindProperty("panelMechanics").objectReferenceValue = panelMechanics;
            so.FindProperty("textNarration").objectReferenceValue = textNarrationGo.GetComponent<TextMeshProUGUI>();
            so.FindProperty("textMechanics").objectReferenceValue = mechanicsTmpGo.GetComponent<TextMeshProUGUI>();

            var arr = so.FindProperty("characterLineTexts");
            arr.arraySize = 5;
            for (var i = 0; i < 5; i++)
                arr.GetArrayElementAtIndex(i).objectReferenceValue = charLines[i];

            so.FindProperty("btnContinueNarration").objectReferenceValue = btnNarr.GetComponent<Button>();
            so.FindProperty("btnContinueCharacters").objectReferenceValue = btnChar.GetComponent<Button>();
            so.FindProperty("btnContinueMechanics").objectReferenceValue = btnMech.GetComponent<Button>();

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ctrl);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[PrologueSceneBuilder] Escena Prologue actualizada (v1.1).");
        }
    }
}
