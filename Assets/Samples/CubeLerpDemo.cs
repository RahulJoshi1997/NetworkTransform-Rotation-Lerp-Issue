using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CubeLerpDemo : MonoBehaviour {
    [SerializeField] private NetworkTransform _netCubePrefab;
    [SerializeField] private Transform _cubeStartPosAnchor;
    [SerializeField] private Transform _cubeEndPosAnchor;
    [SerializeField] private float _animDuration;

    private NetworkTransform _netCube;
    private Coroutine _prevLerper;

    void Start() {
        enabled = false;
        if (NetworkManager.Singleton.IsServer)
            OnServerStarted();
        else
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted() {
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        _netCube = Instantiate(_netCubePrefab, _cubeStartPosAnchor.position, _cubeStartPosAnchor.rotation);
        _netCube.NetworkObject.Spawn();
        enabled = true;
    }

    private void OnGUI() {
        if (GUI.Button(new Rect(10, 70, 300, 20),"Lerp Cube")) {
            if (_prevLerper != null)
                StopCoroutine(_prevLerper);
            _prevLerper = StartCoroutine(LerpCube(_animDuration));
        }
    }

    private IEnumerator LerpCube(float duration) {
        Rigidbody netcubeRB = _netCube.GetComponent<Rigidbody>();

        _cubeStartPosAnchor.rotation = Random.rotationUniform;
        _netCube.transform.SetPositionAndRotation(_cubeStartPosAnchor.position, _cubeStartPosAnchor.rotation);
        Vector3 torque = Random.insideUnitSphere * 10f;//new Vector3(7.90f, -0.10f, 0.00f);//
        netcubeRB.isKinematic = false;
        netcubeRB.AddTorque(torque, ForceMode.VelocityChange);
        //Debug.Log(torque);

        yield return new WaitForSeconds(1f);
        _cubeStartPosAnchor.rotation = _netCube.transform.rotation;

        float t = 0;
        while (t < 1) {
            t = Mathf.Clamp01(t + Time.deltaTime / duration);
            netcubeRB.isKinematic = true;
            _netCube.transform.SetPositionAndRotation(
                Vector3.Lerp(_cubeStartPosAnchor.position, _cubeEndPosAnchor.position, t),
                Quaternion.Euler(Vector3.Lerp(_cubeStartPosAnchor.eulerAngles, _cubeEndPosAnchor.eulerAngles, t))
            );

            yield return null;
        }
    }
}
