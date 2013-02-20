﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Ioc;

namespace S1Nyan.Utils
{
    public class Util
    {
        private static IIndicator indicator;
        public static IIndicator Indicator
        {
            get
            {
                return indicator ?? (indicator = SimpleIoc.Default.GetInstance<IIndicator>());
            }
        }
    }
}