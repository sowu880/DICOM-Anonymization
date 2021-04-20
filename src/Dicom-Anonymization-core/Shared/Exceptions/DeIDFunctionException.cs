// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;

namespace De_Id_Function_Shared.Exceptions
{
    public class DeIDFunctionException : Exception
    {
        public DeIDFunctionException()
        {
        }

        public DeIDFunctionException(string message)
            : base(message)
        {
        }

        public DeIDFunctionException(DeIDFunctionErrorCode templateManagementErrorCode, string message)
            : base(message)
        {
            TemplateManagementErrorCode = templateManagementErrorCode;
        }

        public DeIDFunctionException(DeIDFunctionErrorCode templateManagementErrorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            TemplateManagementErrorCode = templateManagementErrorCode;
        }

        public DeIDFunctionErrorCode TemplateManagementErrorCode { get; }
    }
}
