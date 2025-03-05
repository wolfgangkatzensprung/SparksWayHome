using GlobalEvents;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
public class SparksWayHomeEditorWindow : EditorWindow
{
    const float SPACING_SMALL = 5f;
    const float SPACING_MEDIUM = 8f;
    const float SPACING_BIG = 12f;

    private string eventName = "";

    private int selectedIndex = 0;

    [MenuItem("SparksWayHome/SparksWayHome EditorWindow")]
    public static void ShowWindow()
    {
        GetWindow<SparksWayHomeEditorWindow>("SparksWayHome");
    }

    private void OnGUI()
    {
        try
        {
            GUILayout.Label("Player Control", EditorStyles.centeredGreyMiniLabel);

            GUI.backgroundColor = new Color(1f, 1f, 1f, 1f);
            // Button der Player selektiert
            if (GUILayout.Button("Select Player", GUILayout.Height(25f)))
            {
                Transform playerTrans = GameObject.FindGameObjectsWithTag("Player")[0].transform;
                Selection.activeGameObject = playerTrans.gameObject;
            }
            if (GUILayout.Button("Select Animator", GUILayout.Height(25f)))
            {
                Transform playerTrans = GameObject.FindGameObjectsWithTag("Player")[0].GetComponentInChildren<Animator>().transform;
                Selection.activeGameObject = playerTrans.gameObject;
            }

            GUI.backgroundColor = new Color(.3f, .69f, .9f, 1f);
            // Button der Player in die Mitte des Scene View verschiebt
            if (GUILayout.Button("Move Player To View"))
            {
                Transform playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
                SceneView sv = SceneView.lastActiveSceneView;
                sv.MoveToView(playerTrans);
                playerTrans.position += sv.rotation * new Vector3(1, 0, 1);
            }

            GUILayout.Space(SPACING_SMALL);

            GUILayout.Label("Object Control", EditorStyles.centeredGreyMiniLabel);
            GUI.backgroundColor = new Color(0, .7f, .7f, .6f);

            if (GUILayout.Button("Move Selected To View", GUILayout.Height(25f)))
            {
                Transform trans = Selection.activeGameObject.transform;
                SceneView.lastActiveSceneView.MoveToView(trans);
                trans.position = new Vector3(trans.position.x, trans.position.y, 0f);
            }

            GUILayout.Label("Random Rotate Selection", EditorStyles.centeredGreyMiniLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X", EditorStyles.miniButtonLeft))
            {
                Transform[] transforms = Selection.transforms;
                foreach (Transform trans in transforms)
                {
                    trans.rotation = GetRandomRotation(Vector3.right);
                }
            }
            if (GUILayout.Button("Y", EditorStyles.miniButtonMid))
            {
                Transform[] transforms = Selection.transforms;
                foreach (Transform trans in transforms)
                {
                    trans.rotation = GetRandomRotation(Vector3.up);
                }
            }
            if (GUILayout.Button("Z", EditorStyles.miniButtonRight))
            {
                Transform[] transforms = Selection.transforms;
                foreach (Transform trans in transforms)
                {
                    trans.rotation = GetRandomRotation(Vector3.forward);
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Align to Normal"))
            {
                Transform[] transforms = Selection.transforms;
                foreach (Transform trans in transforms)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(trans.position, -trans.up, out hit))
                    {
                        trans.rotation = Quaternion.LookRotation(hit.normal);
                    }
                }
            }



            GUI.backgroundColor = new Color(.7f, .7f, .3f, .6f);
            // Button der das selektierte GameObject in die im NautiWindow selektierte Szene verschiebt
            if (GUILayout.Button("Move GameObject To Scene"))
            {
                Scene sceneToMoveTo = SceneManager.GetSceneAt(EditorPrefs.GetInt("SceneIndex"));
                foreach (GameObject go in Selection.gameObjects)
                {
                    if (go.transform.parent == null)
                        SceneManager.MoveGameObjectToScene(go, sceneToMoveTo);
                    else
                    {
                        go.transform.parent = null;
                        SceneManager.MoveGameObjectToScene(go, sceneToMoveTo);
                    }
                }
            }

            GUILayout.Space(SPACING_BIG);

            GUILayout.Label("Debug Tools", EditorStyles.centeredGreyMiniLabel);
            GUI.backgroundColor = new Color(0f, .25f, .1f, .35f);

            if (GUILayout.Button("Print EventHandler Entries"))
            {
                foreach (var entry in GameEvents.Instance?.EventsByName)
                {
                    Debug.Log(
                        $"<color=teal>{entry.Value.Method.Name}</color> on <color=teal>{entry.Value.Method.DeclaringType.Name}</color> is listening to <color=cyan>{entry.Key}</color>");
                }
            }

            GUILayout.Space(SPACING_MEDIUM);

            GUILayout.Label("Events Helper", EditorStyles.centeredGreyMiniLabel);
            GUI.backgroundColor = new Color(.65f, .325f, .71f, .35f);

            GUILayout.BeginVertical("box");
            eventName = EditorGUILayout.TextField(eventName);
            GUILayout.EndVertical();

            GUI.backgroundColor = new Color(.75f, .225f, .71f, .35f);
            if (GUILayout.Button("Call Event"))
            {
                if (!string.IsNullOrEmpty(eventName))
                {
                    GameEvents.Trigger(eventName);
                    Debug.Log("Event triggered: " + eventName);
                }
            }

            GUILayout.Space(SPACING_MEDIUM);

            GUILayout.Label("Load Scene(s)", EditorStyles.centeredGreyMiniLabel);
            GUI.backgroundColor = Color.white;

            SceneLoadingDropdown();

            GUILayout.Space(SPACING_MEDIUM);

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Play"))
            {
                EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName("Main"));
                EditorApplication.isPlaying = true;
            }

            GUILayout.Space(SPACING_MEDIUM);
        }
        catch (System.Exception e)
        {
            Debug.Log("UtilityWindow Error");
            Debug.LogException(e);
        }
    }


