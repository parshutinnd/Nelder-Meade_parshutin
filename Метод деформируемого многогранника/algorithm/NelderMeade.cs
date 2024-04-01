using System;
using System.Linq.Expressions;
using OptimizationMethods.models;

namespace OptimizationMethods.methods
{
    public class NelderMeade
    {
        int n = 0; //размерность
        Expression exp = new Expression("0", null, null);
        double alpha = 0; //коэффициент отражения
        double beta = 0;  //коэффициент растяжения
        double gamma = 0; //коэффициент сжатия
        double l = 0; //начальное отклонение
        double epsilon = 0; //эпсилон
        int FrequencyOfUpdates = 0; //период обновления симплекса
        int precision = -1; //точность округление

        double[] StatrPoint;
        Point[] simplex = new Point[n + 1];

        public NelderMeade() 
        {
            Console.Write("Количество переменных (размерность) n = ");
            while (n <= 0) n = protect_int_input();

            Console.Write("Коэффициент отражения alpha = ");
            while (alpha <= 0) alpha = protect_double_input();

            Console.Write("Коэффициент растяжения beta = ");
            while (beta <= 1) beta = protect_double_input();

            Console.Write("Коэффициент сжатия gamma = ");
            while ((gamma >= 1) || (gamma <= 0)) gamma = protect_double_input();

            Console.Write("Начальное отклонение l = ");
            while (l <= 0) l = protect_double_input();

            Console.Write("Для условия останова epsilon = ");
            while (epsilon <= 0) epsilon = protect_double_input();

            Console.Write("Период обновления симлпекса FrequencyOfUpdates = ");
            while (FrequencyOfUpdates < 2) FrequencyOfUpdates = protect_int_input();

            Console.WriteLine("Округление (число знаков после запятой) = ");
            while (precision < 0) precision = protect_int_input();


            Console.Write("Начальная точка X0 = ");
            string[] x0;
            double[] StatrPoint;
            Point[] simplex = new Point[n + 1];
            bool correct = false;
            do
            {
                x0 = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (x0.Length < n)
                {
                    Console.Write("Неверный формат! X0 = ");
                    continue;
                }
                StatrPoint = new double[n];
                for (int i = 0; i < n; i++)
                    if (!double.TryParse(x0[i], out StatrPoint[i]))
                    {
                        Console.Write("Неверный формат! X0 = ");
                        continue;
                    }
                simplex[0] = new Point(StatrPoint, exp);
                correct = true;
            }
            while (!correct);
        }





        private double Dispersion(Point[] simplex)
        {
            int n = simplex[0].dimension;
            double disp = 0;
            for (int j = 1; j < n + 1; j++)
                disp += Math.Pow(simplex[j].value - simplex[0].value, 2);
            disp /= n;
            return Math.Sqrt(disp);
        }
        private Point Centre(Point[] simplex)
        {
            int n = simplex[0].dimension;
            double[] point = new double[n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    point[i] += simplex[j].coords[i];
                point[i] /= n;
            }
            return new Point(point);
        }
        private Point Reflection(Point[] simplex, Point centre, double alpha, Expression exp)
        {
            int n = simplex[0].dimension;
            double[] point = new double[n];
            for (int i = 0; i < n; i++)
                point[i] = centre.coords[i] + alpha * (centre.coords[i] - simplex[n].coords[i]);
            return new Point(point, exp);
        }
        private Point Stretching(Point centre, Point refPoint, double beta, Expression exp)
        {
            int n = centre.dimension;
            double[] point = new double[n];
            for (int i = 0; i < n; i++)
                point[i] = centre.coords[i] + beta * (refPoint.coords[i] - centre.coords[i]);
            return new Point(point, exp);
        }

        private Point Compression(Point[] simplex, Point centre, Point refPoint, double gamma, Expression exp)
        {
            int n = simplex[0].dimension;
            double[] point = new double[n];
            if (simplex[n].value <= refPoint.value)
                for (int i = 0; i < n; i++)
                    point[i] = centre.coords[i] + gamma * (simplex[n].coords[i] - centre.coords[i]);
            else
                for (int i = 0; i < n; i++)
                    point[i] = centre.coords[i] + gamma * (refPoint.coords[i] - centre.coords[i]);
            return new Point(point, exp);
        }

        private void GlobalComprerssion(Point[] simplex, Expression exp)
        {
            int n = simplex[0].dimension;
            double[] newPoint;
            for (int i = 1; i < n + 1; i++)
            {
                newPoint = new double[n];
                for (int j = 0; j < n; j++)
                    newPoint[j] = 0.5 * (simplex[i].coords[j] + simplex[0].coords[j]);
                simplex[i] = new Point(newPoint, exp);
            }
        }
        private void GenerateSimplex(Point[] simplex, Expression exp, double l)
        {
            int n = simplex[0].dimension;
            double[] coords;
            for (int i = 1; i < n + 1; i++)
            {
                coords = (double[])simplex[0].coords.Clone();
                coords[i - 1] += l;
                simplex[i] = new Point(coords, exp);
            }
        }

        private void PrintSimplex(Point[] simplex, int NumInLine = 1, int precision = 3)
        {
            int n = simplex[0].dimension;
            for (int i = 0; i < n; i++)
            {
                simplex[i].PrintPointWithValue($"X{i}");
                Console.Write("; ");
                if ((i + 1) % NumInLine == 0) Console.WriteLine();
            }
            simplex[n].PrintPointWithValue($"X{n}");
            Console.WriteLine();
        }
        private static double protect_double_input()
        {
            double num = 0;
            while (true)
            {
                string text = Console.ReadLine();
                if (double.TryParse(text, out num)) break;
                Console.Write("Неверный формат! Попробуйте ещё: ");
            }
            Console.WriteLine("");
            return num;
        }
        private static int protect_int_input()
        {
            int num = 0;
            while (true)
            {
                string text = Console.ReadLine();
                if (int.TryParse(text, out num)) break;
                Console.Write("Неверный формат! Попробуйте ещё: ");
            }
            
            Console.WriteLine("");
            return num;
            
        }
    }

   
}
}
