using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetLocalizer : MonoBehaviour
{
    public LocalizedGameObject assetRef;
    private GameObject button;
    private AsyncOperationHandle<GameObject> loadOperation;
    public void LoadAsset()
    {
        if (button != null)
        {
            Destroy(button);
        }
        loadOperation = assetRef.LoadAssetAsync();

        StartCoroutine(LoadButton());
    }

    private IEnumerator LoadButton()
    {
        yield return loadOperation; // Wait for loading.

        // Check the loading was successful.
        if (loadOperation.IsValid())
        {
            button = Instantiate(loadOperation.Result, transform);
        }
    }

}