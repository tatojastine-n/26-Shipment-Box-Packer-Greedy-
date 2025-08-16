using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PackingStrategy { FirstFit, BestFit }

public class Box
{
    private readonly List<decimal> _items = new List<decimal>();
    public decimal MaxCapacity { get; }
    public decimal CurrentWeight => _items.Sum();
    public decimal RemainingCapacity => MaxCapacity - CurrentWeight;
    public decimal FillRatio => MaxCapacity > 0 ? CurrentWeight / MaxCapacity : 0;

    public Box(decimal maxCapacity)
    {
        MaxCapacity = maxCapacity;
    }

    public bool TryAdd(decimal weight)
    {
        if (weight <= 0)
            throw new ArgumentException("Item weight must be positive");

        if (CurrentWeight + weight > MaxCapacity)
            return false;

        _items.Add(weight);
        return true;
    }

    public override string ToString()
    {
        return $"[{string.Join(", ", _items)}] (Used: {CurrentWeight}/{MaxCapacity}, {FillRatio:P0})";
    }
}

public class Packer
{
    private readonly List<Box> _boxes = new List<Box>();
    private readonly decimal _boxCapacity;
    private readonly PackingStrategy _strategy;

    public Packer(decimal boxCapacity, PackingStrategy strategy)
    {
        _boxCapacity = boxCapacity;
        _strategy = strategy;
    }

    public void PackItems(IEnumerable<decimal> itemWeights)
    {
        foreach (var weight in itemWeights)
        {
            switch (_strategy)
            {
                case PackingStrategy.FirstFit:
                    FirstFitAdd(weight);
                    break;
                case PackingStrategy.BestFit:
                    BestFitAdd(weight);
                    break;
                default:
                    throw new ArgumentException("Unknown packing strategy");
            }
        }
    }

    private void FirstFitAdd(decimal weight)
    {
        foreach (var box in _boxes)
        {
            if (box.TryAdd(weight))
                return;
        }

        var newBox = new Box(_boxCapacity);
        newBox.TryAdd(weight);
        _boxes.Add(newBox);
    }

    private void BestFitAdd(decimal weight)
    {
        var bestBox = _boxes
            .Where(b => b.RemainingCapacity >= weight)
            .OrderBy(b => b.RemainingCapacity)
            .FirstOrDefault();

        if (bestBox != null)
        {
            bestBox.TryAdd(weight);
        }
        else
        {
            var newBox = new Box(_boxCapacity);
            newBox.TryAdd(weight);
            _boxes.Add(newBox);
        }
    }

    public void PrintResults()
    {
        Console.WriteLine($"\nPacking Results ({_strategy} Strategy, Capacity: {_boxCapacity})");
        Console.WriteLine(new string('=', 50));

        for (int i = 0; i < _boxes.Count; i++)
        {
            Console.WriteLine($"Box {i + 1}: {_boxes[i]}");
        }

        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"Total Boxes Used: {_boxes.Count}");
        Console.WriteLine($"Total Weight: {_boxes.Sum(b => b.CurrentWeight)}");
        Console.WriteLine($"Average Fill Ratio: {_boxes.Average(b => b.FillRatio):P0}");
    }
}

namespace Shipment_Box_Packer__Greedy_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter box capacity:");
            decimal capacity = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Choose strategy (1: First Fit, 2: Best Fit):");
            var strategy = Console.ReadLine() == "1"
                ? PackingStrategy.FirstFit
                : PackingStrategy.BestFit;

            Console.WriteLine("Enter item weights (comma-separated):");
            var items = Console.ReadLine()
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(decimal.Parse)
                .ToList();

            var packer = new Packer(capacity, strategy);
            packer.PackItems(items);
            packer.PrintResults();
        }
    }
}
