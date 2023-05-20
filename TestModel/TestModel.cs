using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary2;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace TestModel
{
    public class TestModel
    {
        [Fact]
        public void BasicTest()
        {
            RawData rawdata = new RawData(0, 6, 5, true, RawData.Linear);
            for (int i = 0; i < rawdata.node_number; i++)
            {
                rawdata.nodes[i] = rawdata.begin + i * (rawdata.end - rawdata.begin) / (rawdata.node_number - 1);
                rawdata.values[i] = rawdata.Function(rawdata.nodes[i]);
            }
            SplineData splinedata = new SplineData(rawdata, 0, 0, 10);
            splinedata.DoSplines();
            Xunit.Assert.True(splinedata.splineDataItems[0].spline_value == rawdata.values[0]);
            Xunit.Assert.True(splinedata.splineDataItems[9].spline_value == rawdata.values[4]);
        }
    }
}