using ClassLibrary2;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ViewModel
{
    public interface IUIServices
    {
        void ReportError(string message);
        string? ReportSaveFile();
        string? ReportOpenFile();

    }

    public class MainViewModel : ViewModelBase
    {

        private readonly IUIServices uiServices;
        public double begin { get; set; }
        public double end { get; set; }
        public int node_number { get; set; }
        public bool uniform { get; set; }
        public bool not_uniform { get; set; }
        public int spline_number { get; set; }
        public double leftSecondDerivative { get; set; }
        public double rightSecondDerivative { get; set; }
        public FRawEnum Functions { get; set; }
        public FRaw fRaw { get; set; }
        public string integral_value { get; set; }

        public List<SplineDataItem> TableData
        {
            get
            {
                if (splinedata != null) return splinedata.splineDataItems;
                return null;
            }

        }
        public List<string> TableData2
        {
            get
            {
                if(rawdata != null)
                {
                    List<string> list = new List<string>();
                    for (int i = 0; i < rawdata.nodes.Length; i++)
                    {
                        list.Add(rawdata.nodes[i].ToString() + "   " + rawdata.values[i].ToString());
                    }
                    return list;
                }
                return null;
                
            }
        }


        public ICommand Command1 { get; private set; }
        public ICommand Command_save { get; private set; }
        public ICommand Command2 { get; private set; }

        public MainViewModel(IUIServices uiServices)
        {
            this.uiServices = uiServices;
            this.begin = 0;
            this.end = 1;
            this.node_number = 5;
            this.uniform = true;
            this.not_uniform = false;
            this.spline_number = 10;
            this.leftSecondDerivative = 0;
            this.rightSecondDerivative = 1;
            this.Functions = FRawEnum.Linear;
            Command1 = new RelayCommand(command1_exe, command1_can_exe);
            Command2 = new RelayCommand(command2_exe, command2_can_exe);
            Command_save = new RelayCommand(save_exe, save_can_exe);
        }

        public string this[string columnName]
        {
            get
            {
                string? message = null;
                if (columnName == "node_number")
                {
                    if (node_number < 2)
                    {
                        message = "Error";
                    }
                }
                if (columnName == "spline_number")
                {
                    if (spline_number <= 2)
                    {
                        message = "Error";
                    }
                }
                if (columnName == "begin")
                {
                    if (begin >= end)
                    {
                        message = "Error";
                    }
                }
                if (columnName == "end")
                {
                    if (begin >= end)
                    {
                        message = "Error";
                    }
                }
                return message;
            }
        }

        public RawData rawdata;
        public SplineData splinedata;

        public void ExecuteSplines_load()
        {
            try
            {
                splinedata = new SplineData(rawdata, leftSecondDerivative, rightSecondDerivative, spline_number);
                splinedata.DoSplines();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void ExecuteSplines()
        {
            try
            {
                if (Functions == FRawEnum.Linear) fRaw = RawData.Linear;
                else if (Functions == FRawEnum.Cubic) fRaw = RawData.Cubic;
                else fRaw = RawData.Rand;
                if (uniform)
                {
                    rawdata = new RawData(begin, end, node_number, true, fRaw);
                    rawdata.nodes = new double[node_number];
                    rawdata.values = new double[node_number];
                    for (int i = 0; i < node_number; i++)
                    {
                        rawdata.nodes[i] = begin + i * (end - begin) / (node_number - 1);
                        rawdata.values[i] = fRaw(rawdata.nodes[i]);
                    }
                }
                else
                {
                    rawdata = new RawData(begin, end, node_number, false, fRaw);
                    rawdata.nodes = new double[node_number];
                    rawdata.values = new double[node_number];
                    for (int i = 0; i < node_number - 1; i++)
                    {
                        if (i % 2 == 0)
                        {
                            rawdata.nodes[i] = begin + i * 0.15 * (end - begin) / (node_number - 1);
                        }
                        else
                        {
                            rawdata.nodes[i] = begin + i * (end - begin) / (node_number - 1);
                        }
                    }
                    rawdata.nodes[node_number - 1] = end;
                    Array.Sort(rawdata.nodes);
                    for (int i = 0; i < node_number; i++)
                    {
                        rawdata.values[i] = fRaw(rawdata.nodes[i]);
                    }
                }
                splinedata = new SplineData(rawdata, leftSecondDerivative, rightSecondDerivative, spline_number);
                splinedata.DoSplines();

            }

            catch (Exception ex)
            {
                throw;
            }
        }
        private void command1_exe(object sender)
        {
            try
            {
                ExecuteSplines();
                RaisePropertyChanged(nameof(TableData));
                RaisePropertyChanged(nameof(TableData2));

                integral_value = splinedata.integralValue.ToString();
                RaisePropertyChanged(nameof(integral_value));

                RaisePropertyChanged(nameof(ChartData));
            }
            catch (Exception ex)
            {
                uiServices.ReportError(ex.Message);
            }
        }

        private bool command1_can_exe(object sender)
        {
            if (this["begin"] != null || this["end"] != null || this["node_number"] != null || this["spline_number"] != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void save_exe(object sender)
        {
            string? filename = uiServices.ReportSaveFile();
            if (filename != null)
            {
                rawdata.Save(filename);
            }

        }

        private bool save_can_exe(object sender)
        {
            if (rawdata != null)
            {
                if (this["begin"] != null || this["end"] != null || this["spline_number"] != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        public void Load(string filename)
        {
            try
            {
                bool grid_type;
                if (uniform)
                {
                    grid_type = true;
                }
                else
                {
                    grid_type = false;
                }
                rawdata = new RawData(begin, end, node_number, grid_type, fRaw);
                RawData.Load(filename, ref rawdata);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void command2_exe(object sender)
        {
            string? filename = uiServices.ReportOpenFile();
            if (filename != null)
            {
                try
                {
                    Load(filename);
                    RaisePropertyChanged(nameof(TableData2));
                    ExecuteSplines_load();
                    RaisePropertyChanged(nameof(TableData));

                    integral_value = splinedata.integralValue.ToString();
                    RaisePropertyChanged(nameof(integral_value));
                    RaisePropertyChanged(nameof(ChartData));
                }
                catch (Exception ex)
                {
                    uiServices.ReportError(ex.Message);
                }
            }
        }

        private bool command2_can_exe(object sender)
        {
            if (this["begin"] != null || this["end"] != null || this["node_number"] != null || this["spline_number"] != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public PlotModel ChartData
        {
            get
            {
                if (splinedata != null && rawdata != null)
                {
                    LineSeries line_series = new LineSeries();
                    PlotModel plot_model = new PlotModel();
                    for (int i = 0; i < splinedata.splineDataItems.Count; i++)
                    {
                        line_series.Points.Add(new DataPoint(splinedata.splineDataItems[i].node, splinedata.splineDataItems[i].spline_value));
                    }
                    plot_model.Series.Add(line_series);
                    ScatterSeries scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
                    for (int i = 0; i < rawdata.node_number; i++)
                    {
                        scatterSeries.Points.Add(new ScatterPoint(rawdata.nodes[i], rawdata.values[i]));
                    }
                    plot_model.Series.Add(scatterSeries);
                    return plot_model;
                }
                else { return null; }
            }
        }

    }
}

