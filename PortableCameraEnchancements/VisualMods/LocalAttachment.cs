using System.Collections.Generic;
using UnityEngine;
using ABI_RC.Systems.Camera.VisualMods;
using MelonLoader;
using ABI_RC.Systems.Camera;

namespace Sketch.PortableCameraEnchancements.VisualMods
{
    public class LocalAttachment : ICameraVisualMod
    {
        private readonly string _name = "Local Attachment";

        private Sprite _image;
        private bool _isEnabled;

        private PortableCamera _portableCamera;
        private UnityEngine.Camera _camera;
        private Transform _originalParent;

        private Matrix4x4 _cachedWorldMatrix;

        public Dictionary<string, Dictionary<string, string>> Translation = new()
        {
            {
                "en", new Dictionary<string, string> { { "Local Attachment", "Local Attachment" } }
            }
        };

        public string GetModName(string language)
        {
            if (Translation.ContainsKey(language) && Translation[language].ContainsKey(_name))
                return Translation[language][_name];

            return _name;
        }

        public Sprite GetModImage() => _image;

        public int GetSortingOrder() => 50;

        public bool ActiveIsOrange() => false;

        public bool DefaultIsOn() => false;

        public void Setup(PortableCamera camera, UnityEngine.Camera cameraComponent)
        {
            _portableCamera = camera;
            _camera = cameraComponent;
            _originalParent = _camera.transform.parent;
            _image = camera.interfaceIcons.Find(x => x.key == "AttachmentIcon")?.image;
            Disable();
        }

        public void Enable()
        {
            if (_portableCamera.IsAttachedToWorldSpace)
            {
                // Cache world transform
                _cachedWorldMatrix = _camera.transform.localToWorldMatrix;

                // Detach from world pin
                _portableCamera.DisableModByType(typeof(CameraAttachment));
                ApplyWorldMatrix(_camera.transform, _cachedWorldMatrix);
            }

            _isEnabled = true;

            var playerLocal = GameObject.Find("_PLAYERLOCAL")?.transform;
            if (playerLocal != null)
            {
                _camera.transform.SetParent(playerLocal, true);
            }
            else
            {
                // Fallback to default behavior
                _portableCamera.EnableModByType(typeof(CameraAttachment));
                Disable();
                return;
            }

            _portableCamera.DisableModByType(typeof(ImageStabilization));
            _portableCamera.SetDroneVisibility(true);
        }

        public void Disable()
        {
            _isEnabled = false;

            if (!_portableCamera.IsAttachedToWorldSpace)
            {
                _camera.transform.SetParent(_originalParent, false);
                _camera.transform.localPosition = Vector3.zero;
                _portableCamera.SetDroneVisibility(false);
            }

            _portableCamera.ApplyFlip(false);
            _portableCamera.DisableModByType(typeof(DroneMode));
            _portableCamera.DisableModByType(typeof(AutoOrbit));
        }

        private void ApplyWorldMatrix(Transform t, Matrix4x4 m)
        {
            t.position = m.GetColumn(3); // Translation

            t.rotation = Quaternion.LookRotation(
                m.GetColumn(2), // Forward (Z)
                m.GetColumn(1)  // Up (Y)
            );

            t.localScale = new Vector3(
                m.GetColumn(0).magnitude,
                m.GetColumn(1).magnitude,
                m.GetColumn(2).magnitude
            );
        }
    }
}
