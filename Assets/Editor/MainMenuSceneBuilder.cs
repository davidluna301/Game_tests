using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TMPro;
using Simonshouse.UI;

namespace Simonshouse.Editor
{
    public static class MainMenuSceneBuilder
    {
        [MenuItem("Simonshouse/Build MainMenu Scene")]
        public static void BuildMainMenuScene()
        {
            void Stretch(RectTransform rt)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            GameObject CreateImage(Transform parent, string name, Color c)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                go.AddComponent<CanvasRenderer>();
                var img = go.AddComponent<Image>();
                img.color = c;
                img.raycastTarget = false;
                Stretch(go.GetComponent<RectTransform>());
                return go;
            }

            GameObject CreateTMP(Transform parent, string name, string text, float fontSize, Color color,
                TextAlignmentOptions alignment)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                go.AddComponent<CanvasRenderer>();
                var tmp = go.AddComponent<TextMeshProUGUI>();
                tmp.text = text;
                tmp.fontSize = fontSize;
                tmp.color = color;
                tmp.alignment = alignment;
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
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 56);
                var child = new GameObject("Text");
                child.transform.SetParent(go.transform, false);
                child.AddComponent<CanvasRenderer>();
                var tmp = child.AddComponent<TextMeshProUGUI>();
                Stretch(child.GetComponent<RectTransform>());
                tmp.text = label;
                tmp.fontSize = 26;
                tmp.color = Color.white;
                tmp.alignment = TextAlignmentOptions.Center;
                return go;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.07f);
            camGo.AddComponent<AudioListener>();

            var mainMenuRoot = new GameObject("MainMenu");
            var menuCtrl = mainMenuRoot.AddComponent<MainMenuController>();

            var canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(mainMenuRoot.transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();
            Stretch(canvasGo.GetComponent<RectTransform>());

            CreateImage(canvasGo.transform, "Img_Background", new Color(0.06f, 0.06f, 0.08f, 1f));

            var title = CreateTMP(canvasGo.transform, "Text_Title", "LA MANSIÓN DE SIMÓN", 56, Color.white,
                TextAlignmentOptions.Center);
            {
                var rt = title.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.78f);
                rt.anchorMax = new Vector2(0.5f, 0.78f);
                rt.sizeDelta = new Vector2(1400, 120);
                rt.anchoredPosition = Vector2.zero;
            }

            var sub = CreateTMP(canvasGo.transform, "Text_Subtitle", "Europa, 1943", 28,
                new Color(0.55f, 0.55f, 0.58f, 1f), TextAlignmentOptions.Center);
            {
                var rt = sub.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.68f);
                rt.anchorMax = new Vector2(0.5f, 0.68f);
                rt.sizeDelta = new Vector2(800, 60);
                rt.anchoredPosition = Vector2.zero;
            }

            var btnNew = CreateButton(canvasGo.transform, "Btn_NewGame", "Nueva Partida");
            {
                var rt = btnNew.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.42f);
                rt.anchorMax = new Vector2(0.5f, 0.42f);
                rt.anchoredPosition = Vector2.zero;
            }

            var btnCont = CreateButton(canvasGo.transform, "Btn_Continue", "Continuar");
            {
                var rt = btnCont.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.32f);
                rt.anchorMax = new Vector2(0.5f, 0.32f);
                rt.anchoredPosition = Vector2.zero;
            }

            var btnQuit = CreateButton(canvasGo.transform, "Btn_Quit", "Salir");
            {
                var rt = btnQuit.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.22f);
                rt.anchorMax = new Vector2(0.5f, 0.22f);
                rt.anchoredPosition = Vector2.zero;
            }

            var gm = new GameObject("GameManager");
            gm.transform.SetParent(mainMenuRoot.transform, false);
            gm.AddComponent<GameManager>();

            var cfm = new GameObject("ChapterFlowManager");
            cfm.transform.SetParent(mainMenuRoot.transform, false);
            cfm.AddComponent<ChapterFlowManager>();

            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");

            var list = EditorBuildSettings.scenes.ToList();
            list.RemoveAll(s =>
            {
                var p = s.path.Replace('\\', '/');
                return p.EndsWith("/New Game.unity") || p.EndsWith("/MainMenu.unity") ||
                       p.EndsWith("/Prologue.unity");
            });
            list.Insert(0, new EditorBuildSettingsScene("Assets/Scenes/Prologue.unity", true));
            list.Insert(0, new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true));
            EditorBuildSettings.scenes = list.ToArray();

            Debug.Log("[MainMenuSceneBuilder] MainMenu guardada y Build Settings actualizados.");
        }
    }
}
