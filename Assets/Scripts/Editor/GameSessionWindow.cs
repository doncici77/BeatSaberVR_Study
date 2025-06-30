#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class GameSessionWindow : EditorWindow
{
    [MenuItem("Window/GameSession")]
    static void Open() => GetWindow<GameSessionWindow>("Game Session");

    private GameSession _cachedTarget;
    private SerializedObject _cachedTargetSerialized;

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        _cachedTarget = GameManager.gameSession;
        _cachedTargetSerialized = new SerializedObject(_cachedTarget);
        _cachedTargetSerialized.Update();
        Repaint();
    }

    private void OnGUI()
    {
        // Play 모드에서만 감시가능
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Only can monitor fame session on Play Mode", MessageType.Warning);
            return;
        }

        // 게임 세션 정상적으로 만들어 졌는지
        if (_cachedTarget == null)
        {
            if (GameManager.gameSession != null)
            {
                _cachedTarget = GameManager.gameSession;
                _cachedTargetSerialized = new SerializedObject(_cachedTarget);
            }
            else
            {
                EditorGUILayout.HelpBox("Game Sesstion is not instantiated yet", MessageType.Warning);
                return;
            }
        }

        _cachedTargetSerialized.Update();
        EditorGUILayout.PropertyField(_cachedTargetSerialized.FindProperty("selectedSongSpec"));
        _cachedTargetSerialized.ApplyModifiedProperties();
    }
}
#endif
