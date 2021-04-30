// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using De_Id_Function_Shared.Exceptions;
using De_Id_Function_Shared.Model;
using EnsureThat;
using MathNet.Numerics.Distributions;

namespace De_Id_Function_Shared
{
    public class PerturbFunction
    {
        public static AgeValue Perturb(AgeValue value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            var result = Perturb(value.Age, perturbSetting);
            return new AgeValue(result, value.AgeType);
        }

        public static decimal Perturb(decimal value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            var noise = (decimal)Noise((double)value, perturbSetting);
            return Math.Round(value + noise, perturbSetting.RoundTo);
        }

        public static string Perturb(string value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(value, nameof(value));
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            try
            {
                var originValue = decimal.Parse(value);
                var perturbedValue = Perturb(originValue, perturbSetting);
                return perturbedValue.ToString();
            }
            catch (Exception ex)
            {
                throw new DeIDFunctionException();
            }
        }

        public static double Perturb(double value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            var noise = Noise(value, perturbSetting);
            return Math.Round(value + noise, perturbSetting.RoundTo);
        }

        public static float Perturb(float value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            var noise = Noise(value, perturbSetting);
            return (float)Math.Round(value + noise, perturbSetting.RoundTo);
        }

        public static int Perturb(int value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            perturbSetting.RoundTo = 0;
            var noise = Noise(value, perturbSetting);
            return (int)Math.Round(value + noise, perturbSetting.RoundTo);
        }

        public static short Perturb(short value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            perturbSetting.RoundTo = 0;
            var noise = Noise(value, perturbSetting);
            return (short)Math.Round(value + noise, perturbSetting.RoundTo);
        }

        public static long Perturb(long value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            perturbSetting.RoundTo = 0;
            var noise = Noise(value, perturbSetting);
            return (long)Math.Round(value + noise, perturbSetting.RoundTo);
        }

        public static uint Perturb(uint value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            perturbSetting.RoundTo = 0;
            var noise = Noise(value, perturbSetting);
            return (uint)Math.Round(value + noise < 0 ? 0 : value + noise, perturbSetting.RoundTo);
        }

        public static ushort Perturb(ushort value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            perturbSetting.RoundTo = 0;
            var noise = Noise(value, perturbSetting);

            return (ushort)Math.Round(value + noise < 0 ? 0 : value + noise, perturbSetting.RoundTo);
        }

        public static ulong Perturb(ulong value, PerturbSetting perturbSetting)
        {
            EnsureArg.IsNotNull(perturbSetting, nameof(perturbSetting));

            perturbSetting.RoundTo = 0;
            var noise = Noise(value, perturbSetting);
            return (ulong)Math.Round(value + noise < 0 ? 0 : value + noise, perturbSetting.RoundTo);
        }

        private static double Noise(double value, PerturbSetting perturbSetting)
        {
            perturbSetting.Validation();

            var span = perturbSetting.Span;
            if (perturbSetting.RangeType == PerturbRangeType.Proportional)
            {
                span = Math.Abs(value * perturbSetting.Span);
            }

            return perturbSetting.NoiseFunction == null ? ContinuousUniform.Sample(-1 * span / 2, span / 2) : perturbSetting.NoiseFunction(span);
        }
    }
}
