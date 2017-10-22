using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ItvwMillenium
{
    class Program
    {
        public static void Main()
        {
           
        }

        public static void NetPositionsCalculator(string filePath)
        {
            var NetPositions = new SortedList();

            using (var sr = new StreamReader(filePath))
            {
                string headerLine = sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var position = new Position(line);
                    NetPosition(NetPositions, position);
                }
            }

            using (var sw = new StreamWriter("Result.csv"))
            {
                sw.WriteLine("TRADER,SYMBOL,QUANTITY");
                
                foreach(Position p in NetPositions.Values)
                    sw.WriteLine("{0},{1},{2}",p.Trader,p.Symbol,p.Quantity);
            }
        }

        public static void NetPosition(SortedList NetPositions, Position p)
        {
            if (NetPositions.Contains(p.Hash()))
            {
                var item = NetPositions[p.Hash()] as Position;
                item.Quantity += p.Quantity;
                return;
            }

            NetPositions.Add(p.Hash(), p);
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

        public Position(string line)
        {
            string[] fields = line.Split(',');
            Trader = fields[0];
            Broker = fields[1];
            Symbol = fields[2];
            Quantity = int.Parse(fields[3]);
            Price = int.Parse(fields[4]);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", Trader, Symbol, Quantity);
        }

        public string Hash()
        {
            return string.Format("{0}{1}", Trader, Symbol);
        }
    }

    [TestClass]
    public class UnitTest
    {

        [TestMethod]
        public void TestNetPositionsCalculator()
        {
            var input = @"test_data.csv";
            var output = @"result.csv";
            var expectedOutput = @"net_positions_expected.csv";
            
            Program.NetPositionsCalculator(input);
            
            using (var o = new StreamReader(output))
            using (var eo = new StreamReader(expectedOutput))
            {
                string outputLine;
                string expectedOutputLine;
                while ((outputLine = o.ReadLine()) != null)
                {
                    expectedOutputLine = eo.ReadLine();
                    Assert.AreEqual(expectedOutputLine, outputLine); 
                }       
            }
        }

        [TestMethod]
        public void TestNetPosition()
        {
            var NetPositions = new SortedList();

            var p1 = new Position("BuzzAldrin,DB,MOON.N,100,69");
            Program.NetPosition(NetPositions, p1);
            Assert.AreEqual(1, NetPositions.Count);
            BrowseAndTestList(NetPositions, "BuzzAldrin", "MOON.N", 100);

            var p2 = new Position("BuzzAldrin,DB,MOON.N,100,69");
            Program.NetPosition(NetPositions, p2);
            Assert.AreEqual(1, NetPositions.Count);
            BrowseAndTestList(NetPositions, "BuzzAldrin", "MOON.N", 200);


            var p3 = new Position("Steevy,DB,AAPL.N,666,85");
            Program.NetPosition(NetPositions, p3);
            Assert.AreEqual(2, NetPositions.Count);
            BrowseAndTestList(NetPositions, "Steevy", "AAPL.N", 666);

        }

        void BrowseAndTestList(SortedList NetPositions, string TargetTrader, string TargetSymbol, int TargetQuantity)
        {
            var hash = string.Format("{0}{1}", TargetTrader, TargetSymbol);
            var p = NetPositions[hash] as Position;
            Assert.AreEqual(TargetQuantity, p.Quantity);
        }
        
    }
}
