#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Simonshouse.Interaction;
using Simonshouse.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Simonshouse.EditorTools
{
    /// <summary>Reconstruye <c>Lobby.unity</c>: placeholders 2D, diálogos C1, panel decisión v2.9, hook en puertas.</summary>
    public static class LobbySceneBuilder
    {
        private const string LobbyPath = "Assets/Scenes/Lobby.unity";
        private const string PrefabGmPath = "Assets/Prefabs/Bootstrap/GameManager.prefab";
        private const string PrefabCfPath = "Assets/Prefabs/Bootstrap/ChapterFlowManager.prefab";
        private const string LobbyDecisionRowPrefabPath = "Assets/Prefabs/UI/LobbyDecisionRow.prefab";

        private static readonly int InteractableLayer = 8;

        [MenuItem("Simonshouse/Lobby/Create Bootstrap Prefabs (v2.7)")]
        public static void MenuCreateBootstrapPrefabs()
        {
            EnsureBootstrapPrefabFiles();
            AssetDatabase.Refresh();
        }

        [MenuItem("Simonshouse/Lobby/Rebuild Lobby Scene (v2.9)")]
        public static void RebuildLobbyScene()
        {
            EnsureBootstrapPrefabFiles();
            AssetDatabase.Refresh();
            EnsureSortingLayers();

            var fathersLetter = AssetDatabase.LoadAssetAtPath<ItemData>(LobbySceneV27Content.FathersLetterAssetPath);

            var scene = EditorSceneManager.OpenScene(LobbyPath, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
                Undo.DestroyObjectImmediate(root);

            var bgId = SortingLayer.NameToID("Background");
            var propsId = SortingLayer.NameToID("Props");
            var interactId = SortingLayer.NameToID("Interactables");
            var charId = SortingLayer.NameToID("Characters");

            var camGo = new GameObject("Camera_Main");
            Undo.RegisterCreatedObjectUndo(camGo, "Lobby Camera");
            camGo.tag = "MainCamera";
            camGo.layer = 0;
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.4f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.06f, 0.05f, 0.08f);
            camGo.AddComponent<AudioListener>();

            var inputGo = new GameObject("SceneInputManager");
            Undo.RegisterCreatedObjectUndo(inputGo, "SceneInputManager");
            inputGo.transform.SetParent(camGo.transform, false);
            var sim = inputGo.AddComponent<SceneInputManager>();
            var simSo = new SerializedObject(sim);
            simSo.FindProperty("sceneCamera").objectReferenceValue = cam;
            simSo.FindProperty("interactableLayer").intValue = LayerMask.GetMask("Interactable");
            simSo.ApplyModifiedPropertiesWithoutUndo();

            var lightGo = new GameObject("Directional_Light");
            Undo.RegisterCreatedObjectUndo(lightGo, "Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;

            var layerBg = new GameObject("Layer_Background");
            Undo.RegisterCreatedObjectUndo(layerBg, "Layer_Background");
            _ = CreateSpriteChild(layerBg.transform, "Img_LobbyBackground", new Vector2(0f, 0f),
                new Vector2(19.2f, 10.8f), new Color(0.12f, 0.12f, 0.14f), bgId, 0);

            var layerProps = new GameObject("Layer_Props");
            Undo.RegisterCreatedObjectUndo(layerProps, "Layer_Props");
            CreateSpriteChild(layerProps.transform, "Prop_Chimenea", new Vector2(-6f, -1f), new Vector2(3f, 4f),
                new Color(0.25f, 0.14f, 0.08f), propsId, 1);
            CreateSpriteChild(layerProps.transform, "Prop_Sillones", new Vector2(3f, -3f), new Vector2(4f, 2f),
                new Color(0.35f, 0.08f, 0.08f), propsId, 1);
            CreateSpriteChild(layerProps.transform, "Prop_Gramofono", new Vector2(5f, -2f), new Vector2(1f, 1.5f),
                new Color(0.45f, 0.45f, 0.48f), propsId, 2);
            CreateSpriteChild(layerProps.transform, "Prop_Radio", new Vector2(7f, 0f), new Vector2(1.5f, 1f),
                new Color(0.4f, 0.4f, 0.42f), propsId, 2);
            CreateSpriteChild(layerProps.transform, "Prop_RelojPendulo", new Vector2(-8f, 0f), new Vector2(0.8f, 3f),
                new Color(0.45f, 0.32f, 0.22f), propsId, 2);

            var layerIx = new GameObject("Layer_Interactables");
            Undo.RegisterCreatedObjectUndo(layerIx, "Layer_Interactables");
            CreateObjectInteractable(layerIx.transform, "Obj_LibroVisitas", LobbySceneV27Content.IdLibroVisitas,
                new Vector2(-2f, -3.5f), new Vector2(0.8f, 0.5f), new Color(0.95f, 0.9f, 0.75f), interactId,
                "Libro de visitas", LobbySceneV27Content.DescLibroVisitas, "entry_crossed",
                "", 0, null, true, true);
            CreateObjectInteractable(layerIx.transform, "Obj_Fotografia", LobbySceneV27Content.IdFotoChimenea,
                new Vector2(-6f, 1.5f), new Vector2(1f, 0.8f), new Color(0.55f, 0.45f, 0.35f), interactId,
                "Fotografía — Padre e hijo", LobbySceneV27Content.DescFotografia, "photo_father_son",
                "Robert", 10, null, true, true);
            CreateObjectInteractable(layerIx.transform, "Obj_Periodico", LobbySceneV27Content.IdPeriodico,
                new Vector2(3.5f, -2.5f), new Vector2(0.9f, 0.6f), new Color(0.9f, 0.85f, 0.2f), interactId,
                "Periódico local", LobbySceneV27Content.DescPeriodico, "fire_newspaper",
                "Lisa", -10, null, true, true);
            CreateObjectInteractable(layerIx.transform, "Obj_CajonCarta", LobbySceneV27Content.IdCajonCarta,
                new Vector2(-4f, -3f), new Vector2(1.5f, 0.5f), new Color(0.35f, 0.22f, 0.14f), interactId,
                "Cajón de la cómoda", LobbySceneV27Content.DescCajonCarta, "fathers_letter_found",
                "", 0, fathersLetter, true, true);
            CreateObjectInteractable(layerIx.transform, "Obj_Abrigo", LobbySceneV27Content.IdAbrigo,
                new Vector2(-7f, 0f), new Vector2(0.8f, 2f), new Color(0.2f, 0.2f, 0.22f), interactId,
                "Abrigo en el perchero", LobbySceneV27Content.DescAbrigo, "note_coat",
                "", 0, null, true, true);
            CreateObjectInteractable(layerIx.transform, "Obj_FotoGrupo", LobbySceneV27Content.IdFotoGrupo,
                new Vector2(-6f, 1f), new Vector2(1f, 0.8f), new Color(0.55f, 0.1f, 0.1f), interactId,
                "Fotografía del grupo", LobbySceneV27Content.DescFotoGrupo, "photo_group_marked",
                "", 0, null, true, false);

            var layerCh = new GameObject("Layer_Characters");
            Undo.RegisterCreatedObjectUndo(layerCh, "Layer_Characters");
            CreateCharacter(layerCh.transform, "Char_Robert", "Robert", new Vector2(-3f, -1f), new Vector2(1f, 2.5f),
                new Color(0.12f, 0.18f, 0.35f), charId, LobbySceneV28Dialogues.Robert);
            CreateCharacter(layerCh.transform, "Char_Ana", "Ana", new Vector2(-1f, -1f), new Vector2(1f, 2.5f),
                new Color(0.15f, 0.35f, 0.18f), charId, LobbySceneV28Dialogues.Ana);
            CreateCharacter(layerCh.transform, "Char_Ben", "Ben", new Vector2(1f, -1f), new Vector2(1f, 2.5f),
                new Color(0.55f, 0.42f, 0.3f), charId, LobbySceneV28Dialogues.Ben);
            CreateCharacter(layerCh.transform, "Char_Lisa", "Lisa", new Vector2(3f, -1f), new Vector2(1f, 2.5f),
                new Color(0.55f, 0.25f, 0.25f), charId, LobbySceneV28Dialogues.Lisa);
            CreateCharacter(layerCh.transform, "Char_Lucas", "Lucas", new Vector2(5f, -1f), new Vector2(1f, 2.5f),
                new Color(0.35f, 0.4f, 0.48f), charId, LobbySceneV28Dialogues.Lucas);

            var lobbyCtrlGo = new GameObject("LobbyController");
            Undo.RegisterCreatedObjectUndo(lobbyCtrlGo, "LobbyController");
            var lobbyCtrl = lobbyCtrlGo.AddComponent<LobbyController>();

            CreateLobbyDecisionUi(lobbyCtrl);

            var layerDoors = new GameObject("Layer_Doors");
            Undo.RegisterCreatedObjectUndo(layerDoors, "Layer_Doors");
            CreateDoor(layerDoors.transform, "Puerta_Habitacion", "Habitacion", new Vector2(-9f, 0f), new Vector2(1.5f, 3f),
                new Color(0.35f, 0.22f, 0.12f), interactId, lobbyCtrl);
            CreateDoor(layerDoors.transform, "Puerta_Estudio", "Estudio", new Vector2(-6.5f, 0f), new Vector2(1.5f, 3f),
                new Color(0.38f, 0.24f, 0.14f), interactId, lobbyCtrl);
            CreateDoor(layerDoors.transform, "Puerta_Galeria", "Galeria", new Vector2(6.5f, 0f), new Vector2(1.5f, 3f),
                new Color(0.36f, 0.23f, 0.13f), interactId, lobbyCtrl);
            CreateDoor(layerDoors.transform, "Puerta_Sotano", "Sotano", new Vector2(9f, 0f), new Vector2(1.5f, 3f),
                new Color(0.22f, 0.14f, 0.1f), interactId, lobbyCtrl);

            var eventSysGo = new GameObject("EventSystem");
            Undo.RegisterCreatedObjectUndo(eventSysGo, "EventSystem");
            eventSysGo.AddComponent<EventSystem>();
            eventSysGo.AddComponent<InputSystemUIInputModule>();

            var bootGo = new GameObject("SceneSafetyBootstrap");
            Undo.RegisterCreatedObjectUndo(bootGo, "Bootstrap");
            var boot = bootGo.AddComponent<SceneSafetyBootstrap>();
            var bootSo = new SerializedObject(boot);
            bootSo.FindProperty("gameManagerPrefab").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<GameObject>(PrefabGmPath);
            bootSo.FindProperty("chapterFlowPrefab").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<GameObject>(PrefabCfPath);
            bootSo.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[LobbyBuilder] Lobby v2.9: panel decisión, hook salida en puertas, EventSystem.");
        }

        private static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static GameObject CreateTmp(Transform parent, string name, string text, float fontSize, Color color,
            TextAlignmentOptions align)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<CanvasRenderer>();
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = align;
            tmp.raycastTarget = false;
            return go;
        }

        private static GameObject CreateTmpButton(Transform parent, string name, string label)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.22f, 0.22f, 0.28f, 1f);
            go.AddComponent<Button>();
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 56);
            var child = new GameObject("Text");
            child.transform.SetParent(go.transform, false);
            child.AddComponent<CanvasRenderer>();
            var tmp = child.AddComponent<TextMeshProUGUI>();
            Stretch(child.GetComponent<RectTransform>());
            tmp.text = label;
            tmp.fontSize = 22;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            return go;
        }

        private static void CreateLobbyDecisionUi(LobbyController lobbyCtrl)
        {
            var canvasGo = new GameObject("Canvas_LobbyUI");
            Undo.RegisterCreatedObjectUndo(canvasGo, "Canvas_LobbyUI");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 80;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();
            Stretch(canvasGo.GetComponent<RectTransform>());

            var btnConvoke = CreateTmpButton(canvasGo.transform, "Btn_ConvocarGrupo", "Convocar al grupo");
            {
                var rt = btnConvoke.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(1f, 1f);
                rt.anchoredPosition = new Vector2(-28f, -28f);
                rt.sizeDelta = new Vector2(320, 52);
            }

            var panelDecision = new GameObject("Panel_Decision_C1");
            Undo.RegisterCreatedObjectUndo(panelDecision, "Panel_Decision_C1");
            panelDecision.transform.SetParent(canvasGo.transform, false);
            var panelRt = panelDecision.AddComponent<RectTransform>();
            Stretch(panelRt);
            var dim = panelDecision.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.58f);
            dim.raycastTarget = true;
            panelDecision.SetActive(false);

            var inner = new GameObject("Inner");
            inner.transform.SetParent(panelDecision.transform, false);
            var innerRt = inner.AddComponent<RectTransform>();
            Stretch(innerRt);
            innerRt.offsetMin = new Vector2(140, 100);
            innerRt.offsetMax = new Vector2(-140, -100);
            var vlg = inner.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(24, 24, 24, 24);
            vlg.spacing = 18;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var titleGo = CreateTmp(inner.transform, "Text_DecisionTitle", "Decisión grupal — capítulo 1", 32,
                Color.white, TextAlignmentOptions.Center);
            var titleLe = titleGo.AddComponent<LayoutElement>();
            titleLe.preferredHeight = 48f;

            var promptGo = CreateTmp(inner.transform, "Text_DecisionPrompt",
                "El grupo aguarda una propuesta. Cómo se organice la primera incursión en la mansión marcará el tono de la noche.",
                22, new Color(0.85f, 0.85f, 0.88f), TextAlignmentOptions.Top);
            var promptLe = promptGo.AddComponent<LayoutElement>();
            promptLe.preferredHeight = 100f;
            var promptRt = promptGo.GetComponent<RectTransform>();
            promptRt.sizeDelta = new Vector2(0f, 100f);

            var btnArea = new GameObject("DecisionButtonsContainer");
            Undo.RegisterCreatedObjectUndo(btnArea, "DecisionButtonsContainer");
            btnArea.transform.SetParent(inner.transform, false);
            btnArea.AddComponent<RectTransform>();
            var areaLe = btnArea.AddComponent<LayoutElement>();
            areaLe.flexibleHeight = 1f;
            areaLe.minHeight = 180f;
            areaLe.preferredHeight = 220f;
            var btnVlg = btnArea.AddComponent<VerticalLayoutGroup>();
            btnVlg.padding = new RectOffset(0, 0, 8, 0);
            btnVlg.spacing = 12;
            btnVlg.childAlignment = TextAnchor.UpperCenter;
            btnVlg.childControlHeight = true;
            btnVlg.childControlWidth = true;
            btnVlg.childForceExpandWidth = true;
            btnVlg.childForceExpandHeight = false;

            var rowPrefab = EnsureLobbyDecisionRowPrefab();

            var lcSo = new SerializedObject(lobbyCtrl);
            lcSo.FindProperty("panelDecision").objectReferenceValue = panelDecision;
            lcSo.FindProperty("textDecisionPrompt").objectReferenceValue = promptGo.GetComponent<TextMeshProUGUI>();
            lcSo.FindProperty("decisionButtonsContainer").objectReferenceValue = btnArea.transform;
            lcSo.FindProperty("decisionButtonPrefab").objectReferenceValue = rowPrefab;
            lcSo.FindProperty("btnShowDecision").objectReferenceValue = btnConvoke;
            lcSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject EnsureLobbyDecisionRowPrefab()
        {
            if (File.Exists(LobbyDecisionRowPrefabPath))
                return AssetDatabase.LoadAssetAtPath<GameObject>(LobbyDecisionRowPrefabPath);

            EnsureDir("Assets/Prefabs/UI");
            var root = new GameObject("LobbyDecisionRow");
            var rt = root.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(900, 56);
            var img = root.AddComponent<Image>();
            img.color = new Color(0.22f, 0.22f, 0.28f, 1f);
            var b = root.AddComponent<Button>();
            b.targetGraphic = img;
            var child = new GameObject("Text");
            child.transform.SetParent(root.transform, false);
            child.AddComponent<RectTransform>();
            Stretch(child.GetComponent<RectTransform>());
            child.AddComponent<CanvasRenderer>();
            var tmp = child.AddComponent<TextMeshProUGUI>();
            tmp.text = "Opción";
            tmp.fontSize = 22;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            PrefabUtility.SaveAsPrefabAsset(root, LobbyDecisionRowPrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath<GameObject>(LobbyDecisionRowPrefabPath);
        }

        private static void EnsureBootstrapPrefabFiles()
        {
            EnsureDir("Assets/Prefabs/Bootstrap");
            if (!File.Exists(PrefabGmPath))
            {
                var gm = new GameObject("GameManager");
                gm.AddComponent<GameManager>();
                gm.AddComponent<AudioManager>();
                PrefabUtility.SaveAsPrefabAsset(gm, PrefabGmPath);
                Object.DestroyImmediate(gm);
                Debug.Log("[LobbyBuilder] Creado " + PrefabGmPath);
            }

            if (!File.Exists(PrefabCfPath))
            {
                var cf = new GameObject("ChapterFlowManager");
                cf.AddComponent<ChapterFlowManager>();
                PrefabUtility.SaveAsPrefabAsset(cf, PrefabCfPath);
                Object.DestroyImmediate(cf);
                Debug.Log("[LobbyBuilder] Creado " + PrefabCfPath);
            }
        }

        private static void EnsureDir(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static void EnsureSortingLayers()
        {
            var tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layers = tagManager.FindProperty("m_SortingLayers");
            string[] names = { "Background", "Props", "Interactables", "Characters", "Effects" };
            foreach (var name in names)
            {
                var found = false;
                for (var i = 0; i < layers.arraySize; i++)
                {
                    if (layers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == name)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    continue;

                var maxId = 0;
                for (var i = 0; i < layers.arraySize; i++)
                {
                    var id = layers.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueID").intValue;
                    if (id > maxId)
                        maxId = id;
                }

                var defaultIdx = -1;
                for (var i = 0; i < layers.arraySize; i++)
                {
                    if (layers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == "Default")
                    {
                        defaultIdx = i;
                        break;
                    }
                }

                if (defaultIdx < 0)
                    defaultIdx = layers.arraySize;

                layers.InsertArrayElementAtIndex(defaultIdx);
                var el = layers.GetArrayElementAtIndex(defaultIdx);
                el.FindPropertyRelative("name").stringValue = name;
                el.FindPropertyRelative("uniqueID").intValue = maxId + 1;
                el.FindPropertyRelative("locked").boolValue = false;
            }

            tagManager.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        private static Sprite MakeSprite(Color color)
        {
            const int w = 4, h = 4;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var pixels = new Color[w * h];
            for (var i = 0; i < pixels.Length; i++)
                pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();
            tex.hideFlags = HideFlags.DontSave;
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 1f);
        }

        private static GameObject CreateSpriteChild(Transform parent, string name, Vector2 pos, Vector2 size,
            Color color, int sortingLayerId, int order)
        {
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            go.transform.localScale = new Vector3(size.x / 4f, size.y / 4f, 1f);
            go.layer = 0;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(color);
            sr.sortingLayerID = sortingLayerId;
            sr.sortingOrder = order;
            return go;
        }

        private static GameObject CreateObjectInteractable(
            Transform parent,
            string goName,
            string interactableId,
            Vector2 pos,
            Vector2 size,
            Color color,
            int sortingLayerId,
            string title,
            string description,
            string clueToAdd,
            string characterIsolationEffect,
            int isolationAmount,
            ItemData associatedItem,
            bool addToInventory,
            bool startActive)
        {
            var go = new GameObject(goName);
            Undo.RegisterCreatedObjectUndo(go, goName);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            go.transform.localScale = new Vector3(size.x / 4f, size.y / 4f, 1f);
            go.layer = InteractableLayer;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(color);
            sr.sortingLayerID = sortingLayerId;
            sr.sortingOrder = 1;
            go.AddComponent<BoxCollider2D>();
            go.AddComponent<InteractableHighlight>();
            go.AddComponent<ObjectInteractable>();
            var so = new SerializedObject(go.GetComponent<ObjectInteractable>());
            so.FindProperty("interactableId").stringValue = interactableId;
            so.FindProperty("singleUse").boolValue = true;
            so.FindProperty("objectTitle").stringValue = title;
            so.FindProperty("objectDescription").stringValue = description;
            so.FindProperty("clueToAdd").stringValue = clueToAdd ?? "";
            so.FindProperty("characterIsolationEffect").stringValue = characterIsolationEffect ?? "";
            so.FindProperty("isolationAmount").intValue = isolationAmount;
            so.FindProperty("associatedItem").objectReferenceValue = associatedItem;
            so.FindProperty("addToInventory").boolValue = addToInventory;
            so.ApplyModifiedPropertiesWithoutUndo();
            go.SetActive(startActive);
            return go;
        }

        private static void CreateCharacter(Transform parent, string objectName, string characterName, Vector2 pos,
            Vector2 size, Color color, int sortingLayerId, IReadOnlyList<(string text, bool isNarration)> dialogRows)
        {
            var go = new GameObject(objectName);
            Undo.RegisterCreatedObjectUndo(go, objectName);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            go.transform.localScale = new Vector3(size.x / 4f, size.y / 4f, 1f);
            go.layer = InteractableLayer;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(color);
            sr.sortingLayerID = sortingLayerId;
            sr.sortingOrder = 3;
            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(4f, 4f);
            go.AddComponent<InteractableHighlight>();
            var ch = go.AddComponent<CharacterInteractable>();
            var so = new SerializedObject(ch);
            so.FindProperty("interactableId").stringValue = objectName;
            so.FindProperty("singleUse").boolValue = false;
            so.FindProperty("characterName").stringValue = characterName;
            so.FindProperty("characterSprite").objectReferenceValue = null;
            var lines = so.FindProperty("dialogLines");
            lines.arraySize = dialogRows.Count;
            for (var i = 0; i < dialogRows.Count; i++)
            {
                var el = lines.GetArrayElementAtIndex(i);
                el.FindPropertyRelative("content").stringValue = dialogRows[i].text;
                el.FindPropertyRelative("isNarration").boolValue = dialogRows[i].isNarration;
                el.FindPropertyRelative("requiredClue").stringValue = "";
                el.FindPropertyRelative("requiredItem").stringValue = "";
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateDoor(Transform parent, string objectName, string sceneName, Vector2 pos, Vector2 size,
            Color color, int sortingLayerId, LobbyController lobbyCtrl)
        {
            var go = new GameObject(objectName);
            Undo.RegisterCreatedObjectUndo(go, objectName);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            go.transform.localScale = new Vector3(size.x / 4f, size.y / 4f, 1f);
            go.layer = InteractableLayer;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(color);
            sr.sortingLayerID = sortingLayerId;
            sr.sortingOrder = 2;
            go.AddComponent<BoxCollider2D>();
            go.AddComponent<InteractableHighlight>();
            var door = go.AddComponent<DoorInteractable>();
            var so = new SerializedObject(door);
            so.FindProperty("interactableId").stringValue = objectName;
            so.FindProperty("targetSceneName").stringValue = sceneName;
            so.FindProperty("requiredClue").stringValue = "";
            so.FindProperty("requiredItem").stringValue = "";
            if (lobbyCtrl != null)
                so.FindProperty("lobbyHook").objectReferenceValue = lobbyCtrl;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
