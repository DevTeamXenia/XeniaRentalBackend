namespace XeniaRentalBackend.Service.Common
{
    public class GeneralService : IGeneralService
    {
        public decimal getRoundedValue(decimal value, int decimalPlaces)
        {
            var fractional = value - Math.Truncate(value);

            decimal rounded;

            if (fractional >= 0.5m)
            {
                rounded = Math.Ceiling(value);
            }
            else
            {
                rounded = Math.Floor(value);
            }
            return Math.Round(rounded, decimalPlaces);
        }



        public double MeterToKiloMeter(double meters)
        {
            return meters / 1000;
        }

    }
}
