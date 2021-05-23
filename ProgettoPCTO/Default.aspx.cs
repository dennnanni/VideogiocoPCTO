using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data.SqlClient;

namespace ProgettoPCTO
{
    enum Cardinals : int { Avanti, Destra, Indietro, Sinistra }
    public partial class Default : System.Web.UI.Page
    {
        private Gameplay _game = null;
        private string _currentAreaID = "area0";
        private Situation _currentSituation;
        private string[] _hostileEntitiesNames = { "creeper", "zombie", "scheletro" };
        private string[] _friendEntitesNames = { "steve", "personaggio misterioso" };
        private string[] _weaponsNames = { "spada", "piccone" };
        private string _selectedAction = ""; // Back up from the drop down list
        private string Username
        {
            get => Session["user"].ToString();
            set => Session["user"] = value;
        }

        private Gameplay Game
        {
            get => (Gameplay)Session["game"];
            set => Session["game"] = value;
        }

        private SQLCommands Commands
        {
            get => (SQLCommands)Session["commands"];
            set => Session["commands"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                

                // Creates the gameplay and loads the first situation
                _game = new Gameplay().SetUp(Server);
                Commands = new SQLCommands("Data Source = (local);Initial Catalog = Videogame;Integrated Security = True;");
                Commands.WriteData("default", _game);
                _game = Commands.ReadData("default");
                Game = _game;
                _selectedAction = drpActions.SelectedValue;
                _currentSituation = _game["area1"];
                this.LoadSituation("area1");
            }
            else
            {
                // Restores last changes 
                _game = (Gameplay)Game;
                _selectedAction = drpActions.SelectedValue;
                _currentSituation = _game[_game.CurrentAreaID];
                this.LoadSituation(_game.CurrentAreaID);

            }
        }

        protected void btnCardinal_Click(object sender, EventArgs e)
        {
            // Gets clicked direction
            string btnName = ((Button)sender).ID.Substring(3);
            int index = -1;
            bool exit = false;
            for (int i = 0; i < 4 && !exit; i++)
                if (btnName == ((Cardinals)i).ToString())
                {
                    index = i;
                    exit = true;
                }

            // If a cardinal button has been pressed, the textbox must be empty
            txtStory.Text = "";
            this.LoadSituation(_currentSituation.Areas[index]);

            _game.CurrentAreaID = _currentAreaID;
            Game = _game;
        }

        public void LoadSituation(string name) // Loading by name is much more easy to reuse code
        {
            Situation s = _game[name];
            bool print = false;

            if (_currentAreaID != name)
                print = true;

            if (!_game[name].IsUnlocked || _currentAreaID != "area0" && _game[_currentAreaID].Entities != null && _game[_currentAreaID].Entities.Count != 0 )
            { 
                name = _currentAreaID;
                print = true;
            }

            s = _game[name];

            // Clears the images panel from all controls and sets the background
            pnlImages.Controls.Clear();
            pnlImages.BackImageUrl = s.ImageURL; // Load background


            // If the situation is changed
            if (print)
            {
                txtStory.Text = "Hai raggiunto " + s.Name + "\n";
                txtStory.Text += s.Description;

            }

            drpActions.Items.Clear();
            // Fill the drop down list with actions in the situation, if there are
            if(s.Actions != null)
                foreach (string st in s.Actions)
                {
                    drpActions.Items.Add(st);
                }

            

            // Enables and unables direction buttons
            for (int i = 0; i < 4; i++)
            {
                if (s.Areas[i] == null)
                {
                    Button btn = pnlCardinals.FindControl("btn" + (Cardinals)i) as Button;
                    btn.Enabled = false;
                }
                else
                {
                    Button btn = pnlCardinals.FindControl("btn" + (Cardinals)i) as Button;
                    btn.Enabled = true;
                }
            }

            // Showing stats
            lblExperience.Text = "Esperienza: " + _game.PlayerProfile.Experience;
            lblHealth.Text = "Salute: " + _game.PlayerProfile.Health;


            // Loading all entities in the situation if there are
            if (s.Entities != null)
                foreach (Character e in s.Entities)
                {
                    if (e.IsVisible)
                    {
                        pnlImages.Controls.Add(_game.SetEntityImage(e));
                        if (print)
                            txtStory.Text += "Hai incontrato " + e.Name + ". " + e.Description + "\n";
                    }
                }

            // Loads all items in the situation
            if(s.Items != null)
                foreach(Item i in s.Items)
                {
                    if (i.IsVisible)
                    {
                        pnlImages.Controls.Add(_game.SetEntityImage(i));
                        if (print)
                            txtStory.Text += "Hai trovato " + i.Name + ". " + i.Description + "\n";
                    }
                }

            if (_currentAreaID != name)
                // Update the current area ID
                _currentAreaID = name;

            // Save the parameters
            _game.CurrentAreaID = name;
            Game = _game;
        }
         

