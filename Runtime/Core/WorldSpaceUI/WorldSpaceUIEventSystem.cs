using System;
using System.Collections.Generic;
using System.Linq;
using Dephion.Ui.Core.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dephion.Ui.Core.WorldSpaceUI
{
    [RequireComponent(typeof(Camera))]
    public class WorldSpaceUIEventSystem : MonoBehaviour
    {
        private Camera _camera;
        private static RaycastHit[] _hits;
        [SerializeField] private float _dragThreshold = 10;
        [SerializeField] private int _maxRaycastDistance = 50;
        private bool _previousMouseDown;
        private ISelectable _currentlySelected;
        private IDraggable _currentlyDragging;
        private static int _activeWorldUiCount = 0;
        private static bool _isActive;
        private Vector2? _startDragPosition;

        private void Awake()
        {
            WorldSpaceUIDocument.DocumentSpawned += IncrementBuffer;
            WorldSpaceUIDocument.DocumentDestroyed += DecrementBuffer;
            ResetRaycastBuffer();
            _camera = GetComponent<Camera>();
            if (_camera == null)
                throw new NullReferenceException("The WorldSpaceUIEventSystem requires a Camera component!");
            _hits = Array.Empty<RaycastHit>();
        }

        private static void ResetRaycastBuffer()
        {
            _activeWorldUiCount = 0;
            _isActive = false;
            _hits = Array.Empty<RaycastHit>();
        }

        private static void DecrementBuffer()
        {
            if (!_isActive)
                return;
            _activeWorldUiCount--;
            Array.Resize(ref _hits, _activeWorldUiCount);
            _isActive = _activeWorldUiCount > 0;
        }

        private static void IncrementBuffer()
        {
            _activeWorldUiCount++;
            Array.Resize(ref _hits, _activeWorldUiCount);
            _isActive = _activeWorldUiCount > 0;
        }

        private void OnDestroy()
        {
            WorldSpaceUIDocument.DocumentSpawned -= IncrementBuffer;
            WorldSpaceUIDocument.DocumentDestroyed -= DecrementBuffer;
            ResetRaycastBuffer();
        }

        private void Update()
        {
            if (!_isActive)
                return;

            Vector3 currentPointerPos = default;
            if (TryGetTouch(0, out var touch))
                if (touch is { phase: TouchPhase.Began | TouchPhase.Moved | TouchPhase.Stationary })
                    currentPointerPos = touch.Value.position;
                else
                    currentPointerPos = Input.mousePosition;

            currentPointerPos.z = _maxRaycastDistance;
            currentPointerPos = _camera.ScreenToWorldPoint(currentPointerPos);
            var currentMouseDown = Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || touch is { phase: TouchPhase.Began };

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            Vector2? currentHitPosition = null;

            if (Physics.RaycastNonAlloc(ray, _hits, _maxRaycastDistance) > 0)
            {
                ISelectable currentlySelected = null;
                foreach (var hit in _hits.Where(h => h.collider).OrderBy(h => h.distance))
                {
                    if (hit.collider.gameObject.TryGetComponent<WorldSpaceUIDocument>(out var doc))
                    {
                        currentlySelected = doc.PickTopElement(hit, out var hitPos);
                        currentHitPosition = hitPos;
                        if (currentlySelected != null)
                            break;
                    }
                }

                Array.Clear(_hits, 0, _hits.Length);

                if (currentlySelected == null)
                {
                    _currentlySelected?.Deselect();
                    _currentlySelected = null;
                }
                else if (currentlySelected != _currentlySelected)
                {
                    if (!_previousMouseDown)
                    {
                        _currentlySelected?.Deselect();
                        _currentlySelected = currentlySelected;
                        _currentlySelected.Select();
                    }
                }
            }
            else
            {
                _currentlySelected?.Deselect();
                if (_currentlyDragging?.IsDragging == true && _startDragPosition.HasValue)
                    _currentlyDragging?.StopDrag(_startDragPosition.Value);
                _currentlySelected = null;
                _currentlyDragging = null;
            }

            if (currentMouseDown && !_previousMouseDown && _currentlySelected is IClickable ce)
                ce.Click();

            if ((Input.GetMouseButton(0) || touch is { phase: TouchPhase.Moved | TouchPhase.Stationary }) && _startDragPosition == null)
                _startDragPosition = currentHitPosition;

            if (SurpassedDraggingThreshold(currentHitPosition, _startDragPosition))
            {
                if (_currentlyDragging == null && _currentlySelected is IDraggable d)
                {
                    _currentlyDragging = d;
                    _currentlyDragging?.StartDrag(currentHitPosition.Value);
                }
                else if (_currentlyDragging?.IsDragging == true)
                    _currentlyDragging.Drag(currentHitPosition.Value);
            }
            else if (_currentlyDragging?.IsDragging == true && _startDragPosition.HasValue)
            {
                _currentlyDragging.StopDrag(_startDragPosition.Value);
                _currentlyDragging = null;
            }

            _previousMouseDown = currentMouseDown;
            if (Input.GetMouseButtonUp(0) || touch is { phase: TouchPhase.Ended })
            {
                _currentlyDragging?.StopDrag();
                _currentlyDragging = null;
                _startDragPosition = null;
            }
        }

        private bool SurpassedDraggingThreshold(Vector2? current, Vector2? previous)
        {
            return current != null && previous != null && Vector2.Distance(current.Value, previous.Value) > _dragThreshold;
        }

        private bool TryGetTouch(int index, out Touch? touch)
        {
            touch = null;
            if (Application.isMobilePlatform)
                touch = Input.GetTouch(index);
            return false;
        }

        /// <summary>
        /// Pick the top element of Type T
        /// </summary>
        /// <param name="picked">previously picked elements</param>
        /// <typeparam name="T">Type to filter by</typeparam>
        /// <returns>the top visual element of type T</returns>
        private T PickTopElementOfType<T>(List<VisualElement> picked)
        {
            return (T)(object)picked.FirstOrDefault(p => p is T);
        }
    }
}