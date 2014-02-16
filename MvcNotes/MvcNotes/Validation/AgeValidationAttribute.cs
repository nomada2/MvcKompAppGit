﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcNotes.Validation
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AgeValidationAttribute : ValidationAttribute
    {
        private int minAge = 1;
        private int maxAge = 100;

        public AgeValidationAttribute()
            : base("The Age should be in between 1 to 100")
        {
        }

        public override bool IsValid(object value)
        {
            int Age = (int)value;
            if (Age < this.minAge || Age > this.maxAge)
                return false;
            return true;
        }
    }

}