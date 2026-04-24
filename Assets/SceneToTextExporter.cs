using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using UnityEngine.SceneManagement;

public class SceneToTextExporter : EditorWindow
{
    [MenuItem("Tools/Export Scene (AI Optimized)")]
    public static void ExportScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"# SCENE_NAME: {scene.name}");
        sb.AppendLine($"# EXPORT_DATE: {System.DateTime.Now:yyyy-MM-dd}");
        sb.AppendLine("---");

        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            ProcessObject(obj, sb, 0);
        }

        string path = EditorUtility.SaveFilePanel("Save AI Scene Data", "", scene.name + "_ai_ready", "yaml");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, sb.ToString());
            Debug.Log($"AI-optimized export saved to: {path}");
        }
    }

    private static void ProcessObject(GameObject obj, StringBuilder sb, int indent)
    {
        string gap = new string(' ', indent * 2);

        // Используем синтаксис YAML-подобный
        sb.AppendLine($"{gap}- GameObject: \"{obj.name}\"");
        sb.AppendLine($"{gap}  Tag: {obj.tag}");
        sb.AppendLine($"{gap}  Layer: {LayerMask.LayerToName(obj.layer)}");

        var t = obj.transform;
        sb.AppendLine($"{gap}  Transform: {{pos: {t.localPosition:F2}, rot: {t.localEulerAngles:F1}, scale: {t.localScale:F2}}}");

        Component[] components = obj.GetComponents<Component>();
        if (components.Length > 1) // 1 это всегда Transform
        {
            sb.AppendLine($"{gap}  Components:");
            foreach (var comp in components)
            {
                if (comp == null || comp is Transform) continue;

                sb.AppendLine($"{gap}    - {comp.GetType().Name}:");

                SerializedObject so = new SerializedObject(comp);
                SerializedProperty prop = so.GetIterator();

                if (prop.NextVisible(true))
                {
                    do
                    {
                        if (prop.name == "m_Script") continue;
                        string value = GetPropertyValue(prop);
                        if (!string.IsNullOrEmpty(value))
                        {
                            // Очищаем имя свойства от лишних пробелов для нейронки
                            string cleanName = prop.name.Replace("m_", "");
                            sb.AppendLine($"{gap}        {cleanName}: {value}");
                        }
                    }
                    while (prop.NextVisible(false));
                }
            }
        }

        // Рекурсия для детей
        if (obj.transform.childCount > 0)
        {
            sb.AppendLine($"{gap}  Children:");
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                ProcessObject(obj.transform.GetChild(i).gameObject, sb, indent + 2);
            }
        }
    }

    private static string GetPropertyValue(SerializedProperty prop)
    {
        // (Метод GetPropertyValue остается таким же, как в предыдущих примерах, 
        // но лучше возвращать числа и простые строки без лишнего декора)
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer: return prop.intValue.ToString();
            case SerializedPropertyType.Boolean: return prop.boolValue.ToString().ToLower();
            case SerializedPropertyType.Float: return prop.floatValue.ToString("F2");
            case SerializedPropertyType.String: return $"\"{prop.stringValue}\"";
            case SerializedPropertyType.Vector3: return prop.vector3Value.ToString();
            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue != null ? $"ref:{prop.objectReferenceValue.name}" : "null";
            case SerializedPropertyType.Enum:
                return (prop.enumValueIndex >= 0 && prop.enumValueIndex < prop.enumDisplayNames.Length)
                    ? prop.enumDisplayNames[prop.enumValueIndex] : prop.intValue.ToString();
            default: return null;
        }
    }
}