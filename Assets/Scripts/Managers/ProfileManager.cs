using System;

namespace Managers
{
    public class ProfileManager : Singleton<ProfileManager>
    {
        public Profile Profile;

        protected override void Awake()
        {
            base.Awake();
            Profile = Profile.GetOrCreate();
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            Profile.Save();
        }
    }
}