using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArzitPlus
{
    [BroadcastReceiver]
    public class ChargingStateReceiver : BroadcastReceiver
    {
        public event EventHandler<bool> ChargingStateChanged;

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBatteryChanged)
            {
                int status = intent.GetIntExtra(BatteryManager.ExtraStatus, -1);
                int level = intent.GetIntExtra(BatteryManager.ExtraLevel, -1);
                int scale = intent.GetIntExtra(BatteryManager.ExtraScale, -1);

                float batteryPercentage = (level / (float)scale) * 100;

                bool isCharging = status == (int)BatteryStatus.Charging || status == (int)BatteryStatus.Full;
                bool isBatteryLow = batteryPercentage <= 15;

                ChargingStateChanged?.Invoke(this, isCharging);

                if (isBatteryLow && !isCharging)
                {
                    // Display toast notification
                    Toast.MakeText(context, "Battery level is low. Please charge your phone.", ToastLength.Short).Show();
                }
            }
        }
    }

}