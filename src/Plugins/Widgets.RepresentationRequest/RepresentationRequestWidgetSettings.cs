﻿using Grand.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.RepresentationRequest
{
    public class RepresentationRequestWidgetSettings : ISettings
    {
        public RepresentationRequestWidgetSettings()
        {
            LimitedToStores = new List<string>();
            LimitedToGroups = new List<string>();
        }
        public int DisplayOrder { get; set; }
        public IList<string> LimitedToStores { get; set; }

        public IList<string> LimitedToGroups { get; set; }
    }
}
