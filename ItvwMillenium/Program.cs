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

        public static void PositionsCalculator(string input, string type)
        {
            var Positions = new SortedList();

            //Read input
            using (var sr = new StreamReader(input))
            {
                string headerLine = sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var p = new Position(line);
                    //Apply logic depending of the type
                    Calculators(type, Positions, p);
                }
            }

            //Output Result
            using (var sw = new StreamWriter("Result.csv"))
            {

                sw.WriteLine("TRADER,SYMBOL,QUANTITY");
                
                foreach(Position p in Positions.Values)
                    if (p.Net || p.Boxed)
                        sw.WriteLine("{0},{1},{2}",p.Trader,p.Symbol,p.Quantity);
            }
        }

        public static void Calculators(string type, SortedList Positions, Position p)
        {
            switch (type)
            {
                case "NetPosition":
                    NetPositionsCalculator(Positions, p);
                    break;
                case "BoxedPosition":
                    BoxedPositionsCalculator(Positions, p);
                    break;
                default:
                    throw new System.ArgumentException("Parameter cannot be null", type);
            }
        }

        public static void NetPositionsCalculator(SortedList NetPositions, Position p)
        {
            p.Net = true;
            if (NetPositions.Contains(p.Hash()))
            {
                var item = NetPositions[p.Hash()] as Position;
                item.Quantity += p.Quantity;
                return;
            }

            NetPositions.Add(p.Hash(), p);
        }

        public static void BoxedPositionsCalculator(SortedList BoxedPositions, Position p)
        {
            if (BoxedPositions.Contains(p.Hash()))
            {
                var item = BoxedPositions[p.Hash()] as Position;
                if (!item.Boxed)
                {
                    if ((item.Quantity>0 && p.Quantity<0) || (item.Quantity<0 && p.Quantity>0))
                    {
                        item.Boxed = true;
                    } 
                }
                item.Quantity += p.Quantity;
                return;
            }

            BoxedPositions.Add(p.Hash(), p);
            
        }
    }

    public class Position
    {
        public string Trader;
        public string Broker;
        public string Symbol;
        public int Quantity;
        public int Price;
        public bool Boxed;
        public bool Net;

        public Position() {}

        public Position(string line)
        {
            string[] fields = line.Split(',');
            Trader = fields[0];
            Broker = fields[1];
            Symbol = fields[2];
            Quantity = int.Parse(fields[3]);
            Price = int.Parse(fields[4]);
            Net = false;
            Boxed = false;
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
        public void TestCalculators()
        {
            TestCalculator("NetPosition", "net_positions_expected.csv");
            TestCalculator("BoxedPosition", "boxed_positions_expected.csv");
        }


        public void TestCalculator(string type, string expectedOutput)
        {
            var input = @"test_data.csv";
            var output = @"result.csv";

            Program.PositionsCalculator(input, type);
            
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
        public void TestNetPositionsCalculator()
        {
            var NetPositions = new SortedList();

            var p1 = new Position("BuzzAldrin,DB,MOON.N,100,69");
            Program.NetPositionsCalculator(NetPositions, p1);
            Assert.AreEqual(1, NetPositions.Count);
            BrowseAndTestList(NetPositions, "BuzzAldrin", "MOON.N", 100);

            var p2 = new Position("BuzzAldrin,DB,MOON.N,100,69");
            Program.NetPositionsCalculator(NetPositions, p2);
            Assert.AreEqual(1, NetPositions.Count);
            BrowseAndTestList(NetPositions, "BuzzAldrin", "MOON.N", 200);


            var p3 = new Position("Steevy,DB,AAPL.N,666,85");
            Program.NetPositionsCalculator(NetPositions, p3);
            Assert.AreEqual(2, NetPositions.Count);
            BrowseAndTestList(NetPositions, "Steevy", "AAPL.N", 666);

        }

        [TestMethod]
        public void TestBoxedPositionsCalculator()
        {
            var BoxedPositions = new SortedList();

            var p1 = new Position("AmazingRandy,DB,ESC.N,100,69");
            Program.BoxedPositionsCalculator(BoxedPositions, p1);
            Assert.AreEqual(1, BoxedPositions.Count);
            BrowseAndTestList(BoxedPositions, "AmazingRandy", "ESC.N", 100);
            TestBoxedPosition(BoxedPositions, "AmazingRandy", "ESC.N", false);

            var p2 = new Position("AmazingRandy,DB,ESC.N,-50,69");
            Program.BoxedPositionsCalculator(BoxedPositions, p2);
            Assert.AreEqual(1, BoxedPositions.Count);
            BrowseAndTestList(BoxedPositions, "AmazingRandy", "ESC.N", 50);
            TestBoxedPosition(BoxedPositions, "AmazingRandy", "ESC.N", true);


            var p3 = new Position("AmazingRandy,DB,HOUD.N,-300,98");
            Program.BoxedPositionsCalculator(BoxedPositions, p3);
            Assert.AreEqual(2, BoxedPositions.Count);
            BrowseAndTestList(BoxedPositions, "AmazingRandy", "HOUD.N", -300);
            TestBoxedPosition(BoxedPositions, "AmazingRandy", "HOUD.N", false);

            var p4 = new Position("AmazingRandy,DB,HOUD.N,-300,98");
            Program.BoxedPositionsCalculator(BoxedPositions, p4);
            Assert.AreEqual(2, BoxedPositions.Count);
            BrowseAndTestList(BoxedPositions, "AmazingRandy", "HOUD.N", -600);
            TestBoxedPosition(BoxedPositions, "AmazingRandy", "HOUD.N", false);

            var p5 = new Position("AmazingRandy,DB,HOUD.N,800,98");
            Program.BoxedPositionsCalculator(BoxedPositions, p5);
            Assert.AreEqual(2, BoxedPositions.Count);
            BrowseAndTestList(BoxedPositions, "AmazingRandy", "HOUD.N", 200);
            TestBoxedPosition(BoxedPositions, "AmazingRandy", "HOUD.N", true);

        }

        void BrowseAndTestList(SortedList positions, string targetTrader, string targetSymbol, int targetQuantity)
        {
            var hash = string.Format("{0}{1}", targetTrader, targetSymbol);
            var p = positions[hash] as Position;
            Assert.AreEqual(targetQuantity, p.Quantity);
        }

        void TestBoxedPosition(SortedList positions, string targetTrader, string targetSymbol, bool targetBoxed)
        {
            var hash = string.Format("{0}{1}", targetTrader, targetSymbol);
            var p = positions[hash] as Position;
            Assert.AreEqual(targetBoxed, p.Boxed);
        }
        
    }
}
