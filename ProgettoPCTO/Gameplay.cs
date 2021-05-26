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
        }
        
        public int Count
        {
            get => _situations.Count;
        }

        public void Add(string key, Situation situation)
        {
            _situations.Add(key, situation);
        }

        [DataMember]
        public Dictionary<string, Situation> Situations
        {
            get => _situations;
            set => _situations = value;
        }

        [DataMember]
        public Player PlayerProfile { get; set; }

        [DataMember]
        public string CurrentAreaID { get; set; } = "area0";
        
        [DataMember]
        public int IdGameplay { get; set; }

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

        /// <summary>
        /// Saves the profile to make it possiblo to load it again
        /// </summary>
        /// <param name="server"></param>
        /// <returns>If the saving process goes well returns true</returns>
        public bool Save(HttpServerUtility server)
        {
            try
            {
                XMLHandler.Write(this, server, "~/App_Data/Data.XML");
                
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
            
        }


        /// <summary>
        /// Loads the last saving, if there are no parameters in saving file, returns the initial game
        /// </summary>
        /// <param name="server"></param>
        /// <returns>Saved game if there is, basic game if there isn't, null if problems show up</returns>
        public Gameplay Restore(HttpServerUtility server)
        {
            try
            {
                Gameplay game = XMLHandler.Read(server, "~/App_Data/SavedData.XML");

                // If no situation exists, then the file is empty and has nothing to load, it must return null
                if (game.Situations.Count == 0)
                    throw new Exception();

                return game;
                
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Sets up the entire game
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public Gameplay SetUp(HttpServerUtility server)
        {
            try
            {
                return XMLHandler.Read(server, "~/App_Data/Data.XML");
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public void Initialize()
        {
            PlayerProfile = new Player(URL: null);

            _situations["area1"] = new Situation(URL: @"~\Img\Areas\area1.png")
            {
                Name = "ingresso",
                Description = "Benvenuto, da qui inizia la tua avventura.\n",
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
                        Description = "Parla con lui per scoprire cosa vuole dirti.",
                        X = 320,
                        Y = 160,
                        Width = 110,
                        Height = 230,
                        Dialogue = "\"Io sono Steve e sarò la tua guida per le prime parti di questa avventura. Adesso ti spiegherò come " +
                            "utilizzare l'interfaccia:\n- Puoi muoverti nel gioco con i pulsanti in basso a sinistra in cui sono indicate " +
                            "le direzioni in cui puoi proseguire. Alcuni passaggi saranno chiusi e ti serviranno degli oggetti per poterli aprire.\n" +
                            "- Puoi trovare le azioni che puoi svolgere nell'ambiente qui sotto, premi \"Agisci\" per svolgere l'azione selezionata.\n " +
                            "- Potrai trovare oggetti nella tua strada, alcuni ti permetteranno di aprire passaggi e altri ti proteggeranno. Un oggetto "+
                            "può essere messo nell'inventario attraverso l'azione, se questo non è già pieno; se dovesse esserlo, potrai scegliere " +
                            "un oggetto da lasciare. Gli oggetti scompaiono se lasciati, quindi fai attenzione!\n"+
                            "- Incontrerai delle entità nel tuo cammino, alcune amiche, altre nemiche, scoprirai cosa vogliono da te.\""+
                            "\nPer iniziare, spostati all'interno del labirinto.\n",
                    }
                },
            };

            _situations["area2"] = new Situation(URL: @"~\Img\Areas\area2.png")
            {
                Name = "interno",
                Areas = new string[] { "area3", null, "area1", "area2a" },
                Actions = new List<string>()
                {
                    "Raccogli la spada", "Uccidi il creeper",
                },

                Entities = new List<Character>()
                {
                    new Character(URL: @"~\Img\Characters\creeper.png")
                    {
                        Name = "Creeper",
                        Description = "Uccidilo con la spada.",
                        X = 250,
                        Y = 160,
                        Width = 220,
                        Height = 230,
                        Dialogue =  "Hai ucciso il creeper!\n",
                        EffectiveWeapon = "Spada",
                        Strength = 5

                    }
                },

                Items = new List<Item>()
                {
                    new Item(URL: @"~\Img\Items\sword.png")
                    {
                        Name = "Spada",
                        Description = "Usala per uccidere i tuoi nemici.",
                        X = 440,
                        Y = 360,
                        Width = 100,
                        Height = 90,
                        Dialogue = "Hai aggiunto Spada al tuo inventario!\n"
                    }
                }

            };

            _situations["area2a"] = new Situation(URL: @"~\Img\Areas\area2a.png")
            {
                Name = "vicolo cieco",
                Description = "L'unica soluzione è tornare indietro...\n",
                Areas = new string[] { null, null, "area2", null },
                Actions = new List<string>()
                {
                    "Raccogli la pozione della salute",
                },
                Items = new List<Item>()
                {
                    new Item(URL: @"~\Img\Items\healthpotion.png")
                    {
                        Name = "Pozione della salute",
                        Description = "Restituisce 30 punti salute!",
                        X = 240,
                        Y = 360,
                        Width = 60,
                        Height = 60,
                        IsCollectable = true,
                        Effectiveness = 30,
                        Dialogue = "Hai aggiunto Pozione della salute al tuo inventario.\n"
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
                Areas = new string[] { "area5a", null, "area3", "area5" }
            };

            _situations["area5"] = new Situation(URL: @"~\Img\Areas\area5.png")
            {
                Name = "corridoio",
                Description = "Prosegui dritto.\n",
                Areas = new string[] { "area6", null, "area4", null },
            };

            _situations["area5a"] = new Situation(URL: @"~\Img\Areas\area5a.png")
            {
                Name = "vicolo cieco",
                Description = "L'unica possibilità è tornare indietro...\n",
                Areas = new string[] { null, null, "area4", null },
            };

            _situations["area6"] = new Situation(URL: @"~\Img\Areas\area6.png")
            {
                Name = "bivio",
                Description = "Dove vuoi andare?\n",
                Areas = new string[] { "area7", "area7a", "area5", null },
            };

            _situations["area7"] = new Situation(URL: @"~\Img\Areas\area7.png")
            {
                Name = "bivio",
                Description = "Dove vuoi andare?\n",
                Areas = new string[] { null, "area8", "area6", "area8a" },
            };

            _situations["area7a"] = new Situation(URL: @"~\Img\Areas\area7a.png")
            {
                Name = "vicolo cieco",
                Description = "L'unica possibilità è tornare indietro...\n",
                Areas = new string[] { null, null, "area6", null },
            };

            _situations["area8"] = new Situation(URL: @"~\Img\Areas\area8.png")
            {
                Name = "corridoio",
                Description = "Puoi proseguire dritto o esplorare la via sulla destra.\n",
                Areas = new string[] { "area9", "area9a", "area7", null },

            };

            _situations["area8a"] = new Situation(URL: @"~\Img\Areas\area8a.png")
            {
                Name = "vicolo cieco",
                Description = "L'unica possibilità è tornare indietro...\n",
                Actions = new List<string>()
                {
                    "Raccogli il piccone",
                },
                Areas = new string[] { null, null, "area7", null },
                Items = new List<Item>()
                {
                    new Item(URL: @"~\Img\Items\pickaxe.png")
                    {
                        Name = "Piccone",
                        Description = "Serve per rompere la pietra!",
                        X = 380,
                        Y = 280,
                        Width = 80,
                        Height = 60,
                        IsCollectable = true,
                        Dialogue = "Hai aggiunto Piccone al tuo inventario.\n",
                    }
                }
            };

            _situations["area9"] = new Situation(URL: @"~\Img\Areas\area9.png")
            {
                Name = "bivio",
                Description = "Prosegui o svolti?\n",
                Areas = new string[] { "area10", "area10a", "area8", null },
            };

            _situations["area9a"] = new Situation(URL: @"~\Img\Areas\area9a.png")
            {
                Name = "vicolo cieco",
                Description = "L'unica soluzione è tornare indietro...\n",
                Areas = new string[] { null, null, "area8", null },
            };

            _situations["area10"] = new Situation(URL: @"~\Img\Areas\area10.png")
            {
                Name = "corridoio",
                Description = "",
                Areas = new string[] { "area11", null, "area9", null },
            };

            _situations["area10a"] = new Situation(URL: @"~\Img\Areas\area10a.png")
            {
                Name = "vicolo cieco",
                Description = "L'unica soluzione è tornare indietro...\n",
                Areas = new string[] { null, null, "area9", null },
            };

            _situations["area11"] = new Situation(URL: @"~\Img\Areas\area11.png")
            {
                Name = "bivio misterioso",
                Description = "Nella rientranza il pavimento è strano... con un piccone potresti riuscire a romperlo...\n",
                Actions = new List<string>()
                {
                    "Apri il passaggio",
                },
                Areas = new string[] { null, null, "area10", "area12" },
                Items = new List<Item>()
                {
                    new Item(URL: @"~\Img\Items\ladder.png")
                    {
                        Name = "Scala",
                        Description = "Puoi usarla per superare delle avversità.",
                        X = 430,
                        Y = 300,
                        Width = 60,
                        Height = 60,
                        IsCollectable = true,
                        IsVisible = false,
                        Dialogue = "Hai aggiunto Scala al tuo inventario.\n"

                    }
                },
                UnlockingItem = "Piccone",
            };

            _situations["area12"] = new Situation(URL: @"~\Img\Areas\area12.png")
            {
                Name = "bivio",
                Description = "Dove vuoi andare?\n",
                Areas = new string[] { null, "area13", "area11", "area13a" },
            };

            _situations["area13"] = new Situation(URL: @"~\Img\Areas\area13.png")
            {
                Name = "bivio",
                Description = "Vuoi proseguire dritto o svoltare a sinistra?\n",
                Areas = new string[] { "area14", null, "area12", "area14a" },
            };

            _situations["area13a"] = new Situation(URL: @"~\Img\Areas\area13a.png")
            {
                Name = "vicolo cieco",
                Description = "L'unica possibilità è tornare indietro...\n",
                Actions = new List<string>()
                {
                    "Parla con personaggio misterioso",
                },
                Areas = new string[] { null, null, "area12", null },
                Entities = new List<Character>()
                {
                    new Character(URL: @"~\Img\Characters\character.png")
                    {
                        Name = "Personaggio misterioso",
                        Description = "Prova a parlargli",
                        X = 400,
                        Y = 160,
                        Width = 120,
                        Height = 230,
                        Dialogue = "Non ti è stato detto cosa devi fare qui... Capisco... Devi trovare la chiave che ti " +
                            "permetterà di uscire. Io non l'ho mai trovata... rimarremo qui per sempre...",


                    }
                }
            };

            _situations["area14"] = new Situation(URL: @"~\Img\Areas\area14.png")
            {
                Name = "corridoio",
                Description = "Bisogna guadare il lago per passare.\n",
                Areas = new string[] { "area15", null, "area13", null },
                Actions = new List<string>()
                {
                    "Apri il passaggio",
                    "Raccogli la pozione della salute"
                },
                UnlockingItem = "Scala",
                Items = new List<Item>()
                {
                    new Item(URL: @"~\Img\Items\healthpotion.png")
                    {
                        Name = "Pozione della salute",
                        Description = "Restituisce 30 punti salute!",
                        X = 240,
                        Y = 360,
                        Width = 60,
                        Height = 60,
                        IsCollectable = true,
                        Effectiveness = 30,
                        Dialogue = "Hai aggiunto Pozione della salute al tuo inventario.\n"
                    }
                }
            };

            _situations["area14a"] = new Situation(URL: @"~\Img\Areas\area14a.png")
            {
                Name = "percorso",
                Description = "Continua ad esplorare oppure torna indietro.\n",
                Areas = new string[] { "area14b", null, "area13", null },
            };

            _situations["area14b"] = new Situation(URL: @"~\Img\Areas\area14b.png")
            {
                Name = "percorso",
                Description = "Continua ad esplorare oppure torna indietro.\n",
                Areas = new string[] { "area14c", null, "area14a", null },
            };

            _situations["area14c"] = new Situation(URL: @"~\Img\Areas\area14c.png")
            {
                Name = "percorso",
                Description = "Continua ad esplorare oppure torna indietro.\n",
                Areas = new string[] { "area14d", null, "area14b", null },
                Actions = new List<string>()
                {
                    "Uccidi lo zombie",
                },
                Entities = new List<Character>()
                {
                    new Character(URL: @"~\Img\Characters\zombie.png")
                    {
                        Name = "Zombie",
                        Description = "Uccidilo con la spada.",
                        X = 250,
                        Y = 160,
                        Width = 200,
                        Height = 230,
                        Dialogue = "GRRRRR...\nHai ucciso zombie",
                        EffectiveWeapon = "Spada",
                        Strength = 30
                    }
                }

            };

            _situations["area14d"] = new Situation(URL: @"~\Img\Areas\area14d.png")
            {
                Name = "percorso",
                Description = "Continua ad esplorare oppure torna indietro.\n",
                Actions = new List<string>()
                {
                    "Raccogli la pozione della forza",
                },
                Areas = new string[] { "area14e", null, "area14c", null },
                Items = new List<Item>()
                {
                    new Item(URL: @"~\Img\Items\strengthpotion.png")
                    {
                        Name = "Pozione della forza",
                        Description = "Ti rafforza!",
                        X = 240,
                        Y = 360,
                        Width = 60,
                        Height = 60,
                        IsCollectable = true,
                        Effectiveness = 20,
                        Dialogue = "Hai aggiunto Pozione della forza al tuo inventario.\n"
                    }
                }

            };

            _situations["area14e"] = new Situation(URL: @"~\Img\Areas\area14e.png")
            {
                Name = "vicolo cieco",
                Description = "Che peccato, tutta questa strada per così poco...\n",
                Actions = new List<string>()
                {
                    "Uccidi lo scheletro",
                    "Raccogli la scala",
                },
                Areas = new string[] { null, null, "area14d", null },
                Entities = new List<Character>()
                {
                    new Character(URL: @"~\Img\Characters\skeleton.png")
                    {
                        Name = "Scheletro",
                        Description = "Uccidilo con la spada.",
                        X = 250,
                        Y = 160,
                        Width = 220,
                        Height = 230,
                        Dialogue = "Hai ucciso scheletro\n",
                        EffectiveWeapon = "Spada",
                        Strength = 20
                    }
                },
                Items = new List<Item>()
                {
                    new Item(URL: @"~\Img\Items\ladder.png")
                    {
                        Name = "Scala",
                        Description = "Puoi usarla per superare delle avversità.",
                        X = 310,
                        Y = 360,
                        Width = 100,
                        Height = 100,
                        IsCollectable = true,
                        IsVisible = true,
                        Dialogue = "Hai aggiunto Scala al tuo inventario.\n"

                    }
                },
            };

            _situations["area15"] = new Situation(URL: @"~\Img\Areas\area15.png")
            {
                Name = "incroncio",
                Description = "Puoi andare in tutte le direzioni.\n",
                IsUnlocked = false,
                Areas = new string[] { "area16a", "area16b", "area14", "area16" },
            };

            _situations["area16"] = new Situation(URL: @"~\Img\Areas\area16.png")
            {
                Name = "percorso",
                Description = "Continua ad esplorare oppure torna indietro.\n",
                Areas = new string[] { "area17", null, "area15", null },
            };

            _situations["area16a"] = new Situation(URL: @"~\Img\Areas\area16a.png")
            {
                Name = "vicolo cieco",
                Description = "Puoi solo tornare indietro...\n",
                Actions = new List<string>()
                {
                    "Uccidi lo scheletro"
                },
                Areas = new string[] { null, null, "area15", null },
                Entities = new List<Character>()
                {
                    new Character(URL: @"~\Img\Characters\skeleton.png")
                    {
                        Name = "Scheletro",
                        Description = "Uccidilo con la spada.",
                        X = 250,
                        Y = 160,
                        Width = 200,
                        Height = 230,
                        Dialogue = "Hai ucciso scheletro\n",
                        EffectiveWeapon = "Spada",
                        Strength = 15
                    }
                },
            };

            _situations["area16b"] = new Situation(URL: @"~\Img\Areas\area16b.png")
            {
                Name = "vicolo cieco",
                Description = "Non è la tua giornata fortunata, torna indietro.\n",
                Areas = new string[] { null, null, "area15", null },
            };

            _situations["area17"] = new Situation(URL: @"~\Img\Areas\area17.png")
            {
                Name = "muro misterioso",
                Description = "Questo muro si potrebbe rompere con un piccone.\n",
                Areas = new string[] { "area18a", "area16", null, "area18" },
                Actions = new List<string>()
                {
                    "Apri il passaggio"
                },
                UnlockingItem = "Piccone"

            };

            _situations["area18"] = new Situation(URL: @"~\Img\Areas\area18.png")
            {
                Name = "percorso",
                Description = "Continua ad esplorare oppure torna indietro.\n",
                Areas = new string[] { "area19", null, "area17", null },
            };

            _situations["area18a"] = new Situation(URL: @"~\Img\Areas\area18a.png")
            {
                Name = "luogo segreto",
                Description = "Puoi proseguire solo utilizzando una scala...\n",
                Areas = new string[] { null, "area18b", "area17", null },
                Actions = new List<string>()
                {
                    "Apri il passaggio"
                },
                IsUnlocked = false,
                UnlockingItem = "Scala"
            };

            _situations["area18b"] = new Situation(URL: @"~\Img\Areas\area18b.png")
            {
                Name = "vicolo cieco",
                Description = "Questo vicolo cieco è speciale.\n",
                Areas = new string[] { null, null, "area18a", null },
                IsUnlocked = false,
                Actions = new List<string>()
                {
                    "Raccogli il totem magico"
                },
                Items = new List<Item>()
                {
                    new Item(URL: @"~\Img\Items\magictotem.png")
                    {
                        Name = "Totem magico",
                        Description = "E' la chiave di tutto.\n",
                        X = 350,
                        Y = 190,
                        Width = 120,
                        Height = 60,
                        Dialogue = "Hai aggiunto totem magico al tuo inventario.\n",

                    }
                }
            };

            _situations["area19"] = new Situation(URL: @"~\Img\Areas\area19.png")
            {
                Name = "bivio",
                Description = "Continua ad esplorare oppure torna indietro.\n",
                Areas = new string[] { null, "area19a", "area18", "area20" },
            };

            _situations["area19a"] = new Situation(URL: @"~\Img\Areas\area19a.png")
            {
                Name = "vicolo cieco",
                Description = "Puoi solo tornare indietro... finiranno questi tempi bui.\n",
                Areas = new string[] { null, null, "area19", null },
            };

            _situations["area20"] = new Situation(URL: @"~\Img\Areas\area20.png")
            {
                Name = "percorso",
                Description = "Continua ad esplorare oppure torna indietro.\n",
                Areas = new string[] { "area21", null, "area19", null },
            };

            _situations["area21"] = new Situation(URL: @"~\Img\Areas\area21.png")
            {
                Name = "porta misteriosa",
                Description = "Questa porta si può aprire con un oggetto magico..\n",
                Areas = new string[] { "area22", null, "area20", null },
                Actions = new List<string>()
                {
                    "Apri la porta"
                },
                UnlockingItem = "Totem"
            };

            _situations["area22"] = new Situation(URL: @"~\Img\Areas\area22.gif")
            {
                Name = "VITTORIA",
                Description = "Sei sopravvissuto!",
                Areas = new string[] { null, null, null, null },
            };
        }

    }
}
