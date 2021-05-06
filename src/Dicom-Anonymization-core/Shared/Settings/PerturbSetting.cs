// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using De_Id_Function_Shared.Exceptions;
using EnsureThat;

namespace De_Id_Function_Shared
{
    public class PerturbSetting
    {
        public double Span { get; set; } = 1;

        public PerturbRangeType RangeType { get; set; } = PerturbRangeType.Proportional;

        public int RoundTo { get; set; } = 2;

        public Func<double, double> NoiseFunction { get; set; }

        public void Validate()
        {
            if (Span < 0 || RoundTo > 28 || RoundTo < 0)
            {
                throw new DeIDFunctionException();
            }
        }

    }
}
