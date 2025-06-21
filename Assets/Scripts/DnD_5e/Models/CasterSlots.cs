using System.Collections.Generic;

namespace DnD.DnD_5e
{
    public static class CasterInfo
    {
        // Index = niveau de classe (0–20), [0] est inutilisé
        public static readonly int[][] CasterSlots = new int[21][]
        {
            null, // Index 0 inutilisé pour simplifier l'accès par niveau réel
            new int[9] { 2, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[9] { 3, 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[9] { 4, 2, 0, 0, 0, 0, 0, 0, 0 },
            new int[9] { 4, 3, 0, 0, 0, 0, 0, 0, 0 },
            new int[9] { 4, 3, 2, 0, 0, 0, 0, 0, 0 },
            new int[9] { 4, 3, 3, 0, 0, 0, 0, 0, 0 },
            new int[9] { 4, 3, 3, 1, 0, 0, 0, 0, 0 },
            new int[9] { 4, 3, 3, 2, 0, 0, 0, 0, 0 },
            new int[9] { 4, 3, 3, 3, 1, 0, 0, 0, 0 },
            new int[9] { 4, 3, 3, 3, 2, 0, 0, 0, 0 },
            new int[9] { 4, 3, 3, 3, 2, 1, 0, 0, 0 },
            new int[9] { 4, 3, 3, 3, 2, 1, 0, 0, 0 },
            new int[9] { 4, 3, 3, 3, 2, 1, 1, 0, 0 },
            new int[9] { 4, 3, 3, 3, 2, 1, 1, 0, 0 },
            new int[9] { 4, 3, 3, 3, 2, 1, 1, 1, 0 },
            new int[9] { 4, 3, 3, 3, 2, 1, 1, 1, 0 },
            new int[9] { 4, 3, 3, 3, 2, 1, 1, 1, 1 },
            new int[9] { 4, 3, 3, 3, 3, 1, 1, 1, 1 },
            new int[9] { 4, 3, 3, 3, 3, 2, 1, 1, 1 },
            new int[9] { 4, 3, 3, 3, 3, 2, 2, 1, 1 }
        };
    }
}