        protected void btnDo_Click(object sender, EventArgs e)
        {
            // If nothing is selected or there are no entities to interact with, the method does nothing
            if (_selectedAction == "")
                return;

            Entity entity = null;

            if(_currentSituation.Entities != null)
                // Search for the character that fits the action (it is possible to have only one entity of the 
                // same type in a situation so there's no problem of ambiguity)
                foreach(Character c in _currentSituation.Entities)
                    if(_selectedAction.ToLower().Contains(c.Name.ToLower()))
                    {
                        entity = c;
                        break;
                    }

            if(_currentSituation.Items != null)
                // Search for the item that fits the action (same of above)
                foreach(Item i in _currentSituation.Items)
                    if (_selectedAction.ToLower().Contains(i.Name.ToLower()))
                    {
                        entity = i;
                        break;
                    }

            if(entity != null)
            {
                if (entity.GetType() == typeof(Item) && (entity as Item).IsCollectable)
                {
                    if(!ItemHandler(entity as Item))
                    {
                        return;
                    }
                }

                // If the monster name is in the action it means it will be killed
                if (_hostileEntitiesNames.Contains(entity.Name.ToLower()))
                {
                    HostileEntityHandler(entity as Character);
                }

                if (_friendEntitesNames.Contains(entity.Name.ToLower()))
                {
                    Character friend = entity as Character;

                    // Removes the enemy from the panel and from the situation
                    _game[_currentAreaID].Entities.Remove(friend);
                    pnlImages.Controls.Remove(pnlImages.FindControl("img" + friend.Name));
                }

                // Shows the dialogue and remove the action from the situation and the drp
                txtStory.Text += entity.Dialogue;
            }

            if (_selectedAction.ToLower().Contains("apri"))
            {
                if (!SituationHandler())
                    return;
            }

            _game[_currentAreaID].Actions.Remove(_selectedAction);

            // Removes the action from the action list
            drpActions.Items.Remove(_selectedAction);

            _game.CurrentAreaID = _currentAreaID;
            Game = _game;
        }

        protected void btnDrop_Click(object sender, EventArgs e)
        {
            int selectedIndex = lstInventory.SelectedIndex;
            if (selectedIndex == -1) // If nothing is selected
                return;

            string itemName = lstInventory.SelectedValue;

            if (_weaponsNames.Contains(itemName.ToLower()))
            {
                txtStory.Text += "\"Non credo sia sensato lasciare un'arma... con cosa pensi di proteggerti?\"\n";
                return;
            }

            // Shows the message
            txtStory.Text += "Hai lasciato " + itemName + "\n";

            Item backup = _game.PlayerProfile.Inventory[itemName];
            _game.PlayerProfile.Drop(backup.Name);
            lstInventory.Items.RemoveAt(selectedIndex);

            _game.CurrentAreaID = _currentAreaID;
            Game = _game;
        }

        private bool ItemHandler(Item item)
        {
            // Try to add item to the inventory
            try
            {
                _game.PlayerProfile.Collect(item);
                _game[_currentAreaID].Items.Remove(item);

                pnlImages.Controls.Remove(pnlImages.FindControl("img" + item.Name));
            }
            catch (Exception ex)
            {
                // If an exception is generated, inventory is full and next instructions must not be done
                txtStory.Text += ex.Message;
                return false;
            }

            // Adds the name to the inventory list
            lstInventory.Items.Add(item.Name);

            return true;
        }

        private void HostileEntityHandler(Character hostile)
        {
            // Player can only kill the enemy if he has the right item in his inventory
            if (hostile.EffectiveWeapon != null && _game.PlayerProfile.Inventory.ContainsKey(hostile.EffectiveWeapon))
            {
                hostile.IsVisible = false;
                _game.PlayerProfile.Experience += 50;

                try
                {
                    _game.PlayerProfile.Health = _game.PlayerProfile.Health - hostile.Strength;
                }
                catch (Exception ex)
                {
                    txtStory.Text = ex.Message;
                    foreach(Control b in pnlCardinals.Controls)
                    {
                        Button button = b as Button;
                        if(button != null)
                            button.Enabled = false;
                    }
                    btnDo.Enabled = false;
                    btnDrop.Enabled = false;
                    btnUse.Enabled = false;
                    btnSave.Enabled = false;

                    return;
                }
                

                // Updates all labels
                lblExperience.Text = "Esperienza: " + _game.PlayerProfile.Experience;
                lblHealth.Text = "Salute: " + _game.PlayerProfile.Health;
                lblStrength.Text = "Forza: " + _game.PlayerProfile.Strength;

                // Removes the enemy from the panel and from the situation
                _game[_currentAreaID].Entities.Remove(hostile);
                pnlImages.Controls.Remove(pnlImages.FindControl("img" + hostile.Name));
            }
            else if (hostile.EffectiveWeapon != null)
            {
                txtStory.Text += "Devi avere " + hostile.EffectiveWeapon + " per farlo.\n";

                return;
            }
        }

