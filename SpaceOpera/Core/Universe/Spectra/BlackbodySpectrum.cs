namespace SpaceOpera.Core.Universe.Spectra
{
    public class BlackbodySpectrum : ISpectrum
    {
        public float Temperature { get; }

        public BlackbodySpectrum(float temperature)
        {
            Temperature = temperature;
        }

        public double GetIntensity(float wavelength)
        {
            double w = wavelength * 1e-9;
            double left = 2.0 * Constants.Planck * Constants.C * Constants.C / (w * w * w * w * w);
            double right = 1.0 / (
                Math.Exp(Constants.Planck * Constants.C / (w * Constants.Boltzman * Temperature)) - 1);
            return left * right;
        }

        public float GetPeak()
        {
            return Constants.Wien / Temperature;
        }
    }
}
