namespace SpaceOpera.Core.Universe
{
    public class Star
    {
        private static readonly float s_BoltzmanConstant = 5.6704e-8f;

        public string Name { get; private set; } = string.Empty;
        public string Type { get; }
        public float TemperatureK { get; }
        public float RadiusS { get; }
        public float MassS { get; }
        public float LuminosityS { get; }

        public Star(string type, float temperatureK, float radiusS, float massS)
        {
            Type = type;
            TemperatureK = temperatureK;
            RadiusS = radiusS;
            MassS = massS;
            LuminosityS = 
                4 * s_BoltzmanConstant * MathF.PI * MathF.Pow(radiusS, 2) * MathF.Pow(temperatureK, 4) 
                / Constants.SolarLuminosity;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return string.Format(
                "[Star: Name={0}, Temperature={1}, Radius={2}, Mass={3}, Luminosity={4}",
                Name, 
                TemperatureK,
                RadiusS,
                MassS, 
                LuminosityS);
        }
    }
}