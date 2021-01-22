using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace Progetto_PCTO
{
    public class Item : Entity
    {
        public Item(string URL) : base(URL)
        {
        }

        public bool Visible
        {
            get => default;
            set
            {
            }
        }

        public Power Powers
        {
            get => default;
            set
            {
            }
        }
    }
}