namespace SpaceOpera.Core.Universe.Spectra
{
    public class RayleighScatteredSpectrum : ISpectrum
    {
        public ISpectrum Base { get; }

        public RayleighScatteredSpectrum(ISpectrum @base)
        {
            Base = @base;
        }

        public float GetIntensity(float wavelength)
        {
            double w = wavelength * 1e-9f;
            return (float)(Base.GetIntensity(wavelength) / (w * w * w * w));
        }
    }
}
