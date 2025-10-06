using System;
using System.Collections.Generic;

using ChipSecuritySystem;

class Program
{
    static void Main(string[] args)
    {
        var chips = TryGetProvidedChips() ?? GetSampleChips();

        var result = ChipChainSolver.FindLongestBlueToGreenChain(chips);

        if (!result.HasPath)
        {
            Console.WriteLine("No valid Blue→Green chain exists.");
            return;
        }

        var parts = new List<string> { "Blue" };
        foreach (var (chip, flipped) in result.Enumerate(chips))
        {
            var left = chip.StartColor;
            var right = chip.EndColor;
            parts.Add(flipped ? $"[{right}, {left}]" : $"[{left}, {right}]");
        }
        parts.Add("Green");

        Console.WriteLine(string.Join(" ", parts));
        Console.WriteLine($"Chips used: {result.PathIndices.Count}/{chips.Count}");
    }

    private static List<ColorChip> GetSampleChips() => new()
    {
    new ColorChip(Color.Blue,   Color.Yellow),
        new ColorChip(Color.Red,    Color.Orange),
        new ColorChip(Color.Yellow, Color.Red),
        new ColorChip(Color.Orange, Color.Purple),
        new ColorChip(Color.Purple, Color.Green),
    };

    private static List<ColorChip>? TryGetProvidedChips()
    {
        return null;
    }
}
