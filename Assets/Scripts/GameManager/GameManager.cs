using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DnD.DnD_5e;


public partial class GameManager : MonoBehaviour
{
    // Liste de joueurs
    public List<Player_DnD_5e> players = new List<Player_DnD_5e>();
}

public static class EntityRegistry
{
    public static List<Entity> All = new();

    public static void Register(Entity e) => All.Add(e);
    public static void Unregister(Entity e) => All.Remove(e);
    public static void Clear() => All.Clear();
}