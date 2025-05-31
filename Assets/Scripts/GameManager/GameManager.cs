using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DnD.DnD_5e;

namespace DnD
{
    public partial class GameManager : MonoBehaviour
    {
        // Liste de joueurs
        public List<PlayerDND5e> players = new List<PlayerDND5e>();
    }
}
