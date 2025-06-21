using System.Collections.Generic;

namespace DnD.DnD_5e
{   
    public struct LvlUpClass
    {
        public List<Etat_DnD_5e>   PassiveEffect   { get; set; }  // État permanent (compétence, aura…)
        public List<Skill_DnD_5e>  Skills          { get; set; }  // Skills utilisable
        public State               StatIncrease    { get; set; }  // Augmentation de caractéristiques
        public int                 ExtraCritDice   { get; set; }  // Dés supplémentaires aux critiques
        public int                 ExtraAttacks    { get; set; }  // Atk supplémentaires par action
        public bool                CarctIncrease   { get; set; }  // Augmentation +2 ou don

        public LvlUpClass(List<Etat_DnD_5e>      passive         = null
                         ,List<Skill_DnD_5e>     skills          = null
                         ,State                  stat            = new State()
                         ,int                    crit            = 0
                         ,int                    ExtraAttacks    = 0
                         ,bool                   carctIncrease   = false
                         )
        {
            this.PassiveEffect  = passive;
            this.Skills         = skills;
            this.StatIncrease   = stat;
            this.ExtraCritDice  = crit;
            this.CarctIncrease  = carctIncrease;
            this.ExtraAttacks   = ExtraAttacks;
        }   
    }
}