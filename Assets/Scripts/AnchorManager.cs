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
using System.Linq;
//using RestSharp;

#if WINDOWS_UWP
using Windows.Storage;
#endif

public class AnchorManager : MonoBehaviour
{
    private Dictionary<GameObject, CloudSpatialAnchor> objectDictionary = new Dictionary<GameObject, CloudSpatialAnchor>();
    // <AnchorID, SensorID>
    private Dictionary<string, string> idDictionary = new Dictionary<string, string>();
    //private bool editMode = false;
    private bool isErrorActive = false;

    private GameObject currentSensor;
    private Sensor currentSensorSensor;
    private CloudSpatialAnchor currentCloudAnchor;
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

    [SerializeField]
    [Tooltip("Radius around device to search for anchors in")]
    private float anchorFindDistance = 10f;
    
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

    /// <summary>
    /// Our queue of actions that will be executed on the main thread.
    /// </summary>
    private readonly Queue<Action> dispatchQueue = new Queue<Action>();

    #region Unity Lifecycle
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PeriodicGetAllSensors());

        // Get a reference to the SpatialAnchorManager component (must be on the same gameobject)
        cloudManager = GetComponent<SpatialAnchorManager>();

        //OpenSystemKeyboard();

        ConfigureSessionAsync();

        
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
            currentSensorSensor.SetSensorID(keyboardText);
            // currentSensorToolTip.ToolTipText = keyboardText;
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
    public void CreateSensor()
    {
        // Get the head ray
        Ray headRay = InputRayUtils.GetHeadGazeRay();

        currentSensor = CreateLocalSensor(headRay.GetPoint(sensorDistance));
        currentSensorSensor = currentSensor.GetComponent<Sensor>();
        // currentSensorToolTip = currentSensor.transform.GetChild(0).GetComponent<ToolTip>();

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
            keyboard.ClearKeyboardText();
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
        currentSensorSensor.SetSensorID(keyboardText);
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

    private void OnCloudAnchorLocated(object sender, AnchorLocatedEventArgs args)
    {
        Debug.Log("Why are we here?");
        System.Diagnostics.Debug.WriteLine("Why are we here?");
        System.Diagnostics.Debug.WriteLine(args);
        if (args.Status == LocateAnchorStatus.Located)
        {
            CloudSpatialAnchor cloudAnchor = args.Anchor;
            Debug.Log($"Located: {cloudAnchor.Identifier}");
            if (idDictionary.ContainsKey(cloudAnchor.Identifier))
            {
                UnityDispatcher.InvokeOnAppThread(() =>
                {
                    Debug.Log($"Trying to place located anchor");
                    Pose anchorPose = Pose.identity;

                    GameObject sensor = CreateLocalSensor(anchorPose.position);
                    Sensor sensorSensor = sensor.GetComponent<Sensor>();
                    sensorSensor.SetSensorID(idDictionary[cloudAnchor.Identifier]);
                    CloudNativeAnchor cloudNativeAnchor = sensor.GetComponent<CloudNativeAnchor>();
                    cloudNativeAnchor.CloudToNative(cloudAnchor);
                    objectDictionary.Add(sensor, cloudAnchor);
                });
            }
        }
    }
    #endregion

    #region Internal functions
    private async void ConfigureSessionAsync()
    {
        await cloudManager.CreateSessionAsync();

        cloudManager.AnchorLocated += OnCloudAnchorLocated;
        
        await cloudManager.StartSessionAsync();
        
        anchorLocateCriteria = new AnchorLocateCriteria();
        anchorLocateCriteria.Identifiers = idDictionary.Keys.ToArray();
        anchorLocateCriteria.BypassCache = true;
        anchorLocateCriteria.Strategy = LocateStrategy.AnyStrategy;
        currentWatcher = cloudManager.Session.CreateWatcher(anchorLocateCriteria);
    }

    private void ResetCurrent()
    {
        currentSensor = null;
        currentSensorSensor = null;
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

        // If the cloud portion of the anchor hasn't been created yet, create it
        if (cna.CloudAnchor == null) { cna.NativeToCloud(); }

        // Get the cloud portion of the anchor
        CloudSpatialAnchor cloudAnchor = cna.CloudAnchor;

        while (!cloudManager.IsReadyForCreate)
        {
            await Task.Delay(330);
            float createProgress = cloudManager.SessionStatus.RecommendedForCreateProgress;
            QueueOnUpdate(new Action(() => Debug.Log($"Move your device to capture more environment data: {createProgress:0%}")));
        }

        bool success = false;

        try
        {
            // Actually save
            await cloudManager.CreateAnchorAsync(cloudAnchor);

            // Store
            currentCloudAnchor = cloudAnchor;

            // Success?
            success = currentCloudAnchor != null;

            if (success && !isErrorActive)
            {
                // Await override, which may perform additional tasks
                // such as storing the key in the AnchorExchanger
                await OnSaveCloudAnchorSuccessfulAsync();
            }
            else
            {
                OnSaveCloudAnchorFailed(new Exception("Failed to save, but no exception was thrown."));
            }
        }
        catch (Exception ex)
        {
            OnSaveCloudAnchorFailed(ex);
        }

        string uri = kvStoreApiUri + "/set";
        WWWForm form = new WWWForm();
        form.AddField("AnchorID", currentCloudAnchor.Identifier);
        form.AddField("SensorID", currentID);

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

        //sensorAnchorList.Add(new KeyValuePair<GameObject, CloudSpatialAnchor>(currentSensor, currentCloudAnchor));
        objectDictionary.Add(currentSensor, currentCloudAnchor);
        ResetCurrent();
    }

    protected virtual async Task LoadAnchorFromCloudAsync(string cloudAnchor, string sensorID)
    {

    }

    /// <summary>
    /// Creates a sphere at the hit point, and then saves a CloudSpatialAnchor there.
    /// </summary>
    /// <param name="hitPoint">The hit point.</param>
    protected virtual GameObject CreateLocalSensor(Vector3 hitPoint)
    {
        if (currentSensor != null)
        {
            Debug.Log("You are already editing a sensor, did not create a new one.");
            return currentSensor;
        }

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
    IEnumerator PeriodicGetAllSensors()
    {
        while (true)
        {
            StartCoroutine(GetAllSensors());
            yield return new WaitForSeconds(60);
        }
    }

    IEnumerator GetAllSensors()
    {
        string uri = kvStoreApiUri + "/get_all";

        Debug.Log(uri);
        System.Diagnostics.Debug.WriteLine("This is a test");
        System.Diagnostics.Debug.WriteLine(uri);

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
                JObject dict = JObject.Parse(json);
                idDictionary = dict.ToObject<Dictionary<string, string>>();
                Debug.Log(idDictionary.ToString());
            }
        }
    }
    #endregion
}
