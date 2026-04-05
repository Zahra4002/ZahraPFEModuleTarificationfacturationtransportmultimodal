using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.Extensions
{
    public static class TransportModeExtensions
    {
        private static readonly Dictionary<TransportMode, string> FrenchLabels = new()
        {
            { TransportMode.Maritime, "Maritime" },
            { TransportMode.Aerien, "Aérien" },
            { TransportMode.Routier, "Routier" },
            { TransportMode.Ferroviaire, "Ferroviaire" },
            { TransportMode.Fluvial, "Fluvial" }
        };

        private static readonly Dictionary<string, TransportMode> StringToMode = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Maritime", TransportMode.Maritime },
            { "Aérien", TransportMode.Aerien },
            { "Aerien", TransportMode.Aerien },
            { "Routier", TransportMode.Routier },
            { "Ferroviaire", TransportMode.Ferroviaire },
            { "Fluvial", TransportMode.Fluvial }
        };

        /// <summary>
        /// Convertit un mode de transport en label français
        /// </summary>
        public static string ToFrench(this TransportMode mode)
        {
            return FrenchLabels.TryGetValue(mode, out var label) ? label : mode.ToString();
        }

        /// <summary>
        /// Parse une chaîne en mode de transport (accepte français et anglais)
        /// </summary>
        public static bool TryParseTransportMode(string? modeString, out TransportMode mode)
        {
            mode = TransportMode.Maritime;
            
            if (string.IsNullOrWhiteSpace(modeString))
                return false;

            return StringToMode.TryGetValue(modeString, out mode);
        }

        /// <summary>
        /// Parse une chaîne en mode de transport ou lève une exception
        /// </summary>
        public static TransportMode ParseTransportMode(string? modeString)
        {
            if (!TryParseTransportMode(modeString, out var mode))
                throw new ArgumentException($"Mode de transport invalide: {modeString}");

            return mode;
        }
    }
}