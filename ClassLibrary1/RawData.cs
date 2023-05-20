using System.Numerics;

namespace ClassLibrary2
{
    public delegate double FRaw(double x);
    public enum FRawEnum { Linear, Cubic, Rand };
    public class RawData
    {
        public double begin { get; set; }
        public double end { get; set; }
        public int node_number { get; set; }
        public bool grid_type { get; set; }
        public FRaw Function { get; set; }
        public string func { get; set; }
        public double[] nodes { get; set; }
        public double[] values { get; set; }
        public string filename { get; set; }
        public RawData(double a, double b, int n, bool type_g, FRaw f)
        {
            begin = a;
            end = b;
            node_number = n;
            grid_type = type_g;
            Function = f;
            nodes = new double[n];
            values = new double[n];
            if (f == Linear)
            {
                func = "Linear";

            }
            else if (f == Cubic)
            {
                func = "Cubic";
            }
            else
            {
                func = "Rand";
            }
        }
        public RawData(string filename)
        {
            this.filename = filename;
        }
        static public double Linear(double x)
        {
            return 2 * x + 3;
        }
        static public double Cubic(double x)
        {
            return 2 * x * x * x + 1;
        }
        static public double Rand(double x)
        {
            Random rand = new Random();
            return rand.NextDouble() * x;
        }
        public static void Load(string filename, ref RawData? rawData)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                StreamReader streamReader = new StreamReader(fs);
                string? nodes = streamReader.ReadLine();
                string? vals = streamReader.ReadLine();
                string[] nodes_vals = nodes.Split(" ");
                string[] func_vals = vals.Split(" ");
                for (var i = 0; i < rawData.node_number; i++)
                {
                    rawData.nodes[i] = Convert.ToDouble(nodes_vals[i]);
                    rawData.values[i] = Convert.ToDouble(func_vals[i]);
                }
                streamReader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        public void Save(string filename)
        {
            FileStream fs = null;
            bool res = true;
            try
            {
                fs = new FileStream(filename, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fs);
                for (var i = 0; i < this.node_number; i++)
                {
                    streamWriter.Write($"{this.nodes[i]} ");
                }
                streamWriter.Write($"\n");
                for (var i = 0; i < this.node_number; i++)
                {
                    streamWriter.Write($"{this.values[i]} ");
                }
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
    }
}