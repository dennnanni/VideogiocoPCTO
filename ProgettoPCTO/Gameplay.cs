using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;

using System.Web.UI.WebControls;

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ProgettoPCTO
{
    [DataContract]
    
    public class Gameplay
    {

        private Dictionary<string, Situation> _situations = new Dictionary<string, Situation>();

        public Gameplay()
        {
            this.Initialize();
        }
        
        public int Count
        {
            get => _situations.Count;
        }

        public void Add(string key, Situation situation)
        {
            _situations.Add(key, situation);
        }

        public Dictionary<string, Situation> Situations
        {
            get => _situations;
            set => _situations = value;
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


        // Cè bisogno di un config file, così fa discretamente schifo, per ora però può rimanere così
        public void Initialize()
        {
            _situations["area1"] = new Situation(URL: @"~\Img\Areas\area1.png")
            {
                Name = "ingresso",
                Description = "Benvenuto, da qui inizia la tua avventura.\nClicca su Steve per sapere come giocare.\n",
                Actions = new List<string> { "Parla con Steve" },
                Areas = new string[]
                {
                    "area2",
                    null,
                    null,
                    null

                },

                Entities = new List<Character>()
                {
                    new Character(URL: @"~\Img\Characters\steve.png")
                    {
                        Name = "Steve",
                        Description = "Hai incontrato Steve, clicca su di lui per scoprire cosa vuole dirti.",
                        X = 320,
                        Y = 160,
                        Width = 110,
                        Height = 230,
                        Dialogue = new Dictionary<string, string>()
                        {
                            {"area1", "Io sono Steve e sarò la tua guida per le prime parti di questa avventura. Adesso ti spiegherò come " + 
                            "utilizzare l'interfaccia:\n- puoi muoverti nel gioco con i pulsanti in basso a sinistra in cui sono indicati i nomi " +
                            "degli ambienti che puoi raggiungere. Alcuni passaggi saranno chiusi e ti serviranno degli oggetti per poterli aprire.\n"+
                            "- Potrai trovare oggetti nella tua strada, alcuni ti permetteranno di aprire passaggi e altri ti proteggeranno. Un oggetto "+
                            "può essere messo nell'inventario cliccandoci sopra, se questo non è già pieno; se dovesse esserlo, potrai scegliere " +
                            "un oggetto da lasciare. Gli oggetti scompaiono se lasciati, quindi fai attenzione!\n"+
                            "- Incontrerai delle entità nel tuo cammino, alcune amiche, altre nemiche, scoprirai cosa vogliono da te."},
                        }
                    }
                },
            };

            _situations["area2"] = new Situation(URL: @"~\Img\Areas\area2.png")
            {
                Name = "interno",
                Areas = new string[] { "area3", null, "area1", null },
                Actions = new List<string>()
                {
                    "Raccogli la spada", "Uccidi il creeper", 
                },

                Entities = new List<Character>()
                {
                    new Character(URL: @"~\Img\Characters\creeper.png")
                    {
                        Name = "Creeper",
                        Description = "Hai incontrato creeper! Uccidilo con la spada.",
                        X = 150,
                        Y = 100,
                        Width = 100,
                        Height = 90,
                        Dialogue = new Dictionary<string, string>()
                        {
                            {"area2", "Hai ucciso il creeper!\n" }
                        },
                        EffectiveWeapon = "Spada",


                    }
                },

                Items = new List<Item>()
                {
                    new Item(URL: @"~\Img\Items\sword.png")
                    {
                        Name = "Spada",
                        Description = "Qui c'è una spada. Usala per uccidere i tuoi nemici.",
                        X = 340,
                        Y = 230,
                        Width = 100,
                        Height = 90,
                        Dialogue = new Dictionary<string, string>()
                        {
                            {"area2", "Hai aggiunto Spada al tuo inventario!\n" }
                        },

                    }
                }

            };

            _situations["area3"] = new Situation(URL: @"~\Img\Areas\area3.png")
            {
                Name = "corridoio",
                Description = "",
                Areas = new string[] { "area4", null, "area2", null }
            };

            _situations["area4"] = new Situation(URL: @"~\Img\Areas\area4.png")
            {
                Name = "bivio",
                Description = "Dove vuoi andare ora?",
                Areas = new string[] { "area5", null, "area3", null }
            };

            _situations["area5"] = new Situation(URL: @"~\Img\Areas\area5.png")
            {
                Name = "vicolo cieco",
                Description = "L'unica possibilità è tornare indietro...",
                Areas = new string[] { null, null, "area4", null },
            };
        }

        public Image SetEntityImage(Entity e)
        {
            Image img = new Image();
            img.ImageUrl = e.ImageURL;
            img.Style["position"] = "absolute";
            img.Style["width"] = e.Width + "px";
            img.Style["height"] = e.Height + "px";
            img.Style["left"] = e.X + "px";
            img.Style["top"] = e.Y + "px";
            img.ID = "img" + e.Name;
            return img;
        }
    }
}
