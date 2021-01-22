using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;



namespace ProgettoPCTO
{
    public class Gameplay: IEnumerable<Situation>
    {
        //private Situation[] situations = new Situation[20];
        public Dictionary<string, Situation> _situations = new Dictionary<string, Situation>();
        public Gameplay()
        {
            this.Initialize();
            //throw new System.NotImplementedException();
        }
        
        public int Count
        {
            get => _situations.Count;
        }

        public void Add(string key, Situation situation)
        {
            _situations.Add(key, situation);
        }

        /// <summary>
        /// Indexer of the dictionary
        /// </summary>
        /// <param name="name">name of the situation</param>
        /// <returns>Situation object, null if it doesn't exist</returns>
        public Situation this[string name]
        {
            get
            {
                if (_situations.ContainsKey(name))
                    return _situations[name];

                return null;
            }

            set
            {
                if (_situations.ContainsKey(name))
                    _situations[name] = value;
            }
        }

        public IEnumerator<Situation> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        // Cè bisogno di un config file, così fa discretamente schifo, per ora però può rimanere così
        public void Initialize()
        {
            _situations["area1"] = new Situation(URL: @"~\Img\Areas\area1.png")
            {
                Areas = new string[]
                {
                    "area2",
                    "area3",
                    null,
                    null

                },

                Entities = new List<Entity>()
                {
                    new Character(URL: @"~\Img\Characters\steve.png")
                    {
                        Name = "steve",
                        X = 100,
                        Y = 100,
                        Width = 100,
                        Height = 90,
                    }
                },
            };

            _situations["area2"] = new Situation(URL: @"~\Img\Areas\area2.png")
            {
                Areas = new string[]
                {
                    null,
                    null,
                    "area1",
                    null
                },

                Entities = new List<Entity>()
                {
                    new Character(URL: @"~\Img\Characters\creeper.png")
                    {
                        Name = "mob",
                        X = 150,
                        Y = 100,
                        Width = 100,
                        Height = 90,

                    }
                },

            };

            _situations["area3"] = new Situation(URL: @"~\Img\Areas\area3.png")
            {
                Areas = new string[] { null, null, null, "area1" }
            };
        }
    }
}