    private string[] sceneLoadingOptions = { "Add Main and UI", "Just Test Level", "Ashe Test Scene", "Level Area A", "Level Area B", "Level Area C", "Level Area D", "All Level Areas", "Main Menu", "Options Menu", "Credits", "Intro", "Outro", "Pause Overlay" };

    private Dictionary<int, Action> sceneLoadingActions = new Dictionary<int, Action>
    {
        { 0, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Additive);
            EditorSceneManager.OpenScene("Assets/Scenes/UI.unity", OpenSceneMode.Additive);
        }},
        { 1, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/UI.unity", OpenSceneMode.Additive);
            EditorSceneManager.OpenScene("Assets/Scenes/Just Test Level.unity", OpenSceneMode.Additive);
        }},
        { 2, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/Ashe Test Scene.unity", OpenSceneMode.Additive);
        }},  
        { 3, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/Level Areas/Level Area A.unity", OpenSceneMode.Additive);
        }},  
        { 4, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/Level Areas/Level Area B.unity", OpenSceneMode.Additive);
        }},    
        { 5, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/Level Areas/Level Area C.unity", OpenSceneMode.Additive);
        }},  
        { 6, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/Level Areas/Level Area D.unity", OpenSceneMode.Additive);
        }},      
        { 7, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/Level Areas/Level Area A.unity", OpenSceneMode.Additive);
            EditorSceneManager.OpenScene("Assets/Scenes/Level Areas/Level Area B.unity", OpenSceneMode.Additive);
            EditorSceneManager.OpenScene("Assets/Scenes/Level Areas/Level Area C.unity", OpenSceneMode.Additive);
            EditorSceneManager.OpenScene("Assets/Scenes/Level Areas/Level Area D.unity", OpenSceneMode.Additive);
        }},     
        { 8, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/UI/MainMenu.unity", OpenSceneMode.Additive);
        }},
        { 9, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/UI/OptionsMenu.unity", OpenSceneMode.Additive);
        }},
        { 10, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/UI/Credits.unity", OpenSceneMode.Additive);
        }},
        { 11, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/UI/IntroCutscene.unity", OpenSceneMode.Additive);
        }},
        { 12, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/UI/OutroCutscene.unity", OpenSceneMode.Additive);
        }}, 
        { 13, () => {
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            EditorSceneManager.OpenScene("Assets/Scenes/UI/PauseOverlay.unity", OpenSceneMode.Additive);
        }}
    };

    private void SceneLoadingDropdown()
    {
        selectedIndex = EditorGUILayout.Popup(selectedIndex, sceneLoadingOptions);
        EditorGUILayout.Space();

        GUI.backgroundColor = new Color(.3f, .325f, .715f, .35f);
        if (GUILayout.Button("Load Scene(s)"))
        {
            if (sceneLoadingActions.ContainsKey(selectedIndex))
            {
                sceneLoadingActions[selectedIndex]();
            }
        }
    }

    Quaternion GetRandomRotation(Vector3 axis)
    {
        return Quaternion.AngleAxis(Random.Range(0,360), axis);
    }
}
#endif