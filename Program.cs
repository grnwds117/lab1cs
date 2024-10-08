using System;
using System.Collections.Generic;
using System.IO;


namespace GeneticsProject
{
    public struct GeneticData
    {
        public string name;
        public string organism;
        public string formula;
    }

    class Program
    {
        static List<GeneticData> data = new List<GeneticData>();
        static string GetFormula(string proteinName)
        {
            foreach (GeneticData item in data)
            {
                if (item.name.Equals(proteinName)) return item.formula;
            }
            return null;
        }

        static void ReadGeneticData(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] fragments = line.Split('\t');
                GeneticData protein;
                protein.name = fragments[0];
                protein.organism = fragments[1];
                protein.formula = fragments[2];
                data.Add(protein);
            }
            reader.Close();
        }

        static void ReadHandleCommands(string filename, string outputFile)
        {
            StreamReader reader = new StreamReader(filename);
            StreamWriter writer = new StreamWriter(outputFile);
            int counter = 1;

            writer.WriteLine("Alexey Zakharov");
            writer.WriteLine("Genetic searching");

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] commands = line.Split('\t');

                writer.WriteLine("--------------------------------------------------------------------------");

                switch (commands[0])
                {
                    case "search":
                        writer.WriteLine($"{counter.ToString("D3")}   search   {Decoding(commands[1])}");
                        int index = Search(commands[1]);
                        writer.WriteLine("organism                protein");
                        if (index != -1)
                            writer.WriteLine($"{data[index].organism}    {data[index].name}");
                        else
                            writer.WriteLine("NOT FOUND");
                        break;

                    case "diff":
                        writer.WriteLine($"{counter.ToString("D3")}   diff   {commands[1]}   {commands[2]}");
                        int differenceCount = Diff(commands[1], commands[2]);
                        writer.WriteLine($"amino-acids difference:\n{differenceCount}");
                        break;

                    case "mode":
                        writer.WriteLine($"{counter.ToString("D3")}   mode   {commands[1]}");
                        string modeResult = Mode(commands[1]);
                        writer.WriteLine($"amino-acid occurs:\n{modeResult}");
                        break;

                    default:
                        writer.WriteLine($"{counter.ToString("D3")}   Unknown command");
                        break;
                }

                counter++;
            }

            writer.Close();
            reader.Close();
        }


        static string Decoding(string formula)
        {
            string decoded = String.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]))
                {
                    int count = formula[i] - '0';
                    char letter = formula[i + 1];
                    decoded += new string(letter, count);
                    i++;
                }
                else
                {
                    decoded += formula[i];
                }
            }
            return decoded;
        }

        static int Search(string amino_acid)
        {
            string decoded = Decoding(amino_acid);
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded)) return i;
            }
            return -1;
        }

        static int Diff(string protein1, string protein2)
        {
            string formula1 = Decoding(GetFormula(protein1));
            string formula2 = Decoding(GetFormula(protein2));

            if (formula1 == null || formula2 == null) return -1;

            int differenceCount = 0;
            int minLength = Math.Min(formula1.Length, formula2.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (formula1[i] != formula2[i])
                {
                    differenceCount++;
                }
            }
            differenceCount += Math.Abs(formula1.Length - formula2.Length);

            return differenceCount;
        }

        static string Mode(string proteinName)
        {
            string formula = GetFormula(proteinName);
            Dictionary<char, int> aminoacidFrequency = new Dictionary<char, int>();
            foreach (char ch in formula)
            {
                if (aminoacidFrequency.ContainsKey(ch)) aminoacidFrequency[ch]++;
                else aminoacidFrequency[ch] = 1;
            } 

            int maxCount = 0;
            char mostCommonAminoacid = ' ';
            foreach (var item in aminoacidFrequency)
            {
                if (item.Value > maxCount || (item.Value == maxCount && item.Key < mostCommonAminoacid))
                {
                    maxCount = item.Value;
                    mostCommonAminoacid = item.Key;
                }
            }

            return $"{mostCommonAminoacid}\t\t  {maxCount}";
        }
        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                ReadGeneticData($"sequences.{i}.txt");
                ReadHandleCommands($"commands.{i}.txt", $"genedata.{i}.txt");
            }
        }
    }
}
