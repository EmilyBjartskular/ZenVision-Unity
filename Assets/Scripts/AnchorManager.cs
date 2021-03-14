using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Newtonsoft.Json.Linq;
//using RestSharp;

#if WINDOWS_UWP
using Windows.Storage;
#endif

public class AnchorManager : MonoBehaviour
{
    private List<KeyValuePair<GameObject, CloudSpatialAnchor>> sensorAnchorList = new List<KeyValuePair<GameObject, CloudSpatialAnchor>>();
    private bool editMode = false;
    private bool isErrorActive = false;

    private GameObject currentSensor;
    private CloudSpatialAnchor currentCloudAnchor;
    private ToolTip currentSensorToolTip;
    private string currentID;

    private string keyboardText;
    private bool keyboardActive = false;

    /// <summary>
    /// Prefab of a sensor anchor.
    /// </summary>
    [SerializeField]
    [Tooltip("Prefab used for a sensor anchor.")]
    private GameObject sensorPrefab;

    /// <summary>
    /// Distance from head to create sensor anchor.
    /// </summary>
    [SerializeField]
    [Tooltip("Distance from head to create sensor anchor.")]
    private float sensorDistance = 0.1f;
    
    /// <summary>
    /// KeyValue store api uri.
    /// </summary>
    [SerializeField]
    [Tooltip("KeyValue store api uri.")]
    private string kvStoreApiUri = "127.0.0.1:5000";

    /// <summary>
    /// Main button collection.
    /// </summary>
    [SerializeField]
    [Tooltip("Main button collection.")]
    private GameObject btnCollMain;

    /// <summary>
    /// Edit mode button collection.
    /// </summary>
    [SerializeField]
    [Tooltip("Edit mode button collection.")]
    private GameObject btnCollEdit;

    [SerializeField]
    private MixedRealityKeyboard keyboard;

    /// <summary>
    /// Reference to the SpatialAnchorManager object.
    /// </summary>
    private SpatialAnchorManager cloudManager;

    private AnchorLocateCriteria anchorLocateCriteria;
    private CloudSpatialAnchorWatcher currentWatcher;
    private PlatformLocationProvider locationProvider;

    /// <summary>
    /// Our queue of actions that will be executed on the main thread.
    /// </summary>
    private readonly Queue<Action> dispatchQueue = new Queue<Action>();

    #region Unity Lifecycle
    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the SpatialAnchorManager component (must be on the same gameobject)
        cloudManager = GetComponent<SpatialAnchorManager>();

        // Register for Azure Spatial Anchor events
        // cloudManager.AnchorLocated += CloudManager_AnchorLocated;

        anchorLocateCriteria = new AnchorLocateCriteria();

        //OpenSystemKeyboard();

