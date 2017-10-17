using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ItvwMillenium
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = @"test_data.csv";
            var Positions = new List<Position>();
            var Net_Positions = new List<Position>();

            using (var sr = new StreamReader(filePath))
            {
                string headerLine = sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Positions.Add(new Position(line));  
                }
            }

            
        }
    }

    class Position
    {
        string Trader;
        string Broker;
        string Symbol;
        int Quantity;
        int Price;

        public Position(string line)
        {
            string[] fields = line.Split(',');
            Trader = fields[0];
            Broker = fields[1];
            Symbol = fields[2];
            Quantity = int.Parse(fields[3]);
            Price = int.Parse(fields[4]);
        }
    }
}
