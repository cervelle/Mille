using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ItvwMilleniumx
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = @"test_data.csv";
            var Positions = new List<Position>();
            var NetPositions = new HashSet<Position>();

            using (var sr = new StreamReader(filePath))
            {
                string headerLine = sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {

                    string[] fields = line.Split(',');
                    Positions.Add(new Position(fields));
                    NetPosition(NetPositions, fields);

                }
            }
        }

        public static void NetPosition(HashSet<Position> NetPositions, string[] fields)
        {
            foreach (Position p in NetPositions)
            {
                if ((fields[0] == p.Trader) && (fields[2] == p.Symbol))
                {
                    p.Quantity += int.Parse(fields[3]);
                    return;
                }
            }
            NetPositions.Add(new Position()
            {
                Trader = fields[0],
                Symbol = fields[2],
                Quantity = int.Parse(fields[3])
            });
        }
    }

    public class Position
    {
        public string Trader;
        public string Broker;
        public string Symbol;
        public int Quantity;
        public int Price;

        public Position() {}

        //public Position(string line)
        //{
        //    string[] fields = line.Split(',');
        //    Trader = fields[0];
        //    Broker = fields[1];
        //    Symbol = fields[2];
        //    Quantity = int.Parse(fields[3]);
        //    Price = int.Parse(fields[4]);
        //}

        public Position(string[] fields)
        {
            Trader = fields[0];
            Broker = fields[1];
            Symbol = fields[2];
            Quantity = int.Parse(fields[3]);
            Price = int.Parse(fields[4]);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Position;
            if (other == null)
            {
                return false;
            }
            return Trader == other.Trader && Symbol == other.Symbol;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestNetPosition()
        {
            
            var NetPositions = new HashSet<Position>();

            var value1 = new string[] { "BuzzAldrin", "DB", "MOON.N", "100", "69" };
            Program.NetPosition(NetPositions, value1);
            Assert.AreEqual(1, NetPositions.Count());
            BrowseAndTestList(NetPositions, "BuzzAldrin", "MOON.N", 100);

            var value2 = new string[] { "BuzzAldrin", "DB", "MOON.N", "100", "69" };
            Program.NetPosition(NetPositions, value2);
            Assert.AreEqual(1, NetPositions.Count());
            BrowseAndTestList(NetPositions, "BuzzAldrin", "MOON.N", 200);

            
            var value3 = new string[] { "Steevy", "DB", "AAPL.N", "666", "85" };
            Program.NetPosition(NetPositions, value3);
            Assert.AreEqual(2, NetPositions.Count());
            BrowseAndTestList(NetPositions, "Steevy", "AAPL.N", 666);

        }

        void BrowseAndTestList(HashSet<Position> NetPositions, string TargetTrader, string TargetSymbol, int TargetQuantity)
        {
            foreach (var p in NetPositions)
                if (p.Trader == TargetTrader && p.Symbol == TargetSymbol)
                    Assert.AreEqual(TargetQuantity, p.Quantity);
        }
        
    }
}
