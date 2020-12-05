using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

Puzzle_1_1();
Puzzle_1_2();
Puzzle_2_1();
Puzzle_2_2();
Puzzle_3_1();
Puzzle_3_2();
Puzzle_4_1();
Puzzle_4_2();
Puzzle_5_1();
Puzzle_5_2();

static void Puzzle_1_1()
{
    var combination = File.ReadAllLines(@"input-01")
        .Select(l => int.Parse(l.Trim()))
        .ToCombinationsOf(2)
        .First(c => c.Sum() == 2020);

    Console.WriteLine(combination.Aggregate((a, x) => a * x));
}

static void Puzzle_1_2()
{
    var combination = File.ReadAllLines(@"input-01")
    .Select(l => int.Parse(l.Trim()))
    .ToCombinationsOf(3)
    .First(c => c.Sum() == 2020);

    Console.WriteLine(combination.Aggregate(1, (a, x) => a * x));
}

static void Puzzle_2_1()
{
    var count = File.ReadAllLines(@"input-02")
        .Where(l =>
        {
            var match = Regex.Match(l, @"(\d+)-(\d+)\s+(\w):\s+(.*)");
            var minCount = int.Parse(match.Groups[1].Value);
            var maxCount = int.Parse(match.Groups[2].Value);
            var character = match.Groups[3].Value[0];
            var password = match.Groups[4].Value;

            var count = password.Count(c => c == character);
            return minCount <= count && count <= maxCount;
        })
        .Count();

    Console.WriteLine(count);
}

static void Puzzle_2_2()
{
    var count = File.ReadAllLines(@"input-02")
        .Where(l =>
        {
            var match = Regex.Match(l, @"(\d+)-(\d+)\s+(\w):\s+(.*)");
            var firstPosition = int.Parse(match.Groups[1].Value);
            var secondPosition = int.Parse(match.Groups[2].Value);
            var character = match.Groups[3].Value[0];
            var password = match.Groups[4].Value;

            return password[firstPosition - 1] == character ^ password[secondPosition - 1] == character;
        })
        .Count();

    Console.WriteLine(count);
}

static void Puzzle_3_1()
{
    var count = Puzzle_3_Traverse(3, 1);
    Console.WriteLine(count);
}

static void Puzzle_3_2()
{
    (int Right, int Down)[] slopes = new[]
    {
        (1, 1),
        (3, 1),
        (5, 1),
        (7, 1),
        (1, 2)
    };

    var result = slopes.Select(s => Puzzle_3_Traverse(s.Right, s.Down))
        .Aggregate((a, x) => a * x);
    Console.WriteLine(result);
}

static int Puzzle_3_Traverse(int right, int down)
{
    var lines = File.ReadAllLines(@"input-03");
    var row = 0;
    var column = 0;
    var count = 0;
    while (row < lines.Length)
    {
        var line = lines[row];
        var index = column % line.Length;
        if (line[index] == '#')
        {
            count++;
        }
        row += down;
        column += right;
    }

    return count;
}

static void Puzzle_4_1()
{
    var fieldNames = new[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
    var input = File.ReadAllText(@"input-04").Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    var validCount = 0;
    foreach (var passportData in input)
    {
        var fields = passportData.Split(' ', '\n');
        var valueCount = 0;
        foreach (var fieldName in fieldNames)
        {
            if (fields.Any(f => Regex.IsMatch(f, $"{fieldName}:[^\\s]+")))
            {
                valueCount++;
            }
        }
        if (valueCount == fieldNames.Length)
        {
            validCount++;
        }
    }

    Console.WriteLine(validCount);
}

static void Puzzle_4_2()
{
    bool hgtIsValid(string s)
    {
        var match = Regex.Match(s, @"^(\d+)(cm|in)$");
        if (match.Success)
        {
            var number = int.Parse(match.Groups[1].Value);
            if (match.Groups[2].Value == "cm")
            {
                return 150 <= number && number <= 193;
            }
            if (match.Groups[2].Value == "in")
            {
                return 59 <= number && number <= 76;
            }
        }
        return false;
    };

    var validators = new Dictionary<string, Func<string, bool>>
    {
        { "byr", s => int.TryParse(s, out int byr) && 1920 <= byr && byr <= 2002 },
        { "iyr", s => int.TryParse(s, out int iyr) && 2010 <= iyr && iyr <= 2020 },
        { "eyr", s => int.TryParse(s, out int eyr) && 2020 <= eyr && eyr <= 2030 },
        { "hgt", hgtIsValid },
        { "hcl", s => Regex.IsMatch(s, @"^#[a-f0-9]{6}$") },
        { "ecl", s => Regex.IsMatch(s, @"^(amb|blu|brn|gry|grn|hzl|oth)$") },
        { "pid", s => Regex.IsMatch(s, @"^[0-9]{9}$") }
    };

    var input = File.ReadAllText(@"input-04").Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    var count = input.Where(passport =>
    {
        var fields = passport.Split(' ', '\n');
        var validFieldsCount = fields.Where(f =>
        {
            var parts = f.Split(':');
            return validators.ContainsKey(parts[0]) && validators[parts[0]](parts[1]);
        }).Count();
        return validFieldsCount == validators.Count;
    }).Count();

    Console.WriteLine(count);
}

static void Puzzle_5_1()
{
    var result = File.ReadAllLines(@"input-05")
        .Select(Puzzle_5_ParseLine)
        .Max(x => x.SeatId);

    Console.WriteLine(result);
}

static void Puzzle_5_2()
{
    var previousSeatId = int.MaxValue;
    foreach (var seat in File.ReadAllLines(@"input-05")
        .Select(Puzzle_5_ParseLine)
        .OrderBy(x => x.SeatId))
    {
        if (seat.SeatId - previousSeatId > 1)
        {
            break;
        }
        previousSeatId = seat.SeatId;
    }

    Console.WriteLine(previousSeatId + 1);
}

static (int Row, int Column, int SeatId) Puzzle_5_ParseLine(string line)
{
    var row = Convert.ToInt32(line.Substring(0, 7).Replace("F", "0").Replace("B", "1"), 2);
    var column = Convert.ToInt32(line.Substring(7).Replace("L", "0").Replace("R", "1"), 2);
    var seatId = row * 8 + column;
    return (row, column, seatId);
}

public static class Ext
{
    public static IEnumerable<T[]> ToCombinationsOf<T>(this IEnumerable<T> items, int count)
    {
        return CreateCombinations(items.ToArray(), count);
    }
    private static IEnumerable<T[]> CreateCombinations<T>(T[] items, int count, int offset = 0)
    {
        if (count == 1)
        {
            for (var i = offset; i < items.Length - 1; i++)
            {
                yield return new T[] { items[i] };
            }
        }
        else
        {
            for (var i = offset; i < items.Length - count; i++)
            {
                foreach (var c in CreateCombinations(items, count - 1, i + 1))
                {
                    var result = new T[count];
                    result[0] = items[i];
                    Array.Copy(c, 0, result, 1, c.Length);
                    yield return result;
                }
            }
        }
    }
}