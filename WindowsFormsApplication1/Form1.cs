using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            dataGridView1.Rows[0].HeaderCell.Value = "Rj";
            dataGridView1.Rows[1].HeaderCell.Value = "Pj";
            dataGridView1.Rows[2].HeaderCell.Value = "Dj";
            dataGridView1.RowHeadersWidth = 60;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }

            var r = new List<int>();
            var p = new List<int>();
            var d = new List<int>();

            for (var i = 0; i < dimen; i++)
            {
                r.Add(Convert.ToInt32(dataGridView1[i, 0].Value));
                p.Add(Convert.ToInt32(dataGridView1[i, 1].Value));
                d.Add(Convert.ToInt32(dataGridView1[i, 2].Value));
            }
            var input = new Input(r, p, d);

            fsP.Text = ListToString(Solver1.Solve(input));
            fsF.Text = Solver1.F(input.ds, Solver1.Solve(input)).ToString();

            ssP.Text = ListToString(Solver2.Solve(input));
            ssF.Text = Solver1.F(input.ds, Solver2.Solve(input)).ToString();

            tsP.Text = ListToString(Solver3.Solve(input));
            tsF.Text = Solver1.F(input.ds, Solver3.Solve(input)).ToString();

            if (dimen <= 11)
            {
                var thread1 = new Thread(() => task(input));
                thread1.Start();
                //fosP.Text = ListToString(Solver4.Solve(input));
                //fosF.Text = Solver1.F(input.ds, Solver4.Solve(input)).ToString();
                //fosP.Text = fourPi;
                //fosF.Text = fourFpi.ToString();
            }
        }

        private void updatePi(string pi)
        {
            if (fosP.InvokeRequired)
            {
                fosP.Invoke(new Action<string>(updatePi), pi);
                return;
            }
            fosP.Text = pi;
        }

        private void updateFPi(string fpi)
        {
            if (fosF.InvokeRequired)
            {
                fosF.Invoke(new Action<string>(updateFPi), fpi);
                return;
            }
            fosF.Text = fpi;
        }

        private void task(Input input)
        {
            var solve = Solver4.Solve(input);
            updatePi(ListToString(solve));
            updateFPi(Solver1.F(input.ds,solve).ToString());
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

                solve = Solver3.Solve(elem);
                var thirdResult = new OneResult(solve, Solver1.F(elem.ds, solve));

                OneResult fourthResult = null;
                if (dim <= 11)
                {
                    solve = Solver4.Solve(elem);
                    fourthResult = new OneResult(solve, Solver1.F(elem.ds, solve));
                }
                var result = new ManyResults(firstResult, secondResult, thirdResult, fourthResult);
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
            public OneResult FourthSolution;

            public ManyResults(OneResult firstSolution, OneResult secondSolution, OneResult thirdSolution, OneResult fourthSolution)
            {
                FirstSolution = firstSolution;
                SecondSolution = secondSolution;
                ThirdSolution = thirdSolution;
                FourthSolution = fourthSolution;
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

                file.WriteLine(i + "." + "3)" + "Pi*=" + manyResList[i].ThirdSolution.Pi);
                file.WriteLine("F(Pi*)=" + manyResList[i].ThirdSolution.Fpi);

                if (manyResList[i].FourthSolution != null)
                {
                    file.WriteLine(i + "." + "3)" + "Pi*=" + manyResList[i].FourthSolution.Pi);
                    file.WriteLine("F(Pi*)=" + manyResList[i].FourthSolution.Fpi);
                }
                file.WriteLine(@"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
            }
            file.Close();
        }

        private void CreateThirdDesctiption(List<ManyResults> manyResList)
        {
            var file = new StreamWriter("thirdDescription.txt", false);
            if (manyResList[0].FourthSolution != null)
            {
                int f2mtf3, f3mtf2, f2eqf3, f2mtf4, f4mtf2, f2eqf4, f3mtf4, f4mtf3, f3eqf4;
                f2mtf3 = f3mtf2 = f2eqf3 = f2mtf4 = f4mtf2 = f2eqf4 = f3mtf4 = f4mtf3 = f3eqf4 = 0;
                foreach (var elem in manyResList)
                {
                    if (elem.SecondSolution.Fpi > elem.ThirdSolution.Fpi)
                        f2mtf3++;
                    else if (elem.SecondSolution.Fpi < elem.ThirdSolution.Fpi)
                        f3mtf2++;
                    else f2eqf3++;

                    if (elem.SecondSolution.Fpi > elem.FourthSolution.Fpi)
                        f2mtf4++;
                    else if (elem.SecondSolution.Fpi < elem.FourthSolution.Fpi)
                        f4mtf2++;
                    else f2eqf4++;

                    if (elem.ThirdSolution.Fpi > elem.FourthSolution.Fpi)
                        f3mtf4++;
                    else if (elem.ThirdSolution.Fpi < elem.FourthSolution.Fpi)
                        f4mtf3++;
                    else f3eqf4++;
                }

                var f23sum = f2mtf3 + f3mtf2 + f2eqf3;
                var f24sum = f2mtf4 + f4mtf2 + f2eqf4;
                var f34sum = f3mtf4 + f4mtf3 + f3eqf4;

                file.WriteLine("f2>f3: " + f2mtf3 * 100 / f23sum + "%");
                file.WriteLine("f2<f3: " + f3mtf2 * 100 / f23sum + "%");
                file.WriteLine("f2=f3: " + f2eqf3 * 100 / f23sum + "%");

                file.WriteLine("f2>f4: " + f2mtf4 * 100 / f24sum + "%");
                file.WriteLine("f2<f4: " + f4mtf2 * 100 / f24sum + "%");
                file.WriteLine("f2=f4: " + f2eqf4 * 100 / f24sum + "%");

                file.WriteLine("f3>f4: " + f3mtf4 * 100 / f34sum + "%");
                file.WriteLine("f3<f4: " + f4mtf3 * 100 / f34sum + "%");
                file.WriteLine("f3=f4: " + f3eqf4 * 100 / f34sum + "%");

                var aTR = AdditionalThirdDescription(manyResList);
                foreach (var str in aTR)
                {
                    file.WriteLine(str);
                }

                chart1.Series[0].ChartType = SeriesChartType.Pie;
                if (f2mtf4>0)
                chart1.Series[0].Points.AddXY("f2>f4 " + f2mtf4 * 100.0 / f24sum + "%", f2mtf4 * 100.0 / f24sum);
                if (f4mtf2>0)
                chart1.Series[0].Points.AddXY("f2<f4 " + f4mtf2 * 100.0 / f24sum + "%", f4mtf2 * 100.0 / f24sum);
                if (f2eqf4>0)
                chart1.Series[0].Points.AddXY("f2=f4 " + f2eqf4 * 100.0 / f24sum + "%", f2eqf4 * 100.0 / f24sum);
                chart1.Series[0].Points[0].Color = Color.Yellow;
                chart1.Series[0].Points[0].Color = Color.Blue;
                chart1.Series[0].Points[0].Color = Color.Green;
                chart1.SaveImage(pictureIteration + ".png", ChartImageFormat.Png);
                pictureIteration++;

                chart2.Series[0].ChartType = SeriesChartType.Pie;
                if (f3mtf4>0)
                chart2.Series[0].Points.AddXY("f3>f4 " + f3mtf4 * 100.0 / f34sum + "%", f3mtf4 * 100.0 / f34sum);
                if (f4mtf3>0)
                chart2.Series[0].Points.AddXY("f3<f4 " + f4mtf3 * 100.0 / f34sum + "%", f4mtf3 * 100.0 / f34sum);
                if (f3eqf4>0)
                chart2.Series[0].Points.AddXY("f3=f4 " + f3eqf4 * 100.0 / f34sum + "%", f3eqf4 * 100.0 / f34sum);
                chart2.Series[0].Points[0].Color = Color.Yellow;
                chart2.Series[0].Points[0].Color = Color.Blue;
                chart2.Series[0].Points[0].Color = Color.Green;
                chart2.SaveImage(pictureIteration + ".png", ChartImageFormat.Png);
                pictureIteration++;
            }
            else
            {
                int f3mtf2, f2mtf3, f2eqf3;
                f3mtf2 = f2mtf3 = f2eqf3 = 0;
                foreach (var elem in manyResList)
                {
                    if (elem.SecondSolution.Fpi > elem.ThirdSolution.Fpi)
                        f2mtf3++;
                    else if (elem.SecondSolution.Fpi < elem.ThirdSolution.Fpi)
                        f3mtf2++;
                    else f2eqf3++;
                }

                var f23sum = f2mtf3 + f3mtf2 + f2eqf3;
                file.WriteLine("f2>f3: " + f2mtf3 * 100 / f23sum + "%");
                file.WriteLine("f2<f3: " + f3mtf2 * 100 / f23sum + "%");
                file.WriteLine("f2=f3: " + f2eqf3 * 100 / f23sum + "%");

                var aTR = AdditionalThirdDescription(manyResList);
                foreach (var str in aTR)
                {
                    file.WriteLine(str);
                }

                chart1.Series[0].ChartType = SeriesChartType.Pie;
                if (f2mtf3>0)
                chart1.Series[0].Points.AddXY("f2>f3", f2mtf3 * 100.0 / f23sum);
                if (f3mtf2>0)
                chart1.Series[0].Points.AddXY("f2<f3", f3mtf2 * 100.0 / f23sum);
                if (f2eqf3>0)
                chart1.Series[0].Points.AddXY("f2=f3", f2eqf3 * 100.0 / f23sum);
                chart1.Series[0].Points[0].Color = Color.Yellow;
                chart1.Series[0].Points[0].Color = Color.Blue;
                chart1.Series[0].Points[0].Color = Color.Green;
                chart1.SaveImage(pictureIteration + ".png", ChartImageFormat.Png);
                pictureIteration++;
            }
            file.Close();

        }

        private List<string> AdditionalThirdDescription(List<ManyResults> manyResList)
        {
            List<string> aTD = new List<string>();
            if (manyResList[0].FourthSolution != null)
            {
                var f2f4 = new int[10];
                var f3f4 = new int[10];
                foreach (var elem in manyResList)
                {
                    var t1 = elem.SecondSolution.Fpi - elem.FourthSolution.Fpi;
                    var t2 = elem.ThirdSolution.Fpi - elem.FourthSolution.Fpi;
                    if (t1 <= 9)
                    {
                        f2f4[t1.GetValueOrDefault()]++;
                    }
                    else f2f4[9]++;
                    if (t2 <= 9)
                    {
                        f3f4[t2.GetValueOrDefault()]++;
                    }
                    else f3f4[9]++;
                }

                for(var i = 0; i < f2f4.Length; i++)
                {
                    aTD.Add("f4-f2="+i+" "+(double)f2f4[i]*100/f2f4.Sum()+"%");
                    aTD.Add("f4-f3="+i+" "+(double)f3f4[i]*100/f3f4.Sum()+"%");
                }
            }
            else
            {
                var f2f3 = new int[10];
                var f3f2 = new int[10];
                foreach (var dif in manyResList.Select(elem => elem.ThirdSolution.Fpi - elem.SecondSolution.Fpi))
                {
                    if (dif >= 0)
                    {
                        if (dif <= 9)
                        {
                            f2f3[dif.GetValueOrDefault()]++;
                        }
                        else
                        {
                            f2f3[9]++;
                        }
                    }
                    else
                    {
                        if (Math.Abs(dif.GetValueOrDefault()) <= 9)
                        {
                            f3f2[Math.Abs(dif.GetValueOrDefault())]++;
                        }
                        else
                        {
                            f3f2[9]++;
                        }
                    }
                }
                for (var i = 0; i < f2f3.Length; i++)
                {
                    aTD.Add("f3-f2>=0 && ="+i+" "+(double)f2f3[i]*100/f2f3.Sum()+"%");
                    aTD.Add("f3-f2<0 && ="+i+ " "+(double)f3f2[i]*100/f3f2.Sum()+"%");
                }
            };
            return aTD;
        }

    }
}
