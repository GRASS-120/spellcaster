using UnityEditor;
using UnityEngine;

// была проблема - unity package не экспортирует project settings (мне нужны были слои и настройки матрицы коллизий)
// нашел такое решение на stackoverflow
public class ExportPackage : MonoBehaviour
{
    [MenuItem ("Export/FullExport")]
    private static void Export()
    {
        AssetDatabase.ExportPackage (AssetDatabase.GetAllAssetPaths(),PlayerSettings.productName + ".unitypackage",ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
    }
}
