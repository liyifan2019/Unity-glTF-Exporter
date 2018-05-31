﻿#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

/// <summary>
/// 将 Unity 的场景或者资源导出为 glTF 2.0 标准格式
/// </summary>
public class ExporterGLTF20 : EditorWindow {

    // Fields limits
    const int NAME_LIMIT = 48;
    const int DESC_LIMIT = 1024;
    const int TAGS_LIMIT = 50;

    const int SPACE_SIZE = 5;
    Vector2 DESC_SIZE = new Vector2(512, 64);

    GameObject mExporterGo;
    SceneToGlTFWiz mExporter;

    Texture2D mBanner;
    string mStatus = "";
    GUIStyle mExporterTextArea;

    private bool mExportAnimation = true;
    private string mParamName = "";
    private string mParamDescription = "";
    private string mParamTags = "";

    void Awake() {
        mExporterGo = new GameObject("Exporter");
        mExporter = mExporterGo.AddComponent<SceneToGlTFWiz>();
        //FIXME: Make sure that object is deleted;
        mExporterGo.hideFlags = HideFlags.HideAndDontSave;
    }

    void OnEnable() {
        mBanner = Resources.Load<Texture2D>("ExporterBanner");
        this.minSize = new Vector2(512, 320);
    }

    void OnSelectionChange() {
        updateExporterStatus();
        Repaint();
    }

    void OnGUI() {

        if (mExporterTextArea == null) {
            mExporterTextArea = new GUIStyle(GUI.skin.textArea);
            mExporterTextArea.fixedWidth = DESC_SIZE.x;
            mExporterTextArea.fixedHeight = DESC_SIZE.y;
        }

        // Model settings
        GUILayout.Label("Model properties", EditorStyles.boldLabel);

        // Model name
        GUILayout.Label("Name");
        mParamName = EditorGUILayout.TextField(mParamName);
        GUILayout.Label("(" + mParamName.Length + "/" + NAME_LIMIT + ")", EditorStyles.centeredGreyMiniLabel);
        EditorStyles.textField.wordWrap = true;
        GUILayout.Space(SPACE_SIZE);

        GUILayout.Label("Description");
        mParamDescription = EditorGUILayout.TextArea(mParamDescription, mExporterTextArea);
        GUILayout.Label("(" + mParamDescription.Length + " / 1024)", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(SPACE_SIZE);

        GUILayout.Label("Tags (separated by spaces)");
        mParamTags = EditorGUILayout.TextField(mParamTags);
        GUILayout.Label("'unity' and 'unity3D' added automatically (" + mParamTags.Length + "/50)", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(SPACE_SIZE);

        GUILayout.Label("Options", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        mExportAnimation = EditorGUILayout.Toggle("Export animation (beta)", mExportAnimation);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(SPACE_SIZE);

        bool enable = updateExporterStatus();

        GUI.enabled = enable;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(mStatus, GUILayout.Width(220), GUILayout.Height(32))) {
            if (!enable) {
                EditorUtility.DisplayDialog("Error", mStatus, "Ok");
            }
            else {

                //exporter.ExportCoroutine(exportPath, null, true, true, opt_exportAnimation, true);

            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Banner
        GUI.enabled = true;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(mBanner);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private bool updateExporterStatus() {
        mStatus = "";

        int nbSelectedObjects = Selection.GetTransforms(SelectionMode.Deep).Length;
        if (nbSelectedObjects == 0) {
            mStatus = "No object selected to export";
            return false;
        }

        if (mParamName.Length > NAME_LIMIT) {
            mStatus = "Model name is too long";
            return false;
        }


        if (mParamName.Length == 0) {
            mStatus = "Please give a name to your model";
            return false;
        }


        if (mParamDescription.Length > DESC_LIMIT) {
            mStatus = "Model description is too long";
            return false;
        }


        if (mParamTags.Length > TAGS_LIMIT) {
            mStatus = "Model tags are too long";
            return false;
        }

        mStatus = "Export " + nbSelectedObjects + " object" + (nbSelectedObjects != 1 ? "s" : "");
        return true;
    }

    [MenuItem("Tools/Export to glTF 2.0")]
    static void Init() {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX // edit: added Platform Dependent Compilation - win or osx standalone
        ExporterGLTF20 window = (ExporterGLTF20)EditorWindow.GetWindow(typeof(ExporterGLTF20));
        window.titleContent.text = "glTF 2.0 Exporter";
        window.Show();
#else // and error dialog if not standalone
		EditorUtility.DisplayDialog("Error", "Your build target must be set to standalone", "Okay");
#endif
    }
}

#endif