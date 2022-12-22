﻿using Managers;

namespace Game
{
    public class DoorSaveData : MapData
    {
        public Rank Rank;

        public DoorSaveData(Rank rank)
        {
            Rank = rank;
        }

        public override void OnReact()
        {
            PlaySoundEffect("door");
            Destroyed();
            
            /*base.OnReact();
            if (GameManager.Instance.PlayerData.Keys[Rank] > 0)
            {
                GameManager.Instance.PlayerData.Keys[Rank] -= 1;
                Destroyed();
            }
            else
            {
                WindowManager.Instance.Warn("No Key");
            }*/
        }
    }
}