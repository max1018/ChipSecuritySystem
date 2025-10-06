using System;
using System.Collections.Generic;
using System.Linq;

namespace ChipSecuritySystem;

/// <summary>
/// Provides search utilities for building the longest Blue-to-Green chip chain.
/// </summary>
public static class ChipChainSolver
{
    public static ChainResult FindLongestBlueToGreenChain(IReadOnlyList<ColorChip> chips)
    {
        if (chips is null) throw new ArgumentNullException(nameof(chips));

        if (chips.Count == 0)
        {
            return ChainResult.Empty;
        }

        var sides = chips.Select(c => (c.StartColor, c.EndColor)).ToArray();

        var byColor = new Dictionary<Color, List<int>>();
        void Add(Color color, int index)
        {
            if (!byColor.TryGetValue(color, out var list))
            {
                list = new List<int>();
                byColor[color] = list;
            }
            list.Add(index);
        }

        for (int i = 0; i < sides.Length; i++)
        {
            Add(sides[i].Item1, i);
            if (sides[i].Item2 != sides[i].Item1)
            {
                Add(sides[i].Item2, i);
            }
        }

        var used = new bool[sides.Length];
        var path = new List<int>(sides.Length);
        var flipped = new List<bool>(sides.Length);
        var best = ChainResult.Empty;

        int RemainingCount() => used.Count(u => !u);

        void Dfs(Color current)
        {
            if (current.Equals(Color.Green))
            {
                if (path.Count > best.PathIndices.Count)
                {
                    best = ChainResult.From(path, flipped);
                }

                return;
            }

            if (byColor.TryGetValue(current, out var candidates))
            {
                foreach (var index in candidates)
                {
                    if (used[index])
                    {
                        continue;
                    }

                    var (left, right) = sides[index];

                    if (path.Count + RemainingCount() <= best.PathIndices.Count)
                    {
                        break;
                    }

                    if (left.Equals(current) || right.Equals(current))
                    {
                        bool flip = right.Equals(current);
                        var next = flip ? left : right;

                        used[index] = true;
                        path.Add(index);
                        flipped.Add(flip);

                        Dfs(next);

                        flipped.RemoveAt(flipped.Count - 1);
                        path.RemoveAt(path.Count - 1);
                        used[index] = false;
                    }
                }
            }
        }

        Dfs(Color.Blue);
        return best;
    }
}

public sealed record ChainResult(IReadOnlyList<int> PathIndices, IReadOnlyList<bool> Flipped)
{
    public static ChainResult Empty { get; } = new ChainResult(Array.Empty<int>(), Array.Empty<bool>());

    public bool HasPath => PathIndices.Count > 0;

    public static ChainResult From(IList<int> indices, IList<bool> flipped)
    {
        return new ChainResult(indices.ToArray(), flipped.ToArray());
    }

    public IEnumerable<(ColorChip Chip, bool IsFlipped)> Enumerate(IReadOnlyList<ColorChip> chips)
    {
        if (chips is null) throw new ArgumentNullException(nameof(chips));
        if (PathIndices.Count != Flipped.Count)
        {
            throw new InvalidOperationException("Path and orientation lengths differ.");
        }

        for (int i = 0; i < PathIndices.Count; i++)
        {
            yield return (chips[PathIndices[i]], Flipped[i]);
        }
    }
}
