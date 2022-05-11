using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AssetLoader : MonoBehaviour
{
    [SerializeField]
    private AssetReference[] m_AssetsToLoad;

    [SerializeField]
    private Image m_Image;
    
    [SerializeField]
    private TextMeshProUGUI m_DebugText;

    private Sprite[] m_Sprites;
    
    private int m_CurrentTexturesLoaded = -1;
    
    private float m_LoadingCooldown = 1.0f;

    private void Start()
    {
        Caching.ClearCache();
        m_Sprites = new Sprite[m_AssetsToLoad.Length];   
        Invoke("StartInit", m_LoadingCooldown);
    }

    private void StartInit()
    {
        Addressables.InitializeAsync().Completed += InitComplete;
    }

    private void InitComplete(AsyncOperationHandle<IResourceLocator> initOp)
    {
        if (initOp.Status == AsyncOperationStatus.Failed)
        {
            m_DebugText.color = Color.red;
            m_DebugText.text = initOp.OperationException.Message + "\n";
            m_DebugText.text += initOp.OperationException.StackTrace + "\n";
        }
        else
            LoadNextTexture();
        
    }

    private void LoadNextTexture()
    {
        if (++m_CurrentTexturesLoaded < m_AssetsToLoad.Length)
            m_AssetsToLoad[m_CurrentTexturesLoaded].LoadAssetAsync<Sprite>().Completed += OnCompleted;
    }

    private void OnCompleted(AsyncOperationHandle<Sprite> loadingOp)
    {
        if (loadingOp.Status == AsyncOperationStatus.Failed)
        {
            m_DebugText.color = Color.red;
            m_DebugText.text += $"Error loading texture number {m_CurrentTexturesLoaded}.\n";
            m_DebugText.text += loadingOp.OperationException.Message + "\n";
            m_DebugText.text += loadingOp.OperationException.StackTrace + "\n";
        }
        else
        {
            m_Sprites[m_CurrentTexturesLoaded] = loadingOp.Result;
            m_Image.sprite = m_Sprites[m_CurrentTexturesLoaded];
        }

        Invoke("LoadNextTexture", m_LoadingCooldown);
    }
}
