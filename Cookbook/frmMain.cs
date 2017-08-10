using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace Cookbook
{
    public partial class frmMain : Form
    {
        //Setting up the SoundPlayer
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();
       

        //Setting up the Connection Strings
        SqlConnection connection;
        string connectionString;

        public frmMain()
        {
            InitializeComponent();

            //Play the music
            player.Stream = Properties.Resources.ThemeSong;
            player.Play();

            //Continued Settings. Added the Cookbook connections from App Config
            connectionString = ConfigurationManager.ConnectionStrings["Cookbook.Properties.Settings.CookbookConnectionString"].ConnectionString;
        }

        //Instantiating the Function. SQL Statement to select all Recipes
        private void frmMain_Load(object sender, EventArgs e)
        {
            PopulateRecipes();
            PopulateAllIngredients();
            
        }

        //Using statements both use the same code block. Takes care of Opening/Closing connection. Sets up Display Settings/Source.
        private void PopulateRecipes()
        {
            using (connection = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Recipe", connection))
            {
                DataTable recipeTable = new DataTable();
                adapter.Fill(recipeTable);

                lstRecipes.DisplayMember = "Name";
                lstRecipes.ValueMember = "Id";
                lstRecipes.DataSource = recipeTable;
            }
        }

        //Using statements both use the same code block. Takes care of Opening/Closing connection. Sets up Display Settings/Source.
        private void PopulateAllIngredients ()
        {
            using (connection = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Ingredient", connection))
            {
                DataTable ingredientTable = new DataTable();
                adapter.Fill(ingredientTable);

                lstAllIngredients.DisplayMember = "Name";
                lstAllIngredients.ValueMember = "Id";
                lstAllIngredients.DataSource = ingredientTable;
            }
        }

        //Using statements use the same code block query. Takes care of Opening/Closing connection. Sets up Display Settings/Source.
        private void PopulateIngredients()
        {
            string query = "SELECT a.Name FROM Ingredient a " + "INNER JOIN RecipeIngredient b ON a.Id = b.IngredientID " + "WHERE b.RecipeId = @RecipeId";

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                command.Parameters.AddWithValue("@RecipeID", lstRecipes.SelectedValue);


                DataTable ingredientTable = new DataTable();
                adapter.Fill(ingredientTable);

                lstIngredients.DisplayMember = "Name";
                lstIngredients.ValueMember = "Id";
                lstIngredients.DataSource = ingredientTable;
            }
        }

        //Populates the Ingredients box everytime you select a new Recipe
        private void lstRecipes_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateIngredients();
        }

        private void btnAddRecipe_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Recipe VALUES (@RecipeName, 80, 'Visit the book of Cain to unlock', 7)";

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@RecipeName", txtRecipeName.Text);

                command.ExecuteNonQuery();
            }

            //Calling Populate Recipes will allow any new Recipes to be updated RealTime. 
            PopulateRecipes();
        }

        private void btnUpdateRecipeName_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Recipe SET Name = @RecipeName WHERE Id = @RecipeID";

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@RecipeName", txtRecipeName.Text);
                command.Parameters.AddWithValue("@RecipeId", lstRecipes.SelectedValue);

                command.ExecuteNonQuery();
            }

            //Calling Populate Recipes will allow any new Recipes to be updated RealTime. 
            PopulateRecipes();
        }

        private void btnAddToRecipe_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO RecipeIngredient VALUES (@RecipeID, @IngredientId)";

            using (connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@RecipeId", lstRecipes.SelectedValue);
                command.Parameters.AddWithValue("@IngredientId", lstAllIngredients.SelectedValue);

                command.ExecuteNonQuery();
            }

            //Calling Populate Recipes will allow any new Recipes to be updated RealTime. 
            PopulateRecipes();
        }
    }
}
