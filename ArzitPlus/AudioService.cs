using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;

namespace ArzitPlus
{
    [Service]
    public class AudioService : Service
    {
        private MediaPlayer mediaPlayer;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            string action = intent.GetStringExtra("Action");

            if (action == "Pause")
            {
                mediaPlayer.Pause();
            }
            else if (action == "Resume")
            {
                mediaPlayer.Start();
            }
            else
            {
                mediaPlayer = MediaPlayer.Create(this, Resource.Raw.lofi);
                mediaPlayer.Looping = true;
                mediaPlayer.Start();
            }
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            mediaPlayer.Stop();
            mediaPlayer.Release();
            mediaPlayer = null;
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}
