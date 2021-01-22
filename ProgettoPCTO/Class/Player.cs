using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace Progetto_PCTO
{
    public class Player : Character
    {
        public Player(string URL) : base(URL)
        {
        }

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

        public Power Powers
        {
            get => default;
            set
            {
            }
        }
    }
}