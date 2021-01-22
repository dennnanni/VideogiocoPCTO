using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace Progetto_PCTO
{
    public class Power : IBaseObject
    {
        public string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Armor
        {
            get => default;
            set
            {
            }
        }

        public int Experience
        {
            get => default;
            set
            {
            }
        }

        public int Strength
        {
            get => default;
            set
            {
            }
        }

        public int Health
        {
            get => default;
            set
            {
            }
        }
    }
}