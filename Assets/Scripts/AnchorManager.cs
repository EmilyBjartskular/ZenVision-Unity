//#if WINDOWS_UWP
//#define HOLOLENS
//#endif

//#define HOLOLENS

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
//#if HOLOLENS
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
//#endif
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
//#if HOLOLENS
    private Dictionary<GameObject, CloudSpatialAnchor> objectDictionary = new Dictionary<GameObject, CloudSpatialAnchor>();
//#endif
    // <AnchorID, SensorID>
    private Dictionary<string, string> idDictionary = new Dictionary<string, string>();
    //private bool editMode = false;
    private bool isErrorActive = false;

    private GameObject currentSensor;
    private Sensor currentSensorSensor;
//#if HOLOLENS
    private CloudSpatialAnchor currentCloudAnchor;
//#endif

    private string keyboardText;
    private bool keyboardActive = false;


    /// <summary>
    /// Reference to the SpatialAnchorManager object.
    /// </summary>
//#if HOLOLENS
    private SpatialAnchorManager cloudManager;

    private AnchorLocateCriteria anchorLocateCriteria;
    private CloudSpatialAnchorWatcher currentWatcher;
//#endif

    /// <summary>
    /// Our queue of actions that will be executed on the main thread.
    /// </summary>
    private readonly Queue<Action> dispatchQueue = new Queue<Action>();

    public bool DeleteMode { get; set; }

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
    private string kvStoreApiUri = "http://127.0.0.1:5000";

    [SerializeField]
    private string apiUri = "http://127.0.0.1:8010";

    [SerializeField]
    private Interactable btnCreate;

    [SerializeField]
    private Interactable btnDelete;

    [SerializeField]
    private Interactable btnSave;

    [SerializeField]
    private Interactable btnCancel;

    [SerializeField]
    private MixedRealityKeyboard keyboard;

    [SerializeField]
    private GameObject DataWindow;

    #region Unity Lifecycle
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PeriodicGetAllSensors());

//#if HOLOLENS
        // Get a reference to the SpatialAnchorManager component (must be on the same gameobject)
        cloudManager = GetComponent<SpatialAnchorManager>();

        //OpenSystemKeyboard();

        ConfigureSessionAsync();
//#endif

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
            currentSensorSensor.SensorID = keyboardText;
            // currentSensorToolTip.ToolTipText = keyboardText;
        }
    }

    void OnDestroy()
    {
//#if HOLOLENS
        if (cloudManager != null && cloudManager.Session != null)
        {
            cloudManager.DestroySession();
        }

        if (currentWatcher != null)
        {
            currentWatcher.Stop();
            currentWatcher = null;
        }
//#endif
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

        btnCreate.IsEnabled = false;
        btnDelete.IsEnabled = false;
        btnSave.IsEnabled = true;
        btnCancel.IsEnabled = true;
    }

    public void DeleteSensors()
    {
//#if HOLOLENS
        DeleteMode = true;
//#endif
    }

    public async void DeleteSensorAsync(GameObject sensor, string sensorId)
    {
//#if HOLOLENS
        string uri = kvStoreApiUri + "/delete";
        WWWForm id = new WWWForm();
        id.AddField("AnchorID", objectDictionary[sensor].Identifier);
        using (UnityWebRequest www = UnityWebRequest.Post(uri, id))
        {
            www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log($"Sensor {sensorId} has been deleted");
                await cloudManager.DeleteAnchorAsync(objectDictionary[sensor]);
                objectDictionary.Remove(sensor);
//#endif
                idDictionary.Remove(sensorId);
                Destroy(sensor);
//#if HOLOLENS
            }
        }
//#endif
    }

    public async void SaveEdit()
    {

        if (!String.IsNullOrEmpty(currentSensorSensor.SensorID))
        {
            await SaveCurrentSensorAnchorToCloudAsync();

            Debug.Log(currentSensorSensor);
            Debug.Log(currentSensorSensor.button);
            currentSensorSensor.button.IsEnabled = true;
            btnCreate.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            ResetCurrent();
            keyboard.ClearKeyboardText();
            keyboard.HideKeyboard();
        }
        else
        {
            keyboard.ShowKeyboard("", false);
            Debug.Log("SensorID may not be empty.");
        }
    }

    public void CancelEdit()
    {
        if (DeleteMode)
        {
            DeleteMode = false;
        }
        else
        {
            GameObject.Destroy(currentSensor);
            ResetCurrent();
            btnCreate.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
        }
    }

    public void CommitKeyboard()
    {
        keyboardText = keyboard.Text;

        currentSensorSensor.SensorID = keyboardText;
        Debug.Log(keyboardText);
        keyboardActive = false;
    }
    #endregion

    #region On functions
    public void OnShowKeyboard()
    {
        keyboardActive = true;
    }

    public void OnHideKeyboard()
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


        System.Diagnostics.Debug.WriteLine(exception);
        System.Diagnostics.Debug.WriteLine("Failed to save anchor " + exception.ToString());

        // UnityDispatcher.InvokeOnAppThread(() => this.feedbackBox.text = string.Format("Error: {0}", exception.ToString()));
    }

