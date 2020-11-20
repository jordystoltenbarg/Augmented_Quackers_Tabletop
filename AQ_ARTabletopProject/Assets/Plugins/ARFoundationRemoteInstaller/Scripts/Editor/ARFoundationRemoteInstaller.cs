using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;


namespace ARFoundationRemote.Editor {
    public class ARFoundationRemoteInstaller : ScriptableObject {
        [Tooltip("Use this field if your platform require additional extension when making a build.")]
        [SerializeField] public string optionalCompanionAppExtension = "";
        
        public const string pluginName = "AR Foundation Editor Remote";
        static readonly Dictionary<string, string> minDependencies = new Dictionary<string, string> {
            {"com.unity.xr.arfoundation", "3.0.1"},
            {"com.unity.xr.arsubsystems", "3.0.0"},
            {"com.unity.xr.arcore", "3.0.1"},
            {"com.unity.xr.arkit", "3.0.1"},
            {"com.unity.xr.arkit-face-tracking", "3.0.1"},
        };

        const string key = "ARFondationRemote_should_install";
        const string pluginsFolderName = "plugins";

        static bool shouldAddToPackageManager {
            get => EditorPrefs.GetBool(key, false);
            set => EditorPrefs.SetBool(key, value);
        }
        

        static void AddToPackageManager(string path) {
            CheckDependencies(success => {
                if (success) {
                    var installRequest = Client.Add(path);
                    runRequest(installRequest, () => {
                        if (installRequest.Status == StatusCode.Success) {
                            Debug.Log(pluginName + " installed successfully. Please read DOCUMENTATION located at Assets/Plugins/ARFoundationRemoteInstaller/DOCUMENTATION.txt");
                        } else {
                            Debug.LogError(pluginName + " installation failed: " + installRequest.Error.message);
                        }
                    });
                } else {
                    Debug.LogError(pluginName + " installation failed. Please fix errors and press 'Installer-Install Plugin'");
                }
            });
        }

        public static void UnInstallPlugin(bool deleteCache) {
            /*#if AR_FOUNDATION_REMOTE_INSTALLED
                FixesForEditorSupport.Apply();
            #endif
            return;*/
            
            #if AR_FOUNDATION_REMOTE_INSTALLED
                FixesForEditorSupport.Undo();
                if (deleteCache) {
                    deleteDestFolderIfExists();
                }
            #endif
            
            var removalRequest = Client.Remove("com.kyrylokuzyk.arfoundationremote");
            runRequest(removalRequest, () => {
                if (removalRequest.Status == StatusCode.Success) {
                    Debug.Log(pluginName + " removed successfully" + ". If you want to delete the plugin completely, please delete the folder: Assets/Plugins/ARFoundationRemoteInstaller");
                } else {
                    Debug.LogError(pluginName + " removal failed: " + removalRequest.Error.message);
                }
            });
        }


        static void CheckDependencies(Action<bool> callback) {
            var listRequest = Client.List();
            runRequest(listRequest, () => {
                callback(checkDependencies(listRequest));
            });
        }

        static bool checkDependencies(ListRequest listRequest) {
            var result = true;
            foreach (var package in listRequest.Result) {
                var packageName = package.name;
                var currentVersion = parseUnityPackageManagerVersion(package.version);
                if (minDependencies.TryGetValue(packageName, out string dependency)) {
                    //Debug.Log(packageName);
                    var minRequiredVersion = new Version(dependency);
                    if (currentVersion < minRequiredVersion) {
                        result = false;
                        Debug.LogError("Please update this package to the required version via Window -> Package Manager: " + packageName + ":" + minRequiredVersion);
                    }
                }
            }

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) {
                if (listRequest.Result.All(_ => _.name != "com.unity.xr.arkit-face-tracking")) {
                    Debug.Log("To enable iOS face tracking, install ARKit Face Tracking 3.0.1 via Package Manager.");
                }
            }
            
            return result;
        }

        static Version parseUnityPackageManagerVersion(string version) {
            var versionNumbersStrings = version.Split('.', '-');
            const int numOfVersionComponents = 3;
            Assert.IsTrue(versionNumbersStrings.Length >= numOfVersionComponents);
            var numbers = new List<int>();
            for (int i = 0; i < numOfVersionComponents; i++) {
                var str = versionNumbersStrings[i];
                if (int.TryParse(str, out int num)) {
                    numbers.Add(num);
                } else {
                    throw new Exception("cant parse " + str + " in " + version);
                }
            }

            return new Version(numbers[0], numbers[1], numbers[2]);
        }

