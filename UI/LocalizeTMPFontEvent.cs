using UnityEngine;
using UnityEngine.Events;
using System;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

[Serializable]
public class LocalizedTMPFont : LocalizedAsset<TMP_FontAsset> { }
[Serializable]
public class UnityEventTMPFont : UnityEvent<TMP_FontAsset> { }

[AddComponentMenu("Localization/Asset/Localize TMP Font Event")]
public class LocalizeTMPFontEvent : LocalizedAssetEvent<TMP_FontAsset, LocalizedTMPFont, UnityEventTMPFont>{}
