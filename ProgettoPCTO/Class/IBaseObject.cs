﻿using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace Progetto_PCTO
{
    public interface IBaseObject
    {
        string Description { get; set; }
        string Name { get; set; }
    }
}