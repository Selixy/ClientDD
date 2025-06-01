using System.Collections.Generic;

namespace DnD.DnD_5e
{
    
    public struct Damage
    {
        public bool Magique; // L'attaque est-elle magique ? (qualificatif général)

        // Dégâts physiques
        public int Contondant;   // Bludgeoning
        public int Perforant;    // Piercing
        public int Tranchant;    // Slashing
        public int Force;        // Force pure (e.g. projectile magique)

        // Éléments
        public int Feu;          // Fire
        public int Froid;        // Cold
        public int Foudre;       // Lightning
        public int Tonnerre;     // Thunder

        // Biologiques / chimiques
        public int Acide;        // Acid
        public int Poison;       // Poison

        // Énergies magiques
        public int Radiant;      // Lumière divine
        public int Nécrotique;   // Mort/vide
        public int Psychique;    // Mental
        public int Heal;         // Soins


        public int Total()
        {
            return Contondant
                 + Perforant
                 + Tranchant 
                 + Force 
                 + Feu 
                 + Froid 
                 + Foudre 
                 + Tonnerre 
                 + Acide 
                 + Poison 
                 + Radiant 
                 + Nécrotique 
                 + Psychique
                 + Heal;
        }

        public override string ToString()
        {
            List<string> parts = new();

            if (Contondant > 0)  parts.Add($"{Contondant} contondant");
            if (Perforant > 0)   parts.Add($"{Perforant} perforant");
            if (Tranchant > 0)   parts.Add($"{Tranchant} tranchant");
            if (Force > 0)       parts.Add($"{Force} force");

            if (Feu > 0)         parts.Add($"{Feu} feu");
            if (Froid > 0)       parts.Add($"{Froid} froid");
            if (Foudre > 0)      parts.Add($"{Foudre} foudre");
            if (Tonnerre > 0)    parts.Add($"{Tonnerre} tonnerre");

            if (Acide > 0)       parts.Add($"{Acide} acide");
            if (Poison > 0)      parts.Add($"{Poison} poison");

            if (Radiant > 0)     parts.Add($"{Radiant} radiant");
            if (Nécrotique > 0)  parts.Add($"{Nécrotique} nécrotique");
            if (Psychique > 0)   parts.Add($"{Psychique} psychique");
            if (Heal > 0)        parts.Add($"{Heal} Heal");

            string type = Magique ? "[magique]" : "[non-magique]";
            return $"Dégâts {type} : " + string.Join(", ", parts);
        }
    }

    
}