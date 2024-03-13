using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Content;
using Android.Widget;
using System;
using Android.Hardware;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArzitPlus
{
    public interface IShakeListener
    {
        void OnShakeDetected();
    }
    public class ShakeDetector : Java.Lang.Object, ISensorEventListener
    {
        private const int ShakeThreshold = 800; // Adjust this value to set the sensitivity of the shake

        private long lastUpdateTime;
        private float lastX;
        private float lastY;
        private float lastZ;
        private readonly Context context;
        private readonly IShakeListener shakeListener;

        public ShakeDetector(Context context, IShakeListener shakeListener)
        {
            this.context = context;
            this.shakeListener = shakeListener;
        }

        public IShakeListener IShakeListener
        {
            get => default;
            set
            {
            }
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            // Not used
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                long currentTime = SystemClock.ElapsedRealtime();
                long timeDifference = currentTime - lastUpdateTime;

                if (timeDifference > 100)
                {
                    lastUpdateTime = currentTime;

                    float x = e.Values[0];
                    float y = e.Values[1];
                    float z = e.Values[2];

                    float acceleration = Math.Abs(x + y + z - lastX - lastY - lastZ) / timeDifference * 10000;

                    if (acceleration > ShakeThreshold)
                    {
                        shakeListener.OnShakeDetected();
                    }

                    lastX = x;
                    lastY = y;
                    lastZ = z;
                }
            }
        }
    }
}