using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR;
using ZXing;
using TMPro;
using PassthroughCameraSamples;

public class QrCodeDetection : MonoBehaviour
{
    [Header("QR Code Settings")]
    [SerializeField] private WebCamTextureManager webCamManager;
    [SerializeField] private int scanFrameFrequency = 10;
    [SerializeField] private EnvironmentRaycastManager raycastManager;

    [Header("Debug UI")]
    [SerializeField] private TMP_Text debugText;

    [Header("Optional Visual Debug Cube")]
    [SerializeField] private GameObject testCube;

    [System.Serializable]
    public struct QrCodeTarget
    {
        public string QrCodeContent;
        public Transform Object;
    }

    [SerializeField] private List<QrCodeTarget> qrCodeTargets = new List<QrCodeTarget>();
    private Dictionary<string, Transform> qrCodeTargetsDic = new Dictionary<string, Transform>();
    private HashSet<string> placedQRCodes = new HashSet<string>();

    private BarcodeReader barcodeReader = new BarcodeReader();
    private bool isCameraReady = false;

    private IEnumerator Start()
    {
        DebugLog("🔄 Initializing QR Scanner...");

        foreach (var target in qrCodeTargets)
        {
            if (!qrCodeTargetsDic.ContainsKey(target.QrCodeContent))
                qrCodeTargetsDic.Add(target.QrCodeContent, target.Object);
        }

        while (webCamManager.WebCamTexture == null)
        {
            DebugLog("📷 Waiting for WebCamTexture...");
            yield return null;
        }

        isCameraReady = true;
        DebugLog("✅ Camera ready.");
    }

    private void Update()
    {
        if (!isCameraReady || Time.frameCount % scanFrameFrequency != 0)
            return;

        WebCamTexture cam = webCamManager.WebCamTexture;

        if (cam == null || cam.width <= 16 || cam.height <= 16)
        {
            DebugLog("❌ WebCamTexture not initialized properly.");
            return;
        }

        Color32[] pixels = cam.GetPixels32();
        if (pixels.Length == 0)
        {
            DebugLog("⚠️ No pixel data from camera.");
            return;
        }

        Result result = barcodeReader.Decode(pixels, cam.width, cam.height);

        if (result != null)
        {
            DebugLog($"✅ QR Detected: {result.Text}");

            // Skip if we've already placed this QR code
            if (placedQRCodes.Contains(result.Text))
            {
                DebugLog($"📌 Skipped — '{result.Text}' already placed.");
                return;
            }

            Vector2Int qrCenter = GetQrCenter(result.ResultPoints, cam.height);
            Pose qrPose = ConvertToWorldPose(qrCenter);
            Vector3 safePosition = qrPose.position;
            safePosition.y = Mathf.Max(safePosition.y, 0.1f); // Clamp Y

            if (qrCodeTargetsDic.TryGetValue(result.Text, out Transform target))
            {
                target.SetPositionAndRotation(safePosition, qrPose.rotation);
                target.gameObject.SetActive(true);
                placedQRCodes.Add(result.Text);

                DebugLog($"🧱 Placed: {target.name} at {safePosition}");
            }
            else
            {
                DebugLog($"❗ Unmapped QR: {result.Text}");
            }

            if (testCube != null)
            {
                testCube.transform.SetPositionAndRotation(safePosition + new Vector3(0, 0.05f, 0), qrPose.rotation);
                testCube.SetActive(true);
            }
        }
        else
        {
            DebugLog("❌ No QR detected in this frame.");
        }
    }

    private Vector2Int GetQrCenter(ResultPoint[] points, int height)
    {
        if (points == null || points.Length == 0)
            return Vector2Int.zero;

        float sumX = 0f, sumY = 0f;
        foreach (var pt in points)
        {
            sumX += pt.X;
            sumY += pt.Y;
        }

        int x = Mathf.RoundToInt(sumX / points.Length);
        int y = Mathf.RoundToInt(height - (sumY / points.Length));

        return new Vector2Int(x, y);
    }

    private Pose ConvertToWorldPose(Vector2Int screenPoint)
    {
        Ray ray = PassthroughCameraUtils.ScreenPointToRayInWorld(webCamManager.Eye, screenPoint);
        Debug.DrawRay(ray.origin, ray.direction * 5f, Color.red, 2f);

        if (raycastManager.Raycast(ray, out EnvironmentRaycastHit hit))
        {
            DebugLog($"🟢 Raycast HIT at {hit.point}");
            DebugMarkerAt(hit.point);
            return new Pose(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        }

        DebugLog("🔴 Raycast FAILED — no collider hit.");
        return Pose.identity;
    }

    private void DebugMarkerAt(Vector3 position)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * 0.05f;

        Renderer r = marker.GetComponent<Renderer>();
        if (r) r.material.color = Color.green;

        Destroy(marker, 5f);
    }

    private void DebugLog(string msg)
    {
        Debug.Log(msg);
        if (debugText != null)
        {
            debugText.text = msg;
        }
    }
}
