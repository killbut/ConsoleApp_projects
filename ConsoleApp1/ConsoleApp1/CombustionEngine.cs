using System.Globalization;
using System.Reflection;
using System.IO;
using System.Reflection.Emit;
using System.Resources;

namespace ConsoleApp1;

public class CombustionEngine
{
    private double _T_Engine { get; set; }

    public int I { get; set; }
    public int[] M { get; set; }
    public int[] V { get; set; }
    public int T_overheat { get; set; }
    public double Hm { get; set; }
    public double Hv { get; set; }
    public double C { get; set; }

    public CombustionEngine()
    {
    }

    #region MainMethod

    public void InputFromFile()
    {
        try
        {
            using (var stream =
                   new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsoleApp1.input.txt")))
            {
                I = int.Parse(stream.ReadLine());
                var array_M = stream.ReadLine().Split(",");
                M = inputToArray(array_M);
                var array_V = stream.ReadLine().Split(",");
                V = inputToArray(array_V);
                T_overheat = int.Parse(stream.ReadLine());
                Hm = Convert.ToDouble(stream.ReadLine());
                Hv = Convert.ToDouble(stream.ReadLine());
                C = Convert.ToDouble(stream.ReadLine());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void OutputToConsole()
    {
        Console.WriteLine($"Момент инерции двигателя I(кг*м^2)={I}");
        Console.WriteLine("Крутящий момент M Скорость вращения коленвала V");
        for (var i = 0; i < M.Length; i++) Console.Write($"{M[i]}\t\t | {V[i]}\n");

        Console.WriteLine("Темпаратура перегрева T (C) = {0}", T_overheat);
        Console.WriteLine("Коэффициент зависимости скорости нагрева от крутящего момента Hm = {0:0.0000000}", Hm);
        Console.WriteLine("Коэффициент зависимости скорости нагрева от скорости вращения коленвала Hv = {0:0.0000000}",
            Hv);
        Console.WriteLine(
            "Коэффициент зависимости скорости охлаждения от температуры двигателя и окружающей среды C= {0:0.0000000}",
            C);
    }

    public void Menu()
    {
        Console.WriteLine("[1] - Изменить исходные данные");
        Console.WriteLine("[2] - Запуск двигателя");
        Console.WriteLine("[3] - Вернуть исходные данные");
        Console.WriteLine("[4] - Вывести данные");
        while (true)
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                    ChangeParameters();
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("\nВведите температуры окружающей среды");
                    Start(Convert.ToInt32(Console.Read()));
                    return;
                case ConsoleKey.D3:
                    InputFromFile();
                    break;
                case ConsoleKey.D4:
                    OutputToConsole();
                    break;
            }
    }

    private void Start(int Temperature)
    {
        _T_Engine = Temperature;
        var counter = 1;
        var speed = V[0];
        var torque = M[0];
        var time = 0;
        while (_T_Engine < T_overheat)
        {
            Console.WriteLine("Время = {0} сек", time);
            Console.WriteLine("Скорость вращения коленвала = {0}", speed);
            Console.WriteLine("Температура двигателя = {0}", _T_Engine);
            var a = torque / I;
            speed += a;
            if (speed >= V[counter])
            {
                torque = M[counter];
                speed = V[counter];
                if (counter < M.Length - 1) counter++;
            }

            _T_Engine += CalculateOverheat(speed, torque) - CalculateColling(Temperature);
            time++;
        }

        Console.WriteLine("Перегрев двигателя!");
        Console.WriteLine("Температура двигателя  = {0}", (int) _T_Engine);
        Console.WriteLine("Время прошедшее до перегрева = {0}", SecondsToMinute(time));
    }

    private void ChangeParameters()
    {
        Console.WriteLine("Введите Момент инерции двигателя I(кг*м^2)");
        Convert.ToInt32(Console.ReadLine());
        for (var i = 0; i < 6; i++)
        {
            Console.WriteLine("Введите Крутящий момент M для t{0}", i);
            Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите Скорость вращения коленвала V для t{0}", i);
            Convert.ToInt32(Console.ReadLine());
        }

        Console.WriteLine("Введите Темпаратура перегрева T (C)");
        Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Введите Коэффициент зависимости скорости нагрева от крутящего момента Hm ");
        Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Введите Коэффициент зависимости скорости нагрева от скорости вращения коленвала Hv");
        Convert.ToDouble(Console.ReadLine());
        Console.WriteLine(
            "Введите Коэффициент зависимости скорости охлаждения от температуры двигателя и окружающей среды C");
        Convert.ToDouble(Console.ReadLine());
    }

    #endregion

    #region Calculate

    private double CalculateOverheat(int speed, int torque)
    {
        return torque * Hm + Math.Pow(speed, 2) * Hv;
    }

    private double CalculateColling(int temperature)
    {
        return C * (temperature - _T_Engine);
    }

    #endregion

    #region Other

    private int[] inputToArray(string[] array)
    {
        var numbers = new int[array.Length];
        var counter = 0;
        foreach (var item in array)
        {
            numbers[counter] = int.Parse(item);
            counter++;
        }

        return numbers;
    }

    private string SecondsToMinute(int time)
    {
        return Math.Abs(time / 60).ToString() + ":" + Math.Abs(time % 60).ToString();
    }

    #endregion
}