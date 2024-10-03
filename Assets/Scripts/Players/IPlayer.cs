using UnityEngine;

namespace Arkanodia.Players
{
    public interface IPlayer
    {
        public int Life { get; set; }
        public void ChangeLife(int health);
        Vector3 Position { get; set; }
    }
}