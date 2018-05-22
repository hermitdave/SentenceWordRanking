using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SentenceWorkRanking
{
    class Program
    {
        private static char[] separatorArray = new char[] { ' ', ',', '\t' };
        private static Dictionary<string, long> wordPositionDictionary = new Dictionary<string, long>();

        static void Main(string[] args)
        {
            string frequencyWordsPath = null;
            string sentenceDataPath = null;
            string sentenceWordRankingDataPath = null;

            foreach (var arg in args)
            {
                if (arg.StartsWith("/freq:"))
                {
                    frequencyWordsPath = arg.Replace("/freq:", string.Empty);
                }
                else if (arg.StartsWith("/input:"))
                {
                    sentenceDataPath = arg.Replace("/input:", string.Empty);
                }
                else if (arg.StartsWith("/output:"))
                {
                    sentenceWordRankingDataPath = arg.Replace("/output:", string.Empty);
                }
            }

            if (string.IsNullOrEmpty(frequencyWordsPath) || string.IsNullOrEmpty(sentenceDataPath) || string.IsNullOrEmpty(sentenceWordRankingDataPath))
            {
                Console.WriteLine("The SentenceWordRank needs to be run as follows:");
                Console.WriteLine("SentenceWordRank /freq:frequencyWords.txt /input:sentenceData.txt /output:sentenceRanking.txt");

                return;
            }

            if (!File.Exists(frequencyWordsPath))
            {
                Console.WriteLine("Please verify the path for Frequency Words");

                return;
            }

            if (!File.Exists(sentenceDataPath))
            {
                Console.WriteLine("Please verify the path for input Sentence Data");

                return;
            }

            Console.WriteLine("Load frequency words");
            LoadFrequencyWords(frequencyWordsPath);


            Console.WriteLine("Load input sentence data");
            ProcessSentenceData(sentenceDataPath, sentenceWordRankingDataPath);

            Console.WriteLine("Sentence word ranking data processed");
        }

        private static void ProcessSentenceData(string sentenceDataPath, string sentenceWordRankingDataPath)
        {
            using (FileStream outputStream = File.OpenWrite(sentenceWordRankingDataPath))
            {
                using (StreamWriter writer = new StreamWriter(outputStream))
                {
                    using (FileStream inputStream = File.OpenRead(sentenceDataPath))
                    {
                        using (StreamReader reader = new StreamReader(inputStream))
                        {
                            List<string> positionList = new List<string>();

                            while(!reader.EndOfStream)
                            {
                                var sentence = reader.ReadLine().ToLowerInvariant();
                                var words = sentence.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);

                                positionList.Clear();

                                long pos = 0;

                                foreach(var word in words)
                                {
                                    pos = 0;

                                    wordPositionDictionary.TryGetValue(word, out pos);

                                    positionList.Add(pos.ToString());
                                }

                                var positionData = string.Join(' ', positionList);

                                writer.WriteLine($"{sentence}\t{positionData}");
                            }
                        }
                    }

                    writer.Flush();
                }
            }
        }

        private static void LoadFrequencyWords(string frequencyWordsPath)
        {
            using (FileStream fs = File.OpenRead(frequencyWordsPath))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    int i = 1;

                    while(!sr.EndOfStream)
                    {
                        string frequencyWordData = sr.ReadLine();

                        string[] parts = frequencyWordData.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length > 0)
                        {
                            wordPositionDictionary.Add(parts[0], i);
                            i++;
                        }
                    }
                }
            }
        }
    }
}
