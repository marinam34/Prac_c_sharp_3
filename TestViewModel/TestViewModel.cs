using Moq;
using ViewModel;
using OxyPlot.Series;
using Xunit;
using FluentAssertions;
using ClassLibrary2;

namespace TestViewModel
{
    [TestClass]
    public class TestViewModel
    {
        [Fact]
        public void ErrorScenario()
        {
            var er = new Mock<IUIServices>();
            var mvm = new MainViewModel(er.Object);
            mvm.spline_number = 0;
            mvm.Command1.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void BasicScenario()
        {
            var er = new Mock<IUIServices>();

            var mvm = new MainViewModel(er.Object);
            mvm.Functions = FRawEnum.Linear;
            mvm.begin = 0;
            mvm.end = 1;
            mvm.Command1.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Never);
            Xunit.Assert.True(mvm.splinedata.integralValue > 0);
        }
    }
}