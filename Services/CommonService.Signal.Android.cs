#if ANDROID
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.Telephony;
using static Android.Telephony.TelephonyCallback;

namespace KavaPryct.Services   // ← debe coincidir
{
    public static partial class CommonService
    {
        private static partial SignalInfo PlatformGetSignalInfo()
        {
            var ctx = global::Android.App.Application.Context; // evita ambigüedad con MAUI Application

            var cm = (ConnectivityManager)ctx.GetSystemService(Context.ConnectivityService)!;
            var active = cm.ActiveNetwork;
            if (active is null) return new SignalInfo(NetTransport.None, null, null, null);

            var caps = cm.GetNetworkCapabilities(active);
            if (caps is null) return new SignalInfo(NetTransport.Other, null, null, null);

            // Wi-Fi
            if (caps.HasTransport(TransportType.Wifi))
            {
                var wifi = (WifiManager?)ctx.GetSystemService(Context.WifiService);
#pragma warning disable CA1416
                var info = wifi?.ConnectionInfo;
#pragma warning restore CA1416

                if (info is null)
                    return new SignalInfo(NetTransport.Wifi, null, null, null);

                int rssi = info.Rssi;                                // dBm
                int bars = WifiManager.CalculateSignalLevel(rssi, 5); // 0..4
                var ssid = info.SSID;                                // puede ser "<unknown ssid>" sin permisos

                return new SignalInfo(NetTransport.Wifi, bars, rssi, ssid);
            }

            if (caps.HasTransport(TransportType.Cellular))
            {
                try
                {
                    var telMgr = (TelephonyManager)Android.App.Application.Context.GetSystemService(Context.TelephonyService);
                    if (telMgr == null)
                        return new SignalInfo(NetTransport.Cellular, null, null, null);

                    var listener = new SignalStrengthListener();
                    telMgr.Listen(listener, PhoneStateListenerFlags.SignalStrengths);

#pragma warning disable CA1416
                    var signalStrength = telMgr.SignalStrength.Level;
#pragma warning restore CA1416

                    if (signalStrength != null)
                    {
                        // Algunas API devuelven valores negativos (dBm)
                        // Nivel: 0..4 (como las “barras” del sistema)
                        //int level = signalStrength.Level;
                        int? dbm = signalStrength;





                        return new SignalInfo(NetTransport.Cellular, dbm, null, telMgr.NetworkOperatorName);
                    }
                }
                catch
                {
                    return new SignalInfo(NetTransport.Cellular, null, null, null);
                }
            }

            if (caps.HasTransport(TransportType.Ethernet))
                return new SignalInfo(NetTransport.Ethernet, null, null, null);

            return new SignalInfo(NetTransport.Other, null, null, null);
        }
    }

    public class SignalStrengthListener : PhoneStateListener
    {
        public int SignalStrengthLevel { get; private set; }

        public override void OnSignalStrengthsChanged(SignalStrength signalStrength)
        {
            SignalStrengthLevel = signalStrength.Level; // Obtiene el nivel de señal
        }
    }
}
#endif

