using Klak.Hap;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Karbon {

public sealed class FlipbookSourceSelector : MonoBehaviour
{
    [SerializeField] string[] _videoFiles = null;
    [SerializeField] float _playbackSpeed = 10;
    [SerializeField] MeshRenderer _cameraQuad = null;
    [SerializeField] MeshRenderer _videoQuad = null;

    GameObject _playerObject;
    int _current;

    void Update()
    {
        var num = GetInputNumber();
        if (num < 0 || num == _current) return;

        if (num == 0)
        {
            _cameraQuad.enabled = true;
            _videoQuad.enabled = false;
        }
        else
        {
            RebuildPlayer(num - 1);
            _cameraQuad.enabled = false;
            _videoQuad.enabled = true;
        }

        _current = num;
    }

    void RebuildPlayer(int videoIndex)
    {
        if (_playerObject != null) Destroy(_playerObject);

        _playerObject = new GameObject("HAP Player");
        _playerObject.transform.SetParent(transform, false);

        var filePath = _videoFiles[videoIndex % _videoFiles.Length];

        var player = _playerObject.AddComponent<HapPlayer>();
        player.targetRenderer = _videoQuad;
        player.targetMaterialProperty = "_BaseMap";
        player.speed = _playbackSpeed;
        player.loop = true;
        player.Open(filePath, HapPlayer.PathMode.StreamingAssets);
    }

    static int GetInputNumber()
    {
        var keyboard = Keyboard.current;
        if (keyboard.digit1Key.wasPressedThisFrame) return 0;
        if (keyboard.digit2Key.wasPressedThisFrame) return 1;
        if (keyboard.digit3Key.wasPressedThisFrame) return 2;
        if (keyboard.digit4Key.wasPressedThisFrame) return 3;
        if (keyboard.digit5Key.wasPressedThisFrame) return 4;
        if (keyboard.digit6Key.wasPressedThisFrame) return 5;
        if (keyboard.digit7Key.wasPressedThisFrame) return 6;
        if (keyboard.digit8Key.wasPressedThisFrame) return 7;
        if (keyboard.digit9Key.wasPressedThisFrame) return 8;
        return -1;
    }
}

} // namespace Karbon
