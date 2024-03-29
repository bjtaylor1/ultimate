﻿using System;

namespace Ultimate.ORM.Tests
{
    public class FullObjectNullables
    {
        public int Id { get; set; } // only one not nullable
        public int? IntVal { get; set; }
        public long? BigIntVal { get; set; }
        public string StringVal { get; set; }
        public AnEnum? EnumVal { get; set; }
        public double? FloatVal { get; set; }
        public decimal? DecimalVal { get; set; }
        public DateTime? DateTimeVal { get; set; }
    }
}
