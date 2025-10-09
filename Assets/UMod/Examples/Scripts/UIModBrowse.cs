using UnityEngine;
using UnityEngine.UI;

using UMod;
using UnityEngine.EventSystems;

namespace UMod.Example
{
    public class UIModBrowse : MonoBehaviour
    {
        // Private
        private IModInfo active = null;

        // Public
        public bool persistent = true;
        public EventSystem eventSystem;
        public Button loadButton;
        public GameObject list;
        public GameObject uiPrefab;

        // Methods
        public void Start()
        {
            // Should the UI survive scene loads
            if (persistent == true)
            {
                DontDestroyOnLoad(gameObject);
                DontDestroyOnLoad(eventSystem.gameObject);
            }

#if UNITY_EDITOR
            // Assign the mod directory
            Mod.DefaultDirectory = new ModDirectory(Application.dataPath + "/UMod/Examples/ExampleMods");
#else
            Mod.DefaultDirectory = new ModDirectory(Application.dataPath + "/Mods");
#endif

            // Populate list
            SetActiveSelection(null);
            GenerateUIList();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) GenerateUIList();
        }

        public void OnLoadClicked()
        {
            if (active != null)
            {            
                // Load the mod
                //ModHost host = 
                   Mod.Load(Mod.DefaultDirectory.GetModPath(active.NameInfo.ModName, null));
            }
        }

        [ContextMenu("RefreshMods")]
        private void GenerateUIList()
        {
            // Destroy all cells
            foreach(Transform t in list.transform)
                Destroy(t.gameObject);

            // Create new cells
            foreach (IModInfo info in Mod.DefaultDirectory.GetMods())// ModDirectory.GetMods())
            {
                CreateUICell(list, info);
            }
        }

        private void CreateUICell(GameObject owner, IModInfo mod)
        {
            GameObject go = Instantiate(uiPrefab);

            // Set as child
            go.transform.SetParent(owner.transform, false);

            // Access the script
            UIModElement element = go.GetComponent<UIModElement>();

            // Add a click listener
            element.OnClicked += OnElementClicked;

            // Fill out the fields
            element.Name = mod.NameInfo.ModName;
            element.Version = mod.NameInfo.ModVersion;

            // Use a relative path
            string relative = Mod.DefaultDirectory.GetModPath(mod.NameInfo.ModName, null).ToString();// ModDirectory.GetModPath(mod.NameInfo.ModName).ToString();
            relative = relative.Replace(Application.dataPath + "/", "");

            element.Path = relative;
        }

        private void OnElementClicked(UIModElement element)
        {
            // Reload the mod
            IModInfo info = Mod.DefaultDirectory.GetMod(element.Name, null);//  ModDirectory.GetMod(element.Name);

            // Set as the selection
            SetActiveSelection(info);
        }

        private void SetActiveSelection(IModInfo mod)
        {
            // Store the value
            active = mod;

            // Activate the load button
            loadButton.interactable = (active != null);

            if(active != null)
                Debug.Log(active.NameInfo.ModName);
        }
    }
}
