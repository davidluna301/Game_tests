using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simonshouse.Chapters;

namespace Simonshouse.Editor
{
    public static class Chapter1SceneBuilder
    {
        [MenuItem("Simonshouse/Build Chapter1 Scene (v1.2)")]
        public static void BuildChapter1Scene()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Chapter1.unity");
            var canvas = GameObject.Find("Canvas")?.transform;
            if (canvas == null)
            {
                Debug.LogError("[Chapter1SceneBuilder] Canvas no encontrado.");
                return;
            }

            var bg = canvas.Find("Background");
            if (bg != null)
                bg.name = "Img_Background";

            var oldRoot = canvas.Find("Container_Options");
            if (oldRoot != null)
                Object.DestroyImmediate(oldRoot.gameObject);

            var panelExp = canvas.Find("Panel_Exploration");
            var panelInt = canvas.Find("Panel_Interactions");
            var panelDec = canvas.Find("Panel_Decision");
            if (panelExp == null || panelInt == null || panelDec == null)
            {
                Debug.LogError("[Chapter1SceneBuilder] Faltan paneles.");
                return;
            }

            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");

            var roomName = GetOrCreateTmp(panelExp, "Text_RoomName", new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.96f),
                44, "EL LOBBY", font, TextAlignmentOptions.Center);
            var roomDesc = GetOrCreateTmp(panelExp, "Text_RoomDesc", new Vector2(0.06f, 0.48f), new Vector2(0.94f, 0.84f),
                26, "", font, TextAlignmentOptions.TopLeft);
            var optExp = GetOrCreateOptionsContainer(panelExp, "Container_Options");

            var intTitle = GetOrCreateTmp(panelInt, "Text_Title", new Vector2(0.06f, 0.82f), new Vector2(0.94f, 0.94f),
                36, "¿Con quién hablas?", font, TextAlignmentOptions.Center);
            var optInt = GetOrCreateOptionsContainer(panelInt, "Container_Options");

            var decPrompt = GetOrCreateTmp(panelDec, "Text_DecisionPrompt", new Vector2(0.06f, 0.52f), new Vector2(0.94f, 0.92f),
                26, "", font, TextAlignmentOptions.TopLeft);
            var optDec = GetOrCreateOptionsContainer(panelDec, "Container_Options");

            var ctrl = Object.FindFirstObjectByType<Chapter1Controller>();
            if (ctrl == null)
            {
                Debug.LogError("[Chapter1SceneBuilder] Chapter1Controller no encontrado.");
                return;
            }

            var so = new SerializedObject(ctrl);
            var optRoot = so.FindProperty("optionsContainer");
            if (optRoot != null)
                optRoot.objectReferenceValue = null;
            so.FindProperty("explorationOptionsContainer").objectReferenceValue = optExp;
            so.FindProperty("interactionsOptionsContainer").objectReferenceValue = optInt;
            so.FindProperty("decisionOptionsContainer").objectReferenceValue = optDec;
            so.FindProperty("textRoomName").objectReferenceValue = roomName;
            so.FindProperty("textRoomDesc").objectReferenceValue = roomDesc;
            so.FindProperty("textInteractionsTitle").objectReferenceValue = intTitle;
            so.FindProperty("textDecisionPrompt").objectReferenceValue = decPrompt;
            so.FindProperty("chapterTitle").stringValue = "CAPÍTULO 1 — La Llegada · 3:00 PM";
            so.FindProperty("narrativeText").stringValue =
                "Son las tres de la tarde. El vestíbulo de la mansión huele a cera derretida y a humedad vieja: un olor que se mete bajo la piel y avisa que este lugar lleva décadas acumulando silencios.\n\n"
                + "Cinco figuras de pie, demasiado conscientes de las sombras que proyectan sobre el mármol, asientan el peso de sus maletas y de sus mentiras. Nadie mira de frente; cada uno vigila el reflejo del otro en los cristales del espejo, en el brillo del reloj de pared, en el libro de visitas abierto como una boca que no debería hablar.\n\n"
                + "Fuera, el día sigue su curso. Dentro, el tiempo se ha quedado atrapado en un instante previo a la tormenta. Lo que comienza aquí no es un duelo compartido: es una negociación entre secretos. Y tú observas. Tú recorres el lobby. Tú decides quién recibe una palabra amable y quién se queda solo con su culpa.";
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ctrl);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Chapter1SceneBuilder] Chapter1 actualizado (v1.2).");
        }

        private static TextMeshProUGUI GetOrCreateTmp(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
            float fontSize, string text, TMP_FontAsset font, TextAlignmentOptions align)
        {
            var tr = parent.Find(name);
            if (tr != null)
                return tr.GetComponent<TextMeshProUGUI>();

            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<CanvasRenderer>();
            var tmp = go.AddComponent<TextMeshProUGUI>();
            if (font != null)
                tmp.font = font;
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = align;
            tmp.enableWordWrapping = true;
            tmp.raycastTarget = false;

            var rt = tmp.rectTransform;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return tmp;
        }

        private static Transform GetOrCreateOptionsContainer(Transform parent, string name)
        {
            var tr = parent.Find(name);
            if (tr != null)
                return tr;

            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.08f, 0.06f);
            rt.anchorMax = new Vector2(0.92f, 0.44f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var vlg = go.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 10f;
            vlg.padding = new RectOffset(12, 12, 12, 12);
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;
            return go.transform;
        }
    }
}