        static Action requestCompletedCallback;
        static Request currentRequest;

        public static void runRequest(Request request, Action callback) {
            if (currentRequest != null) {
                Debug.Log(currentRequest.GetType().Name + " is already running, skipping new " + request.GetType().Name);
                return;
            }
        
            Assert.IsNull(requestCompletedCallback);
            Assert.IsNull(currentRequest);
            currentRequest = request;
            requestCompletedCallback = callback;
            EditorApplication.update += editorUpdate;
        }

        static void editorUpdate() {
            Assert.IsNotNull(currentRequest);
            if (currentRequest.IsCompleted) {
                EditorApplication.update -= editorUpdate;
                currentRequest = null;
                var cachedCallback = requestCompletedCallback;
                requestCompletedCallback = null;
                cachedCallback();
            }
        }

        [DidReloadScripts]
        static void DidReloadScripts() {
            log($"DidReloadScripts: {shouldAddToPackageManager}");
            if (shouldAddToPackageManager) {
                shouldAddToPackageManager = false;
                addToPackageManager();
            }
        }

        static void addToPackageManager() {
            log("AddToPackageManager");
            if (isUnity2019_2) {
                // Unity 2019.2 doesn't support adding local packages via Package Manager API. But it works fine if manually modify manifest file.
                Debug.LogError($"{pluginName}: please add this line to Packages/manifest.json in dependencies section:\n" + 
                               @"""com.kyrylokuzyk.arfoundationremote"": ""file:../" + pluginsFolderName + @"/ARFoundationRemoteSource""" + "\n\n");
            } else {
                AddToPackageManager("file:../" + pluginsFolderName + "/ARFoundationRemoteSource");
            }
        }
        
        [Conditional("_")]
        static void log(string msg) {
            Debug.Log(msg);
        }

        static bool isUnity2019_2 {
            get {
                #if UNITY_2019_2
                    return true;
                #else
                    return false;
                #endif
            }
        }
        
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static void InstallPlugin(bool verbose) {
            addGitIgnore();
            log($"InstallPlugin verbose: {verbose}");
            var slash = Path.DirectorySeparatorChar;
            var source = $"{dataPathParent}{slash}Assets{slash}Plugins{slash}ARFoundationRemoteInstaller{slash}ARFoundationRemoteSource";
            if (Directory.Exists(source)) {
                deleteDestFolderIfExists();
                Directory.Move(source, getDestFolderPath());
                File.Delete($"{source}.meta");
                shouldAddToPackageManager = true;
                log("AssetDatabase.Refresh()");
                AssetDatabase.Refresh();
                log(source);
                log(getRootDestFolderPath());
            } else if (Directory.Exists(getDestFolderPath())) {
                addToPackageManager();
            } else {
                if (verbose) {
                    Debug.LogError($"{pluginName}: please re-import the plugin or buy the additional license if you're trying to install the plugin on different development machine.");
                }
            }
        }

        static void addGitIgnore() {
            var path = $"{dataPathParent}{Path.DirectorySeparatorChar}.gitignore";
            if (File.Exists(path)) {
                var text = File.ReadAllText(path);
                string ignore = $"{pluginsFolderName}/ARFoundationRemoteSource";
                if (!text.Contains(ignore)) {
                    text += $"\n{ignore}";
                    File.WriteAllText(path, text);
                }
            }
        }

        static DirectoryInfo dataPathParent => Directory.GetParent(Application.dataPath);

        static void deleteDestFolderIfExists() {
            var dest = getDestFolderPath();
            if (Directory.Exists(dest)) {
                Directory.Delete(dest, true);
            }
        }

        static string getDestFolderPath() {
            return $"{getRootDestFolderPath()}{Path.DirectorySeparatorChar}ARFoundationRemoteSource";
        }

        static string getRootDestFolderPath() {
            var slash = Path.DirectorySeparatorChar;
            var plugins = $"{dataPathParent}{slash}{pluginsFolderName}";
            createFolderIfNeeded(plugins);
            return plugins;
            
            void createFolderIfNeeded(string path) {
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
            }
        }

        public static void OnImport() {
            log("OnImport");
            if (isUnity2019_2) {
                Debug.LogError($"{pluginName}: please press Assets/Plugins/ARFoundationRemoteInstaller/Installer/Install Plugin button");
            } else {
                InstallPlugin(false);
            }
        }
    }
}
