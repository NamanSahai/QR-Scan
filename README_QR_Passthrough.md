
# QR Code Passthrough AR with Meta Quest

This is a Unity VR project built for Meta Quest 3 that uses passthrough camera access and QR code detection to place 3D models on real-world surfaces. It combines Meta XR's passthrough tools with ZXing barcode scanning and raycasting to create a simple yet powerful AR interaction.

Once a QR code is scanned using the passthrough feed, the system casts a ray into the real world from the QR center and anchors a virtual object (like a model or cube) on any detected surface. The object stays anchored and can be adjusted manually after its first placement.

---

## 🔹 Key Features

- 🟡 Real-time QR detection from passthrough stream
- 🟢 Object placement using raycast surface hit
- 🧱 Adjustable object transforms after placement
- 🔍 Ray + debug visual markers to confirm detection
- 📝 TextMeshPro UI logging for in-headset feedback

---

## 🔧 Requirements

- Unity 2022.3 LTS or Unity 6 (URP)
- Meta Quest 3 (or 2/Pro with passthrough)
- Meta XR All-in-One SDK
- ZXing.Net (for QR code detection)
- Android Build Support enabled in Unity

---

## ▶️ How to Use

1. Clone or download this repository
2. Open it in Unity Hub
3. Switch build target to **Android**
4. Set XR Plugin to **Meta XR**
5. Connect Quest via Link or USB
6. Build and Run the `SampleScene`
7. Scan a QR → the linked object appears on the surface

---

## 📁 Project Structure

```
QR-Scan/
├── Assets/
│   ├── MetaXR/                        # Meta SDK assets
│   ├── PassthroughCameraApiSamples/   # Camera access + passthrough logic
│   ├── QRCodeExamples/                # QR scanning examples
│   ├── Scripts/                       # Main script (QrCodeDetection.cs)
│   ├── model/                         # 3D models to spawn
│   ├── Scenes/                        # Sample scene
│   ├── Resources/                     # ZXing barcode libraries
│   ├── TextMesh Pro/                  # UI logging
│   ├── StreamingAssets/              # Optional runtime files
│   └── Settings/, XR/, Plugins/, etc.
├── Packages/
│   ├── manifest.json                  # Declares all dependencies
├── ProjectSettings/
│   └── XRPlugin, URP, input configs
├── .gitignore                         # Ignores Library, Build, Logs
├── README.md                          # You’re reading it :)
└── LICENSE (optional)
```

---

## 🛠 Credits

- Built by Naman Sahai  
- Meta XR SDK (All-in-One)  
- ZXing.Net open-source barcode scanner  
- Unity URP 3D Core Template

---

## 📌 Notes

- Tested on Meta Quest 3
- Works best with good lighting and clear QR codes
- Once scanned, objects won’t reposition again (unless reset manually)
