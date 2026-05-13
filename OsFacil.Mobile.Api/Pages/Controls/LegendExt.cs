using Syncfusion.Maui.Toolkit.Charts;

namespace OsFacil.Mobile.Api.Pages.Controls
{
    public class LegendExt : ChartLegend
    {
        protected override double GetMaximumSizeCoefficient()
        {
            return 0.5;
        }
    }
}
