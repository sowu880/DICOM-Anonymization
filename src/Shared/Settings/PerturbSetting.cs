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
        public double Span { get; set; }

        public PerturbRangeType RangeType { get; set; }

        public int RoundTo { get; set; }

        public Func<double, double> NoiseFunction { get; set; }

        public void Validation()
        {
            if (Span < 0 || RoundTo > 28 || RoundTo < 0)
            {
                throw new DeIDFunctionException();
            }
        }

    }
}
