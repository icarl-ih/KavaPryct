using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Networking.Connectivity;


namespace KavaPryct.Services
{
    public static partial class CommonService
    {
        private static partial SignalInfo PlatformGetSignalInfo()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile is null) return new SignalInfo(NetTransport.None, null, null, null);

            int? bars = profile.GetSignalBars(); // 0..5 o null
            // Normalizamos a 0..4 para parecerse a Android
            int? normBars = bars.HasValue ? Math.Max(0, Math.Min(4, bars.Value)) : null;

            var adapter = profile.NetworkAdapter;
            var transport = NetTransport.Other;

            // IANA types comunes
            // 71 = Wi-Fi, 6 = Ethernet, 243/244 = WWAN (celular)
            var type = adapter?.IanaInterfaceType ?? 0;
            transport = type switch
            {
                71 => NetTransport.Wifi,
                6  => NetTransport.Ethernet,
                243 or 244 => NetTransport.Cellular,
                _ => NetTransport.Other
            };

            return new SignalInfo(transport, normBars, null, profile.ProfileName);
        }
    }
}
#endif