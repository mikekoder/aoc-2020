using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

var sw = Stopwatch.StartNew();
/*
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
Puzzle_6_1();
Puzzle_6_2();
Puzzle_7_1();
Puzzle_7_2();
Puzzle_8_1();
Puzzle_8_2();
Puzzle_9_1();
Puzzle_9_2();
*/

Puzzle_10_1();
Puzzle_10_2();

sw.Stop();

Console.WriteLine(sw.Elapsed.TotalMilliseconds + " ms");

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

static void Puzzle_6_1()
{
    var result = File.ReadAllText(@"input-06")
        .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
        .Select(g => g.Where(char.IsLetter).Distinct().Count())
        .Aggregate((a, x) => a + x);

    Console.WriteLine(result);
}

static void Puzzle_6_2()
{
    var sum = File.ReadAllText(@"input-06")
        .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
        .Select(g => g.Split('\n')
            .Select(p => p.Where(char.IsLetter))
            .Aggregate((a,x) => a.Intersect(x))
            .Count())
        .Aggregate((a, x) => a + x);

    Console.WriteLine(sum); 
}

static void Puzzle_7_1()
{
    var rules = File.ReadAllLines(@"input-07")
        .Select(Puzzle_7_ParseRule)
        .ToArray();

    var results = new List<string>();
    var queue = new Queue<(string Color, (int Count, string Color)[] Contains)>(
        rules.Where(r => r.Contains.Any(c => c.Color == "shiny gold")));
    while (queue.Count > 0)
    {
        var rule = queue.Dequeue();
        if (results.Contains(rule.Color))
        {
            continue;
        }
        results.Add(rule.Color);
        foreach(var sub in rules.Where(r => r.Contains.Any(c => c.Color == rule.Color)))
        {
            queue.Enqueue(sub);
        }
    }

    Console.WriteLine(results.Count);
}

static void Puzzle_7_2()
{
    var rules = File.ReadAllLines(@"input-07")
        .Select(Puzzle_7_ParseRule)
        .ToArray();

    int subCount(string color)
    {
        var rule = rules.Single(r => r.Color == color);
        return rule.Contains.Sum(c => c.Count + c.Count * subCount(c.Color));
    }

    var count = subCount("shiny gold");
    Console.WriteLine(count);
}

static void Puzzle_8_1()
{
    var instructions = File.ReadAllLines(@"input-08")
        .Select(l => l.Trim().Split(' '))
        .ToArray();
    var line = 0;
    var acc = 0;
    var executedLines = new BitArray(instructions.Length);
    while (true)
    {
        if (executedLines[line])
        {
            break;
        }
        executedLines.Set(line, true);
        
        var instruction = instructions[line];
        var arg = int.Parse(instruction[1]);
        switch (instruction[0])
        {
            case "nop":
                line++;
                break;
            case "acc":
                acc += arg;
                line++;
                break;
            case "jmp":
                line += arg;
                break;
        }
    }

    Console.WriteLine(acc);
}

static (string Color, (int Count, string Color)[] Contains) Puzzle_7_ParseRule(string line)
{
    var parts = line.Split("bags contain", 2);
    var color = parts[0].Trim();
    if (parts[1].Any(char.IsNumber))
    {
        var contents = parts[1].Split(',').Select(x =>
        {
            var match = Regex.Match(x, @"(\d+)\s+(.*)\s+bag");
            return (int.Parse(match.Groups[1].Value), match.Groups[2].Value);
        }).ToArray();

        return (color, contents);
    }
    else
    {
        return (color, new (int, string)[0]);
    }
}

static void Puzzle_8_2()
{
    var instructions = File.ReadAllLines(@"input-08")
       .Select(l => l.Trim().Split(' '))
       .ToArray();

    var acc = 0;
    var lineToChange = 0;
    while (true)
    {
        var line = 0;
        acc = 0;
        var executedLines = new BitArray(instructions.Length);
        while (line < instructions.Length)
        {
            if (executedLines[line])
            {
                break;
            }
            executedLines.Set(line, true);

            var instruction = instructions[line];
            var cmd = instruction[0];
            if (line == lineToChange)
            {
                cmd = instruction[0] switch
                {
                    "nop" => "jmp",
                    "jmp" => "nop",
                    string s => s
                };
            }

            var arg = int.Parse(instruction[1]);
            int nextLine;
            switch (cmd)
            {
                case "nop":
                    nextLine = line + 1;
                    break;
                case "acc":
                    acc += arg;
                    nextLine = line + 1;
                    break;
                case "jmp":
                    nextLine = line + arg;
                    break;
                default:
                    throw new Exception();
            }

            if(line == instructions.Length - 1)
            {
                break;
            }
            else
            {
                line = nextLine;
            }
        }

        if (line == instructions.Length - 1)
        {
            break;
        }
        lineToChange++;
    }

    Console.WriteLine(acc);
}

static void Puzzle_9_1()
{
    var numbers = File.ReadAllLines(@"input-09")
        .Select(l => long.Parse(l))
        .ToArray();

    var lineNumber = 25;
    while (true)
    {
        var pairs = numbers
            .Skip(lineNumber - 25)
            .Take(25)
            .ToCombinationsOf(2);
        if(!pairs.Any(p => p[0] + p[1] == numbers[lineNumber]))
        {
            break;
        }
        lineNumber++;
    }
    Console.WriteLine(numbers[lineNumber]);
}

static void Puzzle_9_2()
{
    var invalidLine = 590;
    var numbers = File.ReadAllLines(@"input-09")
        .Select(l => long.Parse(l))
        .ToArray();

    var invalidNumber = numbers[invalidLine];
    var startIndex = 0;
    long min = 0, max = 0;
    while (max == 0)
    {
        var count = 3;
        while (true)
        {
            var selection = numbers.Skip(startIndex).Take(count);
            var sum = selection.Sum();
            if (sum == invalidNumber)
            {
                min = selection.Min();
                max = selection.Max();
            }
            if (sum >= invalidNumber)
            {
                break;
            }
            count++;
        }
        startIndex++;
    }
    Console.WriteLine(min + max);
}

static void Puzzle_10_1()
{
    var adapters = File.ReadAllLines(@"input-10")
        .Select(int.Parse)
        .OrderBy(x => x)
        .ToArray();

    var differences = adapters.Skip(1).Select((a, i) => a - adapters[i]);
    var diff1 = differences.Count(d => d == 1) + 1; // add the 0 -> 1
    var diff3 = differences.Count(d => d == 3) + 1; // last adapter -> device

    Console.WriteLine(diff1 * diff3);
}

static void Puzzle_10_2()
{
    var counters = File.ReadAllLines(@"input-10")
        .Select(int.Parse)
        .ToDictionary(x => x, x => 0L);

    long calculateWaysToGetTarget(int target)
    {
        var count = 0L;
        for(var previous = target - 1; previous >= target - 3; previous--)
        {
            if(previous == 0)
            {
                count++;
            }
            else if (counters.ContainsKey(previous))
            {
                if(counters[previous] == 0)
                {
                    counters[previous] = calculateWaysToGetTarget(previous);
                }
                count += counters[previous];
            }
        }
        return count;
    }
    var target = counters.Keys.Max() + 3;
    var count = calculateWaysToGetTarget(target);
    Console.WriteLine(count);
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
            for (var x = offset; x <= items.Length - 1; x++)
            {
                yield return new T[] { items[x] };
            }
        }
        else
        {
            for (var i = offset; i <= items.Length - count; i++)
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