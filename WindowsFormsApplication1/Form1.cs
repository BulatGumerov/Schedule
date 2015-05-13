using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.DataVisualization;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int dimen;
        private Random rand = new Random();
        private int pictureIteration;

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Int32.TryParse(textBox1.Text, out dimen))
            {
                return;
            }
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            for (int i = 0; i < dimen; i++)
            {
                dataGridView1.Columns.Add("", (i+1).ToString());
            }

            for (var i = 0; i < 3; i++)
            {
                dataGridView1.Rows.Add();
                for (var j = 0; j < dimen; j++)
                {
                    dataGridView1[j, i].Value = "0";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }

            var r = new List<int>();
            var d = new List<int>();
            var p = new List<int>();

            for (var i = 0; i < dimen; i++)
            {
                r.Add(Convert.ToInt32(dataGridView1[i, 0].Value));
                d.Add(Convert.ToInt32(dataGridView1[i, 1].Value));
                p.Add(Convert.ToInt32(dataGridView1[i, 2].Value));
            }
            var input = new Input(r, d, p);

            fsP.Text = ListToString(Solver1.Solve(input));
            fsF.Text = Solver1.F(input.ds, Solver1.Solve(input)).ToString();

            ssP.Text = ListToString(Solver2.Solve(input));
            ssF.Text = Solver1.F(input.ds, Solver2.Solve(input)).ToString();

            if (dimen <= 15)
            {
                tsP.Text = ListToString(Solver3.Solve(input));
                tsF.Text = Solver1.F(input.ds, Solver3.Solve(input)).ToString();
            }
        }

        private static string ListToString(List<Solution> solution)
        {
            var t = Solver1.DistinctUntilChanged(solution);
            return t != null ? t.Aggregate("", (current, elem) => current + ((elem.index + 1) + " ")) : null;
        }

        private List<Input> CreateRandomList(int dimen, int amount)
        {
            var result = new List<Input>(); 
            while(result.Count<amount)
            {
                var r = new List<int>();
                var p = new List<int>();
                var d = new List<int>();

                r.Add(rand.Next(0, 2));
                while (r.Count < dimen)
                {
                    r.Add(rand.Next(0, 2*dimen + 1));
                }
                r.Sort();

                for (var i = 0; i < dimen; i++)
                {
                    d.Add(rand.Next(1, 2*dimen + 1));
                    p.Add(rand.Next(1, 2*dimen + 1));
                }
                result.Add(new Input(r, p, d));
            }
            return result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int dim;
            int amount;
            if (!Int32.TryParse(textBox4.Text, out amount) || !Int32.TryParse(textBox5.Text, out dim))
            {
                return;
            }

            var inpList = CreateRandomList(dim, amount);
            CreateFirstDesctiption(inpList);
            var listManyResults = new List<ManyResults>();
            foreach (var elem in inpList)
            {
                var solve = Solver1.Solve(elem);
                var firstResult = new OneResult(solve, Solver1.F(elem.ds, solve));

                solve = Solver2.Solve(elem);
                var secondResult = new OneResult(solve, Solver1.F(elem.ds, solve));

                OneResult thirdResult=null;
                if (dim <= 15)
                {
                    solve = Solver3.Solve(elem);
                    thirdResult = new OneResult(solve, Solver1.F(elem.ds, solve));
                }
                var result = new ManyResults(firstResult, secondResult, thirdResult);
                listManyResults.Add(result);
            }

            CreateSecondDescription(listManyResults);
            CreateThirdDesctiption(listManyResults);
        }

        public class ManyResults
        {
            public OneResult FirstSolution;
            public OneResult SecondSolution;
            public OneResult ThirdSolution;

            public ManyResults(OneResult firstSolution, OneResult secondSolution, OneResult thirdSolution)
            {
                FirstSolution = firstSolution;
                SecondSolution = secondSolution;
                ThirdSolution = thirdSolution;
            }
        }
        public class OneResult
        {
            public string Pi;
            public int? Fpi;

            public OneResult(List<Solution> pi, int fpi)
            {
                Fpi = fpi;
                Pi = ListToString(pi);
            }
        }

        private static void CreateFirstDesctiption(List<Input> inpList)
        {
            var file = new StreamWriter("firstDescription.txt", false);
            for(var i = 0; i<inpList.Count; i++)
            {
                file.WriteLine(i+")");
                file.Write("r: ");
                foreach (var elem in inpList[i].rs)
                {
                    file.Write(elem+" ");
                }
                file.WriteLine();

                file.Write("p: ");
                foreach (var elem in inpList[i].ps)
                {
                    file.Write(elem + " ");
                }
                file.WriteLine();

                file.Write("d: ");
                foreach (var elem in inpList[i].ds)
                {
                    file.Write(elem + " ");
                }
                file.WriteLine();
                file.WriteLine(@"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
            }
            file.Close();
        }

        private static void CreateSecondDescription(List<ManyResults> manyResList)
        {
            var file = new StreamWriter("secondDescription.txt", false);
            for (var i = 0; i < manyResList.Count; i++)
            {
                file.WriteLine(i + "." + "1)" + "Pi*=" + manyResList[i].FirstSolution.Pi);
                file.WriteLine("F(Pi*)="+manyResList[i].FirstSolution.Fpi);

                file.WriteLine(i + "." + "2)" + "Pi*=" + manyResList[i].SecondSolution.Pi);
                file.WriteLine("F(Pi*)=" + manyResList[i].SecondSolution.Fpi);

                if (manyResList[i].ThirdSolution != null)
                {
                    file.WriteLine(i + "." + "3)" + "Pi*=" + manyResList[i].ThirdSolution.Pi);
                    file.WriteLine("F(Pi*)=" + manyResList[i].ThirdSolution.Fpi);
                }
                file.WriteLine(@"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
            }
            file.Close();
        }

        private void CreateThirdDesctiption(List<ManyResults> manyResList)
        {
            var file = new StreamWriter("thirdDescription.txt", false);
            if (manyResList[0].ThirdSolution != null)
            {
                int f1mtf2, f1mtf3, f2mtf1, f2mtf3, f3mtf1, f3mtf2, f1eqf2, f1eqf3, f2eqf3;
                f1mtf2 = f1mtf3 = f2mtf1 = f2mtf3 = f3mtf1 = f3mtf2 = f1eqf2 = f1eqf3 = f2eqf3 = 0;
                foreach (var elem in manyResList)
                {
                    var t = (double)elem.FirstSolution.Fpi/elem.SecondSolution.Fpi;
                    if (t < 1)
                        f2mtf1++;
                    else if (t > 1)
                        f1mtf2++;
                    else f1eqf2++;

                    t = (double)elem.FirstSolution.Fpi / elem.ThirdSolution.Fpi;
                    if (t < 1)
                        f3mtf1++;
                    else if (t > 1)
                        f1mtf3++;
                    else f1eqf3++;

                    t = (double)elem.SecondSolution.Fpi / elem.ThirdSolution.Fpi;
                    if (t < 1)
                        f3mtf2++;
                    else if (t > 1)
                        f2mtf3++;
                    else f2eqf3++;
                }

                var f12sum = f1mtf2 + f2mtf1 + f1eqf2;
                var f23sum = f2mtf3 + f3mtf2 + f2eqf3;
                var f13sum = f1mtf3 + f3mtf1 + f1eqf3;

                file.WriteLine("f1/f2>1: " + f1mtf2 * 100 / f12sum + "%");
                file.WriteLine("f1/f2<1: " + f2mtf1 * 100 / f12sum + "%");
                file.WriteLine("f1/f2=1: " + f1eqf2 * 100 / f12sum + "%");

                file.WriteLine("f2/f3>1: " + f2mtf3 * 100 / f23sum + "%");
                file.WriteLine("f2/f3<1: " + f3mtf2 * 100 / f23sum + "%");
                file.WriteLine("f2/f3=1: " + f2eqf3 * 100 / f23sum + "%");

                file.WriteLine("f1/f3>1: " + f1mtf3 * 100 / f13sum + "%");
                file.WriteLine("f1/f3<1: " + f3mtf1 * 100 / f13sum + "%");
                file.WriteLine("f1/f3=1: " + f1eqf3 * 100 / f13sum + "%");


                chart1.Series[0].ChartType = SeriesChartType.Pie;
                chart1.Series[0].Points.AddXY("f1>f3 "+f1mtf3 * 100.0 / f13sum + "%", f1mtf3 * 100.0 / f13sum);
                chart1.Series[0].Points.AddXY("f1<f3 "+f3mtf1 * 100.0 / f13sum + "%", f3mtf1 * 100.0 / f13sum);
                chart1.Series[0].Points.AddXY("f1=f3 "+f1eqf3 * 100.0 / f13sum + "%", f1eqf3 * 100.0 / f13sum);
                chart1.Series[0].Points[0].Color = Color.Yellow;
                chart1.Series[0].Points[0].Color = Color.Blue;
                chart1.Series[0].Points[0].Color = Color.Green;
                chart1.SaveImage(pictureIteration + ".png", ChartImageFormat.Png);
                pictureIteration++;

                chart2.Series[0].ChartType = SeriesChartType.Pie;
                chart2.Series[0].Points.AddXY("f2>f3 "+f2mtf3 * 100.0 / f23sum + "%", f2mtf3 * 100.0 / f23sum);
                chart2.Series[0].Points.AddXY("f2<f3 "+f3mtf2 * 100.0 / f23sum + "%", f3mtf2 * 100.0 / f23sum);
                chart2.Series[0].Points.AddXY("f2=f3 "+f2eqf3 * 100.0 / f23sum + "%", f2eqf3 * 100.0 / f23sum);
                chart2.Series[0].Points[0].Color = Color.Yellow;
                chart2.Series[0].Points[0].Color = Color.Blue;
                chart2.Series[0].Points[0].Color = Color.Green;
                chart2.SaveImage(pictureIteration + ".png", ChartImageFormat.Png);
                pictureIteration++;
            }
            else
            {
                int f1mtf2, f2mtf1, f1eqf2;
                f1mtf2 = f2mtf1 = f1eqf2 = 0;
                foreach (var elem in manyResList)
                {
                    var t = elem.FirstSolution.Fpi / elem.SecondSolution.Fpi;
                    if (t < 0)
                        f2mtf1++;
                    else if (t > 0)
                        f1mtf2++;
                    else f1eqf2++;
                }

                var f12sum = f1mtf2 + f2mtf1 + f1eqf2;
                file.WriteLine("f1/f2>1: " + f1mtf2 * 100 / f12sum + "%");
                file.WriteLine("f1/f2<1: " + f2mtf1 * 100/ f12sum + "%");
                file.WriteLine("f1/f2=1: " + f1eqf2 * 100/ f12sum + "%");

                chart1.Series[0].ChartType = SeriesChartType.Pie;
                chart1.Series[0].Points.AddXY("f1>f2", f1mtf2 * 100.0 / f12sum);
                chart1.Series[0].Points.AddXY("f1<f2", f2mtf1 * 100.0 / f12sum);
                chart1.Series[0].Points.AddXY("f1=f2", f1eqf2 * 100.0 / f12sum);
                chart1.Series[0].Points[0].Color = Color.Yellow;
                chart1.Series[0].Points[0].Color = Color.Blue;
                chart1.Series[0].Points[0].Color = Color.Green;
                chart1.SaveImage(pictureIteration + ".png", ChartImageFormat.Png);
                pictureIteration++;
            }
            file.Close();

        }
    }
}