//#if HOLOLENS
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
                    sensorSensor.SensorID = idDictionary[cloudAnchor.Identifier];
                    sensorSensor.button.IsEnabled = true;
                    CloudNativeAnchor cloudNativeAnchor = sensor.GetComponent<CloudNativeAnchor>();
                    cloudNativeAnchor.CloudToNative(cloudAnchor);
                    objectDictionary.Add(sensor, cloudAnchor);

                    string uri = $"{apiUri}/api/type/{currentSensorSensor.SensorID}";
                    using (UnityWebRequest www = UnityWebRequest.Get(uri))
                    {
                        www.SendWebRequest();
                        if (www.isNetworkError || www.isHttpError)
                        {
                            Debug.Log(www.error);
                        }
                        else
                        {
                            Debug.Log("Success");
                            var json = www.downloadHandler.text;
                            JObject dict = JObject.Parse(json);
                            var val = (string)dict["type"];
                            try
                            {

                                sensorSensor.SensorType = (SensorType)Enum.Parse(typeof(SensorType), val);
                            }
                            catch (ArgumentException)
                            {
                                Debug.Log($"Could not cast '{val}' to known SensorType");
                                sensorSensor.SensorType = SensorType.DefaultSensor;
                            }
                        }
                    }
                });
            }
        }
    }
//#endif
    #endregion

    #region Internal functions
//#if HOLOLENS
    private async void ConfigureSessionAsync()
    {
        await cloudManager.CreateSessionAsync();

        cloudManager.AnchorLocated += OnCloudAnchorLocated;

        await cloudManager.StartSessionAsync();
        
        //PlatformLocationProvider locationProvider = new PlatformLocationProvider();
        //locationProvider.Sensors.BluetoothEnabled = true;
        //locationProvider.Sensors.GeoLocationEnabled = true;
        //locationProvider.Sensors.WifiEnabled = true;
        //locationProvider.Sensors.KnownBeaconProximityUuids = ...
        //cloudManager.Session.LocationProvider = locationProvider;

        anchorLocateCriteria = new AnchorLocateCriteria();

        //NearDeviceCriteria nearDevice = new NearDeviceCriteria();
        //nearDevice.DistanceInMeters = anchorFindDistance;
        //nearDevice.MaxResultCount = 100;
        //anchorLocateCriteria.NearDevice = nearDevice;

        anchorLocateCriteria.Identifiers = idDictionary.Keys.ToArray();
        //anchorLocateCriteria.BypassCache = true;
        //anchorLocateCriteria.Strategy = LocateStrategy.AnyStrategy;
        currentWatcher = cloudManager.Session.CreateWatcher(anchorLocateCriteria);
    }
//#endif

    private void ResetCurrent()
    {
        currentSensor = null;
        currentSensorSensor = null;
//#if HOLOLENS
        currentCloudAnchor = null;
//#endif
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

//#if HOLOLENS
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

        Debug.Log(cloudManager.SessionStatus);
        Debug.Log($"Started? {cloudManager.IsSessionStarted}");
        if(!cloudManager.IsSessionStarted) await cloudManager.StartSessionAsync();

        try
        {
            // Actually save
            await cloudManager.CreateAnchorAsync(cloudAnchor);

            // Store
            currentCloudAnchor = cloudAnchor;
            Debug.Log(currentCloudAnchor);

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
        form.AddField("SensorID", currentSensorSensor.SensorID);

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

        string uri2 = $"{apiUri}/api/type/{currentSensorSensor.SensorID}";
        using (UnityWebRequest www = UnityWebRequest.Get(uri2))
        {
            www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Success");
                var json = www.downloadHandler.text;
                JObject dict = JObject.Parse(json);
                var val = (string)dict["type"];
                try
                {

                    currentSensorSensor.SensorType = (SensorType)Enum.Parse(typeof(SensorType), val);
                }
                catch (ArgumentException)
                {
                    Debug.Log($"Could not cast '{val}' to known SensorType");
                    currentSensorSensor.SensorType = SensorType.DefaultSensor;
                }
            }
        }


        //sensorAnchorList.Add(new KeyValuePair<GameObject, CloudSpatialAnchor>(currentSensor, currentCloudAnchor));
        objectDictionary.Add(currentSensor, currentCloudAnchor);
//#endif
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
//#if HOLOLENS
        // Attach a cloud-native anchor behavior to help keep cloud
        // and native anchors in sync.
        newGameObject.AddComponent<CloudNativeAnchor>();
//#endif

        Sensor newGameObjectSensor = newGameObject.GetComponent<Sensor>();
        newGameObjectSensor.Manager = this;
        newGameObjectSensor.Network = GetComponent<NetworkGadget>();
        newGameObjectSensor.DataWindow = DataWindow;

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
