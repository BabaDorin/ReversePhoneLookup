﻿using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture.Xunit2;

namespace ReversePhoneLookup.Models.UnitTests.Shared
{
    public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] objects) : base(new AutoMoqDataAttribute(), objects) { }
    }
}
