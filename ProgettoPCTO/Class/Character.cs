using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace Progetto_PCTO
{
    public class Character : Entity
    {
        public Character(string URL) : base(URL)
        {

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