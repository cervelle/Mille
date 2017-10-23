using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareTest
{
    /**
     * Welcome to the Software Test. Please make sure you
     * read the instructions carefully.
     *
     * FAQ:
     * Can I use linq? Yes.
     * Can I cheat and look things up on Stack Overflow? Yes.
     * Can I use a database? No.
     */

    /// There are two challenges in this file
    /// The first one should takes ~10 mins with the
    /// second taking between ~30-40 mins.
    public interface IChallenge
    {
        /// Are you a winner?
        bool Winner();
    }

    /// Lets find out
    public class Program
    {
        /// <summary>
        /// Challenge Uno - NumberCalculator
        ///
        /// Fill out the TODOs with your own code and make any
        /// other appropriate improvements to this class.
        /// </summary>
        public class NumberCalculator : IChallenge
        {
            public int FindMax(int[] numbers)
            {
                return numbers.Max();
            }

            public int[] FindMax(int[] numbers, int n)
            {
                //Find the 'n' highest numbers
                return numbers.OrderByDescending(x => x).Take(n).ToArray();
            }

            public int[] Sort(int[] numbers)
            {
                return numbers.OrderBy(x => x).ToArray();
            }

            public bool Winner()
            {
                var numbers = new[] { 5, 7, 5, 3, 6, 7, 9 };
                var sorted = Sort(numbers);
                var maxes = FindMax(numbers, 2);

                // Are the following test cases sufficient, to prove your code works
                // as expected? If not either write more test cases and/or describe what
                // other tests cases would be needed.
                // *****(see below)*****

                return sorted.First() == 3
                       && sorted.Last() == FindMax(numbers) //challenge each other
                       && sorted.Length == numbers.Length //belt and braces
                       && FindMax(numbers) == 9 //now useless
                       && maxes[0] == 9
                       && maxes[1] == 7
                       && maxes.Length == 2; //check the length of maxes (to be honest i have done the error :D)
            }
        }

        /// <summary>
        /// Challenge Due - Run Length Encoding
        ///
        /// RLE is a simple compression scheme that encodes runs of data into
        /// a single data value and a count. It's useful for data that has lots
        /// of contiguous values (for example it was used in fax machines), but
        /// also has lots of downsides.
        ///
        /// For example, aaaaaaabbbbccccddddd would be encoded as
        ///
        /// 7a4b4c5d
        ///
        /// You can find out more about RLE here...
        /// http://en.wikipedia.org/wiki/Run-length_encoding
        ///
        /// In this exercise you will need to write an RLE **Encoder** which will take
        /// a byte array and return an RLE encoded byte array.
        /// </summary>
        public class RunLengthEncodingChallenge : IChallenge
        {
            public byte[] Encode(byte[] original)
            {
                var list = new Queue<byte>();
                var bufferChar = original[0];
                var counter = 0;
                foreach (var c in original)
                {
                    if (c == bufferChar)
                        ++counter;
                    else
                    {
                        list.Enqueue(Convert.ToByte(counter));
                        list.Enqueue(bufferChar);
                        bufferChar = c;
                        counter = 1;
                    }
                }
                list.Enqueue(Convert.ToByte(counter));
                list.Enqueue(bufferChar);

                var final = new byte[list.Count];
                for (int i = 0; i < final.Length; i++)
                {
                    final[i] = list.Dequeue();
                }

                return final;
            }

            public bool Winner()
            {
                // TODO: Are the following test cases sufficient, to prove your code works
                // as expected? If not either write more test cases and/or describe what
                // other tests cases would be needed.

                var testCases = new[]
                {
                    new Tuple<byte[], byte[]>(new byte[]{0x01, 0x02, 0x03, 0x04}, new byte[]{0x01, 0x01, 0x01, 0x02, 0x01, 0x03, 0x01, 0x04}),
                    new Tuple<byte[], byte[]>(new byte[]{0x01, 0x01, 0x01, 0x01}, new byte[]{0x04, 0x01}),
                    new Tuple<byte[], byte[]>(new byte[]{0x01, 0x01, 0x02, 0x02}, new byte[]{0x02, 0x01, 0x02, 0x02})
                    //several tests with sequence of character like a or aaaaaaabbbbbbbccccc or abbbbbbbbbbbbbbbbbbbbbc or aaaaaaaaaaabccccccccbddddddddd
                    //mutiple repeated character throughout the output, in the beginning/middle/end and isolated character in the middle of the input
                    //I guess another good test could be to add space or ponctuation in a test
                };

                // TODO: What limitations does your algorithm have (if any)?
                //My algorithm is limit by the size of the array (could be type "long")
                // TODO: What do you think about the efficiency of this algorithm for encoding data?
                //Depending of the datas, the algorithm is not very efficient. For instance encoding this sentence is not very efficient as the output will be larger than the input.

                foreach (var testCase in testCases)
                {
                    var encoded = Encode(testCase.Item1);
                    var isCorrect = encoded.SequenceEqual(testCase.Item2);

                    if (!isCorrect)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static void Main(string[] args)
        {
            var challenges = new IChallenge[]
            {
                new NumberCalculator(),
                new RunLengthEncodingChallenge()
            };

            foreach (var challenge in challenges)
            {
                var challengeName = challenge.GetType().Name;

                var result = challenge.Winner()
                    ? string.Format("You win at challenge {0}", challengeName)
                    : string.Format("You lose at challenge {0}", challengeName);

                Console.WriteLine(result);
            }

            Console.ReadLine();
        }
    }
}
