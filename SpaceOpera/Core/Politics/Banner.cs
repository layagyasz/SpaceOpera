using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics
{
    class Banner
    {
        public uint Symbol { get; }
        public uint Background { get; }
        public Color PrimaryColor { get; }
        public Color SecondaryColor { get; }
        public Color SymbolColor { get; }

        public Banner(uint Symbol, uint Background, Color PrimaryColor, Color SecondaryColor, Color SymbolColor)
        {
            this.Symbol = Symbol;
            this.Background = Background;
            this.PrimaryColor = PrimaryColor;
            this.SecondaryColor = SecondaryColor;
            this.SymbolColor = SymbolColor;
        }
    }
}