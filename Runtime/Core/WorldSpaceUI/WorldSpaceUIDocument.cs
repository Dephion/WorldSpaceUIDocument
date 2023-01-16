using System;
using Dephion.Ui.Core.Extensions;
using Dephion.Ui.Core.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Dephion.Ui.Core.WorldSpaceUI
{
    public class WorldSpaceUIDocument : MonoBehaviour
    {
        public static event Action DocumentSpawned, DocumentDestroyed;

        [Tooltip("Width of the panel in pixels. The RenderTexture used to render the panel will have this width.")] [SerializeField]
        protected int _panelWidth = 1280;

        [Tooltip("Height of the panel in pixels. The RenderTexture used to render the panel will have this height.")] [SerializeField]
        protected int _panelHeight = 720;

        [Tooltip("Scale of the panel. It is like the zoom in a browser.")] [SerializeField]
        protected float _panelScale = 1.0f;

        [Tooltip("Pixels per world units, it will determine the real panel size in the world based on panel pixel width and height.")] [SerializeField]
        protected float _pixelsPerUnit = 1280.0f;

        [Tooltip("Visual tree element object of this panel.")] [SerializeField]
        protected VisualTreeAsset _visualTreeAsset;

        [Tooltip("PanelSettings that will be used to create a new instance for this panel.")] [SerializeField]
        protected PanelSettings _panelSettingsPrefab;

        [Tooltip("RenderTexture that will be used to create a new instance for this panel.")] [SerializeField]
        protected RenderTexture _renderTexturePrefab;

        public Vector2 PanelSize
        {
            get => new Vector2(_panelWidth, _panelHeight);
            set
            {
                _panelWidth = Mathf.RoundToInt(value.x);
                _panelHeight = Mathf.RoundToInt(value.y);
                RefreshPanelSize();
            }
        }

        public float PanelScale
        {
            get => _panelScale;
            set
            {
                _panelScale = value;

                if (PanelSettings != null)
                    PanelSettings.scale = value;
            }
        }

        public VisualTreeAsset VisualTreeAsset
        {
            get => _visualTreeAsset;
            set
            {
                _visualTreeAsset = value;

                if (UiDocument != null)
                    UiDocument.visualTreeAsset = value;
            }
        }

        public int PanelWidth
        {
            get => _panelWidth;
            set
            {
                _panelWidth = value;
                RefreshPanelSize();
            }
        }

        public int PanelHeight
        {
            get => _panelHeight;
            set
            {
                _panelHeight = value;
                RefreshPanelSize();
            }
        }

        public float PixelsPerUnit
        {
            get => _pixelsPerUnit;
            set
            {
                _pixelsPerUnit = value;
                RefreshPanelSize();
            }
        }

        public PanelSettings PanelSettingsPrefab
        {
            get => _panelSettingsPrefab;
            set
            {
                _panelSettingsPrefab = value;
                RebuildPanel();
            }
        }

        public RenderTexture RenderTexturePrefab
        {
            get => _renderTexturePrefab;
            set
            {
                _renderTexturePrefab = value;
                RebuildPanel();
            }
        }

        protected MeshRenderer MeshRenderer;
        protected PanelEventHandler PanelEventHandler;
        protected UIDocument UiDocument;
        protected PanelSettings PanelSettings;
        protected RenderTexture RenderTexture;
        protected Material Material;
        private WorldSpaceUIPanelRaycaster _panelPanelRaycaster;
        private Texture2D _mainTexture2D;
        private Vector2 _resolution;


        protected virtual void Awake()
        {
            PixelsPerUnit = _pixelsPerUnit;
            CreateCanvas();
            _resolution = new Vector2(_panelWidth, _panelHeight);
        }

        protected virtual void Start()
        {
            RebuildPanel();
            DocumentSpawned?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            DestroyGeneratedAssets();
            DocumentDestroyed?.Invoke();
        }

        private void CreateCanvas()
        {
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer = gameObject.AddComponent<MeshRenderer>();
            MeshRenderer.sharedMaterial = null;
            MeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            MeshRenderer.receiveShadows = false;
            MeshRenderer.allowOcclusionWhenDynamic = false;
            MeshRenderer.lightProbeUsage = LightProbeUsage.Off;
            MeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            MeshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            meshFilter.sharedMesh = quad.GetComponent<MeshFilter>().sharedMesh;
            gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
            Destroy(quad);
        }

        /// <summary>
        /// Use this method to initialise the panel without triggering a rebuild (i.e.: when instantiating it from scripts). Start method
        /// will always trigger RebuildPanel(), but if you are calling this after the GameObject started you must call RebuildPanel() so the
        /// changes take effect.
        /// </summary>
        public void InitPanel(int panelWidth, int panelHeight, float panelScale, float pixelsPerUnit, VisualTreeAsset visualTreeAsset, PanelSettings panelSettingsPrefab, RenderTexture renderTexturePrefab)
        {
            _panelWidth = panelWidth;
            _panelHeight = panelHeight;
            _panelScale = panelScale;
            _pixelsPerUnit = pixelsPerUnit;
            _visualTreeAsset = visualTreeAsset;
            _panelSettingsPrefab = panelSettingsPrefab;
            RenderTexture = renderTexturePrefab;
        }

        /// <summary>
        /// Rebuilds the panel by destroy current assets and generating new ones based on the configuration.
        /// </summary>
        public void RebuildPanel()
        {
            DestroyGeneratedAssets();
            RenderTexture = InstantiateRenderTexture();
            PanelSettings = CreateWorldPanelSettings(RenderTexture);
            AddUiDocument();
            SetupMaterial();
            RefreshPanelSize();
            ReplacePanelRaycaster();
        }

        /// <summary>
        /// Replace the default panel raycaster with a Dephion WorldCanvasPanelRaycaster
        /// </summary>
        private void ReplacePanelRaycaster()
        {
            PanelEventHandler[] handlers = FindObjectsOfType<PanelEventHandler>();

            foreach (PanelEventHandler handler in handlers)
            {
                if (handler.panel == UiDocument.rootVisualElement.panel)
                {
                    PanelEventHandler = handler;
                    var panelRaycaster = PanelEventHandler.GetComponent<PanelRaycaster>();
                    if (panelRaycaster != null)
                    {
                        panelRaycaster.enabled = false;
                        var worldCanvasRaycaster = PanelEventHandler.AddComponent<WorldSpaceUIPanelRaycaster>();
                        worldCanvasRaycaster.SetPanelSettings(PanelSettings);
                        worldCanvasRaycaster.CopyFrom(panelRaycaster);
                        Destroy(panelRaycaster);
                        _panelPanelRaycaster = worldCanvasRaycaster;
                        _panelPanelRaycaster.enabled = false;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Setup the world canvas material
        /// </summary>
        private void SetupMaterial()
        {
            if (PanelSettings.colorClearValue.a < 1.0f)
                Material = new Material(Shader.Find("Unlit/Transparent"));
            else
                Material = new Material(Shader.Find("Unlit/Texture"));

            Material.SetTexture("_MainTex", RenderTexture);
            _mainTexture2D = Material.mainTexture as Texture2D;
            MeshRenderer.sharedMaterial = Material;
        }

        /// <summary>
        /// Generate the UI (toolkit) Document
        /// </summary>
        private void AddUiDocument()
        {
            UiDocument = gameObject.AddComponent<UIDocument>();
            UiDocument.panelSettings = PanelSettings;
            UiDocument.visualTreeAsset = _visualTreeAsset;
        }

        /// <summary>
        /// Create panelSettings
        /// </summary>
        /// <param name="renderTexture">Render texture to project the UI on</param>
        /// <returns>Panel Settings</returns>
        private PanelSettings CreateWorldPanelSettings(RenderTexture renderTexture)
        {
            PanelSettings = Instantiate(_panelSettingsPrefab);
            PanelSettings.targetTexture = renderTexture;
            PanelSettings.clearColor = true;
            PanelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            PanelSettings.scale = _panelScale;
            PanelSettings.name = $"PanelSettings {name}";
            renderTexture.name = $"RenderTexture {name}";
            return PanelSettings;
        }

        /// <summary>
        /// Create a new render texture
        /// </summary>
        /// <returns></returns>
        private RenderTexture InstantiateRenderTexture()
        {
            RenderTextureDescriptor textureDescriptor = _renderTexturePrefab.descriptor;
            textureDescriptor.width = _panelWidth;
            textureDescriptor.height = _panelHeight;
            return new RenderTexture(textureDescriptor);
        }

        protected void RefreshPanelSize()
        {
            if (RenderTexture != null && (RenderTexture.width != _panelWidth || RenderTexture.height != _panelHeight))
            {
                RenderTexture.Release();
                RenderTexture.width = _panelWidth;
                RenderTexture.height = _panelHeight;
                RenderTexture.Create();

                if (UiDocument != null)
                    UiDocument.rootVisualElement?.MarkDirtyRepaint();
            }

            transform.localScale = new Vector3(_panelWidth / _pixelsPerUnit, _panelHeight / _pixelsPerUnit, 1.0f);
            _resolution = new Vector2(_panelWidth, _panelHeight);
        }

        protected void DestroyGeneratedAssets()
        {
            if (UiDocument) Destroy(UiDocument);
            if (RenderTexture) Destroy(RenderTexture);
            if (PanelSettings) Destroy(PanelSettings);
            if (Material) Destroy(Material);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (Application.isPlaying && Material != null && UiDocument != null)
            {
                if (UiDocument.visualTreeAsset != _visualTreeAsset)
                    VisualTreeAsset = _visualTreeAsset;
                if (_panelScale != PanelSettings.scale)
                    PanelSettings.scale = _panelScale;

                RefreshPanelSize();
            }
        }
#endif

        public ISelectable PickTopElement(RaycastHit hit, out Vector2 position)
        {
            return _panelPanelRaycaster.PickTopElementOfType<ISelectable>(TransformPhysicsEventForUIToolkit(hit), _resolution, out position);
        }

        public PointerEventData TransformPhysicsEventForUIToolkit(RaycastHit hit)
        {
            var eventData = new PointerEventData(EventSystem.current);
            Vector2 position = new Vector2(_panelWidth * hit.textureCoord.x, _panelHeight * hit.textureCoord.y);
            eventData.position = position;

            eventData.selectedObject = PanelEventHandler.gameObject;
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;
            raycastResult.gameObject = PanelEventHandler.gameObject;
            raycastResult.module = _panelPanelRaycaster;
            raycastResult.screenPosition = position;
            eventData.pointerCurrentRaycast = raycastResult;
            raycastResult = eventData.pointerPressRaycast;
            raycastResult.screenPosition = position;
            eventData.pointerPressRaycast = raycastResult;
            eventData.eligibleForClick = true;
            return eventData;
        }
    }
}