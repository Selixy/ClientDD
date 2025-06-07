using System;

public struct Dice
{
    public int Number;
    public int Value;

    private static Random rng = new Random();

    private int RollOnce()
    {
        return rng.Next(1, Value + 1);
    }

    public int Roll()
    {
        int total = 0;
        for (int i = 0; i < Number; i++)
        {
            total += RollOnce();
        }
        return total;
    }
}
