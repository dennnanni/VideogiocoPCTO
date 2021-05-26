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

        private SQLHandler Handler
        {
            get => (SQLHandler)Session["commands"];
            set => Session["commands"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Creates the gameplay and loads the first situation
                Game = new Gameplay().SetUp(Server);
                Handler = new SQLHandler("Data Source = (local);Initial Catalog = Videogame;Integrated Security = True;");
                Game = Handler.ReadData(Username);
                if (!(Game.PlayerProfile.Inventory is null))
                {
                    lstInventory.DataSource = Game.PlayerProfile.Inventory.Keys;
                    lstInventory.DataBind();
                }
                _selectedAction = drpActions.SelectedValue;
                if(Game.CurrentAreaID == "area0")
                {
                    _currentSituation = Game["area1"];
                    this.LoadSituation("area1");
                }
                else
                {
                    _currentSituation = Game[Game.CurrentAreaID];
                    this.LoadSituation(Game.CurrentAreaID);
                }
            }
            else
            {
                // Restores last changes
                Game.CurrentAreaID = Game.CurrentAreaID;
                _selectedAction = drpActions.SelectedValue;
                _currentSituation = Game[Game.CurrentAreaID];
                this.LoadSituation(Game.CurrentAreaID);

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

        }

        public void LoadSituation(string name) // Loading by name is much more easy to reuse code
        {
            Situation s = Game[name];
            bool print = false;

            if (Game.CurrentAreaID != name)
                print = true;

            if (!Game[name].IsUnlocked || Game.CurrentAreaID != "area0" && Game[Game.CurrentAreaID].Entities != null && Game[Game.CurrentAreaID].Entities.Count != 0)
            { 
                name = Game.CurrentAreaID;
                print = true;
            }

            s = Game[name];

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
            lblExperience.Text = "Esperienza: " + Game.PlayerProfile.Experience;
            lblHealth.Text = "Salute: " + Game.PlayerProfile.Health;


            // Loading all entities in the situation if there are
            if (s.Entities != null)
                foreach (Character e in s.Entities)
                {
                    pnlImages.Controls.Add(Game.SetEntityImage(e));
                    if (print)
                        txtStory.Text += "Hai incontrato " + e.Name + ". " + e.Description + "\n";
                }

            // Loads all items in the situation
            if(s.Items != null)
                foreach(Item i in s.Items)
                {
                    if (i.IsVisible)
                    {
                        pnlImages.Controls.Add(Game.SetEntityImage(i));
                        if (print)
                            txtStory.Text += "Hai trovato " + i.Name + ". " + i.Description + "\n";
                    }
                }

            // Save the parameters
            Game.CurrentAreaID = name;

            // Updates in the database
            Handler.UpdateGameplay(Game.IdGameplay, Game.CurrentAreaID);
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

                    // Removes the entity from the array
                    Handler.DeleteCharacter(friend.IdCharacter);
                    Game[Game.CurrentAreaID].Entities.Remove(friend);
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

            // Deletes the selected action both from the database and the gameplay
            Handler.DeleteAction(Game.IdGameplay, Game[Game.CurrentAreaID].IdSituation, _selectedAction);
            Game[Game.CurrentAreaID].Actions.Remove(_selectedAction);

            // Removes the action from the action list
            drpActions.Items.Remove(_selectedAction);

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

            Item backup = Game.PlayerProfile.Inventory[itemName];
            Game.PlayerProfile.Drop(backup.Name);
            lstInventory.Items.RemoveAt(selectedIndex);

            Game.CurrentAreaID = Game.CurrentAreaID;
            Game = Game;
        }

        private bool ItemHandler(Item item)
        {
            // Try to add item to the inventory
            try
            {
                Game.PlayerProfile.Collect(item);
                for(int i = 0; i < Game[Game.CurrentAreaID].Items.Count; i++)
                {
                    if(Game[Game.CurrentAreaID].Items[i].Name == item.Name)
                    {
                        Game[Game.CurrentAreaID].Items[i].IsCollectable = false;
                        Game[Game.CurrentAreaID].Items[i].IsVisible = false;
                    }
                }

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

            // Updates in the db
            Handler.UpdateItem(item, Game.PlayerProfile.IdCharacter);

            return true;
        }

        private void HostileEntityHandler(Character hostile)
        {
            // Player can only kill the enemy if he has the right item in his inventory
            if (hostile.EffectiveWeapon != null && Game.PlayerProfile.Inventory.ContainsKey(hostile.EffectiveWeapon))
            {
                Game.PlayerProfile.Experience += 50;

                try
                {
                    Game.PlayerProfile.Health = Game.PlayerProfile.Health - hostile.Strength;
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
                lblExperience.Text = "Esperienza: " + Game.PlayerProfile.Experience;
                lblHealth.Text = "Salute: " + Game.PlayerProfile.Health;
                lblStrength.Text = "Forza: " + Game.PlayerProfile.Strength;

                // Removes the enemy from the panel, from the situation, and from the database
                Handler.DeleteCharacter(hostile.IdCharacter);
                Game[Game.CurrentAreaID].Entities.Remove(hostile);
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
            if (Game.PlayerProfile.InventoryToString().Contains(Game[Game.CurrentAreaID].UnlockingItem))
            {
                // Replaces the image with the unlocked visual
                Situation s = Game[Game.CurrentAreaID];
                s.ImageURL = s.ImageURL.Replace(Game.CurrentAreaID, Game.CurrentAreaID + "u");
                s.Name = s.Name.Replace(" misterioso", "");
                s.Name = s.Name.Replace(" misteriosa", "");
                s.Description = "Il passaggio è aperto!";
                for(int i = 0; i < s.Areas.Length; i++)
                {
                    if(s.Areas[i] != null && Game[s.Areas[i]].IsUnlocked == false)
                    {
                        Game[s.Areas[i]].IsUnlocked = true;
                        Handler.UpdateVariables(Game.IdGameplay, Game[s.Areas[i]]);
                    }
                }
                // Checks if it is something that should be removed from the inventory
                if(s.UnlockingItem == "Scala" || s.UnlockingItem == "Totem")
                {
                    Handler.DeleteItem(Game.PlayerProfile.Inventory[s.UnlockingItem].IdItem);
                    Game.PlayerProfile.Inventory.Remove(s.UnlockingItem);
                    lstInventory.Items.Remove(s.UnlockingItem);
                }

                // Shows items if there are some
                if(s.Items != null)
                {
                    for (int i = 0; i < s.Items.Count; i++)
                    {
                        s.Items[i].IsVisible = true;
                        s.Items[i].IsCollectable = true;
                        Handler.UpdateItem(s.Items[i], -1);
                        s.Actions.Add("Raccogli " + s.Items[i].Name);
                        using(SqlConnection conn = new SqlConnection(Handler.ConnectionString))
                        {
                            conn.Open();
                            List<string> tmp = new List<string>();
                            tmp.Add("Raccogli " + s.Items[i].Name);
                            Handler.InsertActions(Game.IdGameplay, s.IdSituation, tmp, conn);
                        }
                    }
                }

                // Removes the action
                Handler.DeleteAction(Game.IdGameplay, s.IdSituation, _selectedAction);
                s.Actions.Remove(_selectedAction);
                
                Game[Game.CurrentAreaID] = s;
                this.LoadSituation(Game.CurrentAreaID);

                return true;
            }
            else
            {
                txtStory.Text += "Per aprire questo passaggio devi avere " + Game[Game.CurrentAreaID].UnlockingItem + ".\n";
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
                    foreach(string index in Game.PlayerProfile.Inventory.Keys)
                    {
                        // Gets potion effectiveness
                        if (Game.PlayerProfile.Inventory[index].Name.Contains("salute"))
                            val = Game.PlayerProfile.Inventory[index].Effectiveness;
                    }
                    message = Game.PlayerProfile.Cure(val);
                    lblHealth.Text = "Salute: " + Game.PlayerProfile.Health;
                }
                else if (selectedItem.Contains("forza"))
                {
                    int val = 0;
                    foreach (string index in Game.PlayerProfile.Inventory.Keys)
                    {
                        // Gets potion effectiveness
                        if (Game.PlayerProfile.Inventory[index].Name.Contains("forza"))
                            val = Game.PlayerProfile.Inventory[index].Effectiveness;
                    }
                    message = Game.PlayerProfile.Strengthen(val);
                    lblStrength.Text = "Forza: " + Game.PlayerProfile.Strength;
                }

                Handler.DeleteItem(Game.PlayerProfile.Inventory[lstInventory.SelectedValue].IdItem);
                Game.PlayerProfile.Inventory.Remove(lstInventory.SelectedValue);
                lstInventory.Items.Remove(lstInventory.SelectedValue);
                    
            }

            txtStory.Text += "Hai usato " + selectedItem + ". " + message + "\n";

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //Handler.WriteData(Username, Game);

            //if (!_game.Save(Server))
            //{
            //    txtStory.Text += "IMPOSSIBILE SALVARE I PROGRESSI.\n";
            //    return;
            //}
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            // Reads from the file which contains saved data
            Game = Game.Restore(Server);

            if (Game == null)
            {
                txtStory.Text += "NESSUN PROGRESSO SALVATO!\n";
                return;
            }

            lstInventory.Items.Clear();
            foreach(string itemName in Game.PlayerProfile.Inventory.Keys)
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
            _selectedAction = drpActions.SelectedValue;
            _currentSituation = Game[Game.CurrentAreaID];

            this.LoadSituation(Game.CurrentAreaID);
        }

        protected void btnRestart_Click(object sender, EventArgs e)
        {
            Game = Game.SetUp(Server);
            if(Game == null)
            {
                Page.Response.Redirect("~/Errore.aspx", true);
            }
            Game.CurrentAreaID = "area0";
            Game.CurrentAreaID = "area1";
            _selectedAction = drpActions.SelectedValue;
            _currentSituation = Game["area1"];
            Game = Game;
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
            this.LoadSituation(Game.CurrentAreaID);
        }

    }
}