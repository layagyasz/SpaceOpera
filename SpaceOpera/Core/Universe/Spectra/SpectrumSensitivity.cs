using Cardamom.Mathematics;
using Cardamom.Mathematics.Color;

namespace SpaceOpera.Core.Universe.Spectra
{
    public class SpectrumSensitivity
    {
        public Interval Range { get; set; }
        public LinearApproximation? X { get; set; }
        public LinearApproximation? Y { get; set; }
        public LinearApproximation? Z { get; set; }

        public ColorCie GetColor(ISpectrum spectrum, float resolution = 1)
        {
            float x = 0;
            float y = 0;
            float z = 0;
            for (float i = Range.Minimum; i <= Range.Maximum; i += resolution)
            {
                float intensity = spectrum.GetIntensity(i);
                x += intensity * X!.GetPoint(i);
                y += intensity * Y!.GetPoint(i);
                z += intensity * Z!.GetPoint(i);
            }
            float s = x + y + z;
            return new(x / s, y / s);
        }

        public ColorCie GetColor(float wavelength)
        {
            wavelength = Range.Clamp(wavelength);
            float x = X!.GetPoint(wavelength);
            float y = Y!.GetPoint(wavelength);
            float z = Z!.GetPoint(wavelength);
            float s = x + y + z;
            return new(x / s, y / s);
        }
    }
}
