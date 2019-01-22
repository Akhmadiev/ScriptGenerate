using System.Linq;

namespace ScriptGenerate
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public static class Exist
    {
        public static void Execute()
        {
            Console.WriteLine("Write size limit:");
            var limit = int.Parse(Console.ReadLine());

            var rnd = new Random();
            var sizevars = new[] { 50, 100, 200, 500, 1024, 2048 };
            var shortDate = DateTime.Now.ToString("MM.dd.yyyy");
            var sizes = new List<int>();
            var anf = StartScript();
            var apf = StartScript();
            var af = StartScript();

            for (var i = 0; i < limit;)
            {
                var index = rnd.Next(sizevars.Length);
                var size = sizevars[index];
                i += size;

                if (i >= limit)
                {
                    for (var j = i - size; j < limit; j += sizevars[0])
                    {
                        sizes.Add(sizevars[0]);
                    }

                    break;
                }

                sizes.Add(size);
            }

            var sum = sizes.Sum();

            var writeCount = 0;
            var readCount = 0;

            for (var i = 0; i < sizes.Count;)
            {
                var writeRnd = rnd.Next(1, 10);
                var readRnd = rnd.Next(1, writeCount - readCount + writeRnd);

                i += writeRnd;

                for (var j = writeCount; j < (i >= sizes.Count ? sizes.Count : writeCount + writeRnd); j++)
                {
                    anf.AppendLine($@"execute ../hdd/{sizes[j]}mb.db ../anf/add.db ../result/{shortDate}_anfW{sizes[j]}mb.txt ../result/{shortDate}_anfWTotal.txt {sizes[j]}mb.db");
                    apf.AppendLine($@"execute ../hdd/{sizes[j]}mb.db ../apf/add.db ../result/{shortDate}_apfW{sizes[j]}mb.txt ../result/{shortDate}_apfWTotal.txt {sizes[j]}mb.db");
                    af.AppendLine($@"execute ../hdd/{sizes[j]}mb.db ../af/add.db ../result/{shortDate}_afW{sizes[j]}mb.txt ../result/{shortDate}_afWTotal.txt {sizes[j]}mb.db");
                }

                for (var j = readCount; j < (i >= sizes.Count ? sizes.Count : readCount + readRnd); j++)
                {
                    anf.AppendLine($@"execute ../anf/{sizes[j]}mb.db  ../hdd/anf/add.db ../result/{shortDate}_anfR{sizes[j]}mb.txt ../result/{shortDate}_anfRTotal.txt {sizes[j]}mb.db");
                    apf.AppendLine($@"execute ../apf/{sizes[j]}mb.db  ../hdd/apf/add.db ../result/{shortDate}_apfR{sizes[j]}mb.txt ../result/{shortDate}_apfRTotal.txt {sizes[j]}mb.db");
                    af.AppendLine($@"execute  ../af/{sizes[j]}mb.db  ../hdd/af/add.db ../result/{shortDate}_afR{sizes[j]}mb.txt ../result/{shortDate}_afRTotal.txt {sizes[j]}mb.db");
                }

                if (i >= sizes.Count)
                {
                    break;
                }

                writeCount += writeRnd;
                readCount += readRnd;
            }


            var targetPath = @"C:\scripts\existing";
            if (!Directory.Exists(targetPath))
            {
                if (!Directory.Exists(@"C:\scripts"))
                {
                    Directory.CreateDirectory(@"C:\scripts");
                }

                Directory.CreateDirectory(targetPath);
            }

            using (var write = new StreamWriter($@"{targetPath}\anf.txt"))
            {
                write.WriteLine(anf.ToString());
            }

            using (var write = new StreamWriter($@"{targetPath}\apf.txt"))
            {
                write.WriteLine(apf.ToString());
            }

            using (var write = new StreamWriter($@"{targetPath}\af.txt"))
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
                        "\t cat $from >> $to\n" +
                        "\t tt=$((($(date +%s%N) - $ts)/ 1000000))\n" +
                        "\t echo \"$currentDate $tt\" >> $result\n" +
                        "\t echo \"$currentDate $tt $size\" >> $resultTotal\n" +
                        "}\n\n\n";

            return new StringBuilder(start);
        }
    }
}
