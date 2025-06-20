namespace DnD.DnD_5e
{ 
    public struct SkillData
    {
        public int MasteryLevel; // 0 = pas maîtrisé, 1 = maîtrise, 2 = expertise
        public int FlatBonus;    // bonus fixe (ex: objet, race...)

        public SkillData(int masteryLevel, int flatBonus = 0)
        {
            MasteryLevel = masteryLevel;
            FlatBonus = flatBonus;
        }
    }
}