        ConfigureSessionAsync();
        StartCoroutine(GetAllSensors());
    }

    // Update is called once per frame
    void Update()
    {
        lock (dispatchQueue)
        {
            if (dispatchQueue.Count > 0)
            {
                dispatchQueue.Dequeue()();
            }
        }

        if (keyboardActive)
        {
            keyboardText = keyboard.Text;
            currentSensorToolTip.ToolTipText = keyboardText;
        }
    }

    void OnDestroy()
    {
        if (cloudManager != null && cloudManager.Session != null)
        {
            cloudManager.DestroySession();
        }

        if (currentWatcher != null)
        {
            currentWatcher.Stop();
            currentWatcher = null;
        }
    }
    #endregion

    #region Public functions
    public async void GetSensors()
    {

    }

    public void CreateSensor()
    {
        // Get the head ray
        Ray headRay = InputRayUtils.GetHeadGazeRay();

        currentSensor = CreateLocalSensor(headRay.GetPoint(sensorDistance));
        currentSensorToolTip = currentSensor.transform.GetChild(0).GetComponent<ToolTip>();

        btnCollMain.SetActive(false);
        btnCollEdit.SetActive(true);
    }

    public async void SaveEdit()
    {
        if (currentID != null && currentID != "")
        {
            await SaveCurrentSensorAnchorToCloudAsync();

            btnCollMain.SetActive(true);
            btnCollEdit.SetActive(false);
            ResetCurrent();
        }
        else
        {
            keyboard.ShowKeyboard("", false);
            Debug.Log("SensorID may not be empty.");
        }
    }

    public void CancelEdit()
    {
        GameObject.Destroy(currentSensor);
        ResetCurrent();
        btnCollMain.SetActive(true);
        btnCollEdit.SetActive(false);
    }

    public void CommitKeyboard()
    {
        keyboardText = keyboard.Text;

        currentID = keyboardText;
        currentSensorToolTip.ToolTipText = keyboardText;
        Debug.Log(keyboardText);
        keyboardActive = false;
    }
    #endregion

    #region On functions
    public async void OnShowKeyboard()
    {
        keyboardActive = true;
    }

    public async void OnHideKeyboard()
    {
        keyboardActive = false;
    }

    /// <summary>
    /// Called when a cloud anchor is saved successfully.
    /// </summary>
    protected virtual Task OnSaveCloudAnchorSuccessfulAsync()
    {
        // To be overridden.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when a cloud anchor is not saved successfully.
    /// </summary>
    /// <param name="exception">The exception.</param>
    protected virtual void OnSaveCloudAnchorFailed(Exception exception)
    {
        // we will block the next step to show the exception message in the UI.
        isErrorActive = true;
        Debug.LogException(exception);
        Debug.Log("Failed to save anchor " + exception.ToString());

        // UnityDispatcher.InvokeOnAppThread(() => this.feedbackBox.text = string.Format("Error: {0}", exception.ToString()));
    }

    #endregion

    #region Internal functions
    private async void ConfigureSessionAsync()
    {
        await cloudManager.CreateSessionAsync();
        await cloudManager.StartSessionAsync();
        locationProvider = new PlatformLocationProvider();
        cloudManager.Session.LocationProvider = locationProvider;
    }

    private void ResetCurrent()
    {
        currentSensor = null;
        currentSensorToolTip = null;
        currentID = null;
        currentCloudAnchor = null;
    }

    private void QueueOnUpdate(Action updateAction)
    {
        lock (dispatchQueue)
        {
            dispatchQueue.Enqueue(updateAction);
        }
    }

    /// <summary>
    /// Saves the current object anchor to the cloud.
    /// </summary>
    protected virtual async Task SaveCurrentSensorAnchorToCloudAsync()
    {
        Debug.Log(currentSensor);

        // Get the cloud-native anchor behavior
        CloudNativeAnchor cna = currentSensor.GetComponent<CloudNativeAnchor>();
        Debug.Log(cna);

        // If the cloud portion of the anchor hasn't been created yet, create it
        if (cna.CloudAnchor == null) { cna.NativeToCloud(); }

        // Get the cloud portion of the anchor
        CloudSpatialAnchor cloudAnchor = cna.CloudAnchor;
        Debug.Log(cloudAnchor);

        Debug.Log(cloudManager);
        Debug.Log(cloudManager.IsReadyForCreate);
        Debug.Log(cloudManager.SessionStatus);
        Debug.Log(cloudManager.SessionStatus.RecommendedForCreateProgress);

        while (!cloudManager.IsReadyForCreate)
        {
            await Task.Delay(330);
            float createProgress = cloudManager.SessionStatus.RecommendedForCreateProgress;
            QueueOnUpdate(new Action(() => Debug.Log($"Move your device to capture more environment data: {createProgress:0%}")));
        }

        bool success = false;

        try
        {
            Debug.Log("1");
            // Actually save
            await cloudManager.CreateAnchorAsync(cloudAnchor);

            Debug.Log("2");
            // Store
            currentCloudAnchor = cloudAnchor;

            Debug.Log("3");
            // Success?
            success = currentCloudAnchor != null;

            if (success && !isErrorActive)
            {
                // Await override, which may perform additional tasks
                // such as storing the key in the AnchorExchanger
                Debug.Log("Maybe we made it???");
                await OnSaveCloudAnchorSuccessfulAsync();
            }
            else
            {
                Debug.Log("Are we here?");
                OnSaveCloudAnchorFailed(new Exception("Failed to save, but no exception was thrown."));
            }
        }
        catch (Exception ex)
        {
            Debug.Log("We are here");
            OnSaveCloudAnchorFailed(ex);
        }

        Debug.Log("success?");
        Debug.Log(currentCloudAnchor.Identifier);

        string uri = kvStoreApiUri + "/set_anchor";
        WWWForm form = new WWWForm();
        form.AddField("SensorID", currentID);
        form.AddField("AnchorID", currentCloudAnchor.Identifier);

        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Success");
            }
        }
    }

    /// <summary>
    /// Creates a sphere at the hit point, and then saves a CloudSpatialAnchor there.
    /// </summary>
    /// <param name="hitPoint">The hit point.</param>
    protected virtual GameObject CreateLocalSensor(Vector3 hitPoint)
    {
        // Create a white sphere.
        // Create the prefab
        GameObject newGameObject = GameObject.Instantiate(sensorPrefab, hitPoint, Quaternion.identity) as GameObject;

        // Attach a cloud-native anchor behavior to help keep cloud
        // and native anchors in sync.
        newGameObject.AddComponent<CloudNativeAnchor>();

        // Set the color
        newGameObject.GetComponent<MeshRenderer>().material.color = Color.white;

        Debug.Log("ASA Info: Created a local anchor.");
        // Return created object
        return newGameObject;
    }
    #endregion

    #region Coroutines
    IEnumerator GetAllSensors()
    {
        string uri = kvStoreApiUri + "/get_anchors";

        Debug.Log(uri);

        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Success");

                var json = www.downloadHandler.text;
                Debug.Log(json);

                JObject list = JObject.Parse(json);

                foreach(var kvpair in list)
                {
                    Debug.Log(kvpair);
                }
            }
        }
    }
    #endregion
}