        private bool SituationHandler()
        {
            if (_game.PlayerProfile.InventoryToString().Contains(_game[_currentAreaID].UnlockingItem))
            {
                // Replaces the image with the unlocked visual
                Situation s = _game[_currentAreaID];
                s.ImageURL = s.ImageURL.Replace(_currentAreaID, _currentAreaID + "u");
                s.Name = s.Name.Replace(" misterioso", "");
                s.Name = s.Name.Replace(" misteriosa", "");
                s.Description = "Il passaggio è aperto!";

                // Checks if it is something that should be removed from the inventory
                if(s.UnlockingItem == "Scala" || s.UnlockingItem == "Totem")
                {
                    _game.PlayerProfile.Inventory.Remove(s.UnlockingItem);
                    lstInventory.Items.Remove(s.UnlockingItem);
                }

                // Finds the locked area and unlocks it
                for(int i =  0; i < s.Areas.Length; i++)
                {
                    if (s.Areas[i] != null && !_game[s.Areas[i]].IsUnlocked)
                    {
                        _game[s.Areas[i]].IsUnlocked = true;
                        i = 4;
                    }
                }

                // Shows items if there are some
                if(s.Items != null)
                {
                    for (int i = 0; i < s.Items.Count; i++)
                    {
                        s.Items[i].IsVisible = true;
                        s.Actions.Add("Raccogli " + s.Items[i].Name);
                    }
                }

                // Removes the action
                s.Actions.Remove(_selectedAction);
                
                _game[_currentAreaID] = s;
                this.LoadSituation(_currentAreaID);

                return true;
            }
            else
            {
                txtStory.Text += "Per aprire questo passaggio devi avere " + _game[_currentAreaID].UnlockingItem + ".\n";
                return false;
            }
        }

        protected void btnUse_Click(object sender, EventArgs e)
        {
            string selectedItem = lstInventory.SelectedValue.ToLower();
            string message = "";

            if(selectedItem == "")
            {
                txtStory.Text += "Devi selezionare qualcosa da usare!\n";
                return;
            }

            if(!selectedItem.Contains("pozione") && !selectedItem.Contains("armatura"))
            {
                txtStory.Text += "Non puoi utilizzare questo oggetto.\n";
                return;
            }

            if (selectedItem.Contains("pozione"))
            {
                if (selectedItem.Contains("salute"))
                {
                    int val = 0;
                    // Searches the potion in the inventory and get its effectiveness
                    foreach(string index in _game.PlayerProfile.Inventory.Keys)
                    {
                        // Gets potion effectiveness
                        if (_game.PlayerProfile.Inventory[index].Name.Contains("salute"))
                            val = _game.PlayerProfile.Inventory[index].Effectiveness;
                    }
                    message = _game.PlayerProfile.Cure(val);
                    lblHealth.Text = "Salute: " + _game.PlayerProfile.Health;
                }
                else if (selectedItem.Contains("forza"))
                {
                    int val = 0;
                    foreach (string index in _game.PlayerProfile.Inventory.Keys)
                    {
                        // Gets potion effectiveness
                        if (_game.PlayerProfile.Inventory[index].Name.Contains("forza"))
                            val = _game.PlayerProfile.Inventory[index].Effectiveness;
                    }
                    message = _game.PlayerProfile.Strengthen(val);
                    lblStrength.Text = "Forza: " + _game.PlayerProfile.Strength;
                }

                _game.PlayerProfile.Inventory.Remove(lstInventory.SelectedValue);
                lstInventory.Items.Remove(lstInventory.SelectedValue);
                    
            }

            txtStory.Text += "Hai usato " + selectedItem + ". " + message + "\n";

            Game = _game;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Commands.WriteData(Username, Game);

            //if (!_game.Save(Server))
            //{
            //    txtStory.Text += "IMPOSSIBILE SALVARE I PROGRESSI.\n";
            //    return;
            //}
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            // Reads from the file which contains saved data
            _game = _game.Restore(Server);

            if (_game == null)
            {
                txtStory.Text += "NESSUN PROGRESSO SALVATO!\n";
                return;
            }

            lstInventory.Items.Clear();
            foreach(string itemName in _game.PlayerProfile.Inventory.Keys)
            {
                lstInventory.Items.Add(itemName);
            }

            // Enables the interface
            foreach (Button b in pnlCardinals.Controls)
            {
                b.Enabled = true;
            }
            btnDo.Enabled = true;
            btnDrop.Enabled = true;
            btnUse.Enabled = true;
            btnSave.Enabled = true;

            // Stores the gameplay structure
            Game = _game;
            _selectedAction = drpActions.SelectedValue;
            _currentSituation = _game[_game.CurrentAreaID];

            this.LoadSituation(_game.CurrentAreaID);
        }

        protected void btnRestart_Click(object sender, EventArgs e)
        {
            _game = _game.SetUp(Server);
            if(_game == null)
            {
                Page.Response.Redirect("~/Errore.aspx", true);
            }
            _currentAreaID = "area0";
            _game.CurrentAreaID = "area1";
            _selectedAction = drpActions.SelectedValue;
            _currentSituation = _game["area1"];
            Game = _game;
            foreach (Control b in pnlCardinals.Controls)
            {
                Button button = b as Button;
                if (button != null)
                    button.Enabled = true;
            }
            btnDo.Enabled = true;
            btnDrop.Enabled = true;
            btnUse.Enabled = true;
            btnSave.Enabled = true;
            lstInventory.Items.Clear();
            this.LoadSituation(_game.CurrentAreaID);
        }

    }
}