﻿using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ProgettoPCTO
{
    [DataContract]
    [KnownType(typeof(Entity))]
    public abstract class Entity : IBaseObject, IEquatable<Entity>
    {
        public Entity(string URL)
        {
            ImageURL = URL;
        }

        [DataMember]
        public int X { get; set; }

        [DataMember]
        public int Y { get; set; }

        [DataMember]
        public string ImageURL { get; private set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public Dictionary<string, Item> Inventory { get; set; }

        [DataMember]
        public bool IsVisible { get; set; } = true;

        [DataMember]
        public string EffectiveWeapon { get; set; }

        /// <summary>
        /// Match of a dialogue with a situation, the key is the situation, the value is the dialogue
        /// </summary>
        [DataMember]
        public string Dialogue { get; set; }

        public bool Equals(Entity other)
        {
            if (Name == other.Name && ImageURL == other.ImageURL && Description == other.Description)
                return true;

            return false;
        }
    }
}