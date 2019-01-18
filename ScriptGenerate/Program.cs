using System.Security.Cryptography;

namespace ScriptGenerate
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            execute();
        }

        public static void execute()
        {
            Console.WriteLine("Write size limit:");
            var limit = int.Parse(Console.ReadLine());

            var rnd = new Random();
            var sizevars = new[] {1, 5, 10};
            var sizes = new List<int>();
            var anf = StartScript();
            var apf = StartScript();
            var af = StartScript();

            for (var i = 0; i < limit;)
            {
                var index = rnd.Next(3);
                var size = sizevars[index];
                i += size;

                if (i >= limit)
                {
                    for (var j = i - size; j < limit; j++)
                    {
                        sizes.Add(sizevars[0]);
                    }

                    break;
                }

                sizes.Add(size);
            }

            var writeCount = 0;
            var readCount = 0;

            for (var i = 0; i < sizes.Count;)
            {
                var writeRnd = rnd.Next(1, 10);
                var readRnd = rnd.Next(1, writeCount - readCount + writeRnd);

                i += writeRnd;

                for (var j = writeCount; j < (i >= sizes.Count ? sizes.Count : writeCount + writeRnd); j++)
                {
                    anf.AppendLine($@"execute ../hdd/{sizes[j]}gb.db ../anf/{j}_{sizes[j]}gb.db ../result/anfW{sizes[j]}gb.txt ../result/anfWTotal.txt {sizes[j]}gb.db");
                    apf.AppendLine($@"execute ../hdd/{sizes[j]}gb.db ../apf/{j}_{sizes[j]}gb.db ../result/apfW{sizes[j]}gb.txt ../result/apfWTotal.txt {sizes[j]}gb.db");
                    af.AppendLine($@"execute ../hdd/{sizes[j]}gb.db ../af/{j}_{sizes[j]}gb.db ../result/afW{sizes[j]}gb.txt ../result/afWTotal.txt {sizes[j]}gb.db");
                }

                for (var j = readCount; j < (i >= sizes.Count ? sizes.Count : readCount + readRnd); j++)
                {
                    anf.AppendLine($@"execute ../anf/{j}_{sizes[j]}gb.db ../hdd/anf/{j}_{sizes[j]}gb.db ../result/anfR{sizes[j]}gb.txt ../result/anfRTotal.txt {sizes[j]}gb.db");
                    apf.AppendLine($@"execute ../apf/{j}_{sizes[j]}gb.db ../hdd/apf/{j}_{sizes[j]}gb.db ../result/apfR{sizes[j]}gb.txt ../result/apfRTotal.txt {sizes[j]}gb.db");
                    af.AppendLine($@"execute ../af/{j}_{sizes[j]}gb.db ../hdd/af/{j}_{sizes[j]}gb.db ../result/afR{sizes[j]}gb.txt ../result/afRTotal.txt {sizes[j]}gb.db");
                }

                if (i >= sizes.Count)
                {
                    break;
                }

                writeCount += writeRnd;
                readCount += readRnd;
            }

            using (var write = new StreamWriter(@"C:\scripts\anf.txt"))
            {
                write.WriteLine(anf.ToString());
            }

            using (var write = new StreamWriter(@"C:\scripts\apf.txt"))
            {
                write.WriteLine(apf.ToString());
            }

            using (var write = new StreamWriter(@"C:\scripts\af.txt"))
            {
                write.WriteLine(af.ToString());
            }
        }

        private static StringBuilder StartScript()
        {
            var start = @"execute () {" +
                        "\n\t from=$1\n" +
                        "\t to=$2\n" +
                        "\t result=$3\n" +
                        "\t resultTotal=$4\n" +
                        "\t size=$5\n" +
                        "\t currentDate=$(date +\"%T\")\n" +
                        "\t ts=$(date +%s%N)\n" +
                        "\t cp $from $to\n" +
                        "\t tt=$((($(date +%s%N) - $ts)/ 1000000))\n" +
                        "\t echo \"$currentDate $tt\" >> $result\n" +
                        "\t echo \"$currentDate $tt $size\" >> $resultTotal\n" +
                        "}\n\n\n";

            return new StringBuilder(start);
        }
    }
}
