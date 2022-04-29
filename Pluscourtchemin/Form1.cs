using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Pluscourtchemin
{
    public partial class Form1 : Form
    {
        static public double[,] matrice;
        static public int nblignes;
        static public int nbcolonnes;
        static public int xinitial;
        static public int yinitial;
        static public int xfinal;
        static public int yfinal;
        static Graphics g;

        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            xinitial = Convert.ToInt32(textBoxXInit.Text);
            yinitial = Convert.ToInt32(textBoxYinit.Text);
            xfinal = Convert.ToInt32(textBoxXFinal.Text);
            yfinal = Convert.ToInt32(textBoxYFinal.Text);
            SearchTree search = new SearchTree();
            Node2 N0 = new Node2();
            N0.x = xinitial;
            N0.y = yinitial;
            List<GenericNode> solution = search.RechercheSolutionAEtoile(N0);
            TimeSpan dur = DateTime.Now - start;
            // Affichage de l'arbre d'exploration
            search.GetSearchTree(treeView1);

            // Affichage des noeuds explorés
            // Affichage dans le picture box
            SolidBrush brush1 = new SolidBrush(Color.Yellow);
            SolidBrush brush2 = new SolidBrush(Color.Green);
            SolidBrush brush3 = new SolidBrush(Color.Blue);
            int largeur = pictureBox1.Width / nbcolonnes;
            int hauteur = pictureBox1.Height / nbcolonnes;
            Rectangle rect;
            // Les fermés
            for (int i=0; i< search.L_Fermes.Count; i++)
            {
                Node2 noeudferme = (Node2)search.L_Fermes[i];
                rect = new Rectangle(noeudferme.x * largeur, noeudferme.y * hauteur, largeur - 1, hauteur - 1);
                g.FillRectangle(brush1, rect);
            }
            // Les ouverts
            for (int i = 0; i < search.L_Ouverts.Count; i++)
            {
                Node2 noeudouvert = (Node2)search.L_Ouverts[i];
                rect = new Rectangle(noeudouvert.x * largeur, noeudouvert.y * hauteur, largeur - 1, hauteur - 1);
                g.FillRectangle(brush2, rect);
            }

            // Affichage de la solution en bleu
            listBox1.Items.Clear();
            Node2 N1 = N0;
            rect = new Rectangle(N1.x * largeur, N1.y * hauteur, largeur - 1, hauteur - 1);
            g.FillRectangle(brush3, rect);
            for (int i = 1; i < solution.Count; i++)
            {
                Node2 N2 = (Node2)solution[i];
                listBox1.Items.Add(Convert.ToString(N1.x) + "," + Convert.ToString(N1.y)
                     + " ---> " + Convert.ToString(N2.x) + "," + Convert.ToString(N2.y));
                rect = new Rectangle(N2.x * largeur, N2.y * hauteur, largeur - 1, hauteur - 1);
                g.FillRectangle(brush3, rect);
                N1 = N2;
            }
            // Affichage du nb de noeuds dans ouverts et fermés
            labelOuverts.Text = Convert.ToString(search.L_Ouverts.Count);
            labelFermes.Text = Convert.ToString(search.L_Fermes.Count);
            Console.WriteLine(dur);
        }

        private void load_environment ( string filename )
        {
            StreamReader monStreamReader = new StreamReader(filename);

            // Lecture du fichier avec un while, évidemment !
            // 1ère ligne : nombre de lignes de l'environnement
            string ligne = monStreamReader.ReadLine();
            int i = 0;
            while (ligne[i] != ':') i++;
            string strnblignes = "";
            i++; // On dépasse le ":"
            while (ligne[i] == ' ') i++; // on saute les blancs éventuels
            while (i < ligne.Length)
            {
                strnblignes = strnblignes + ligne[i];
                i++;
            }
            nblignes = Convert.ToInt32(strnblignes);

            // 2ème ligne du fichier, nombre de colonnes de l'environnement
            ligne = monStreamReader.ReadLine();
            i = 0;
            while (ligne[i] != ':') i++;
            string strnbcolonnes = "";
            i++; // On dépasse le ":"
            while (ligne[i] == ' ') i++; // on saute les blancs éventuels
            while (i < ligne.Length)
            {
                strnbcolonnes = strnbcolonnes + ligne[i];
                i++;
            }
            nbcolonnes = Convert.ToInt32(strnbcolonnes);

            // Par défaut, tout l'environnement est accessible, on met 0 dans la matrice
            matrice = new double[nbcolonnes, nblignes];
            for (i = 0; i < nbcolonnes; i++)
                for (int j = 0; j < nblignes; j++)
                    matrice[i, j] = 0;

            // Ensuite on répertorie tous les obstacles décrits dans le fichier 
            ligne = monStreamReader.ReadLine();
            while (ligne != null)
            {
                string lignex = ligne;
                i = 0;
                while (lignex[i] != ':') i++;
                i++; // on passe le :
                while (lignex[i] == ' ') i++; // on saute les blancs éventuels
                string strX = "";
                while (i < lignex.Length)
                {
                    strX = strX + lignex[i];
                    i++;
                }
                int xobstacle = Convert.ToInt32(strX);

                // On doit trouver le y de l'obstacle
                string ligney = monStreamReader.ReadLine();
                i = 0;
                while (ligney[i] != ':') i++;
                i++; // On dépasse le ":"
                // On saute les blancs éventuels
                while (ligney[i] == ' ') i++;
                string strY = "";
                while (i < ligney.Length)
                {
                    strY = strY + ligney[i];
                    i++;
                }
                int yobstacle = Convert.ToInt32(strY);

                matrice[xobstacle, yobstacle] = -1; // -1 par convention pour signifier inaccessible

                ligne = monStreamReader.ReadLine();  // On passe à l'obstacle suivant
            }
            // Fermeture du StreamReader (obligatoire) 
            monStreamReader.Close();

            // Affichage dans le picture box
            SolidBrush brush1 = new SolidBrush(Color.Gray);
            SolidBrush brush2 = new SolidBrush(Color.Black);
            int largeur = pictureBox1.Width / nbcolonnes;
            int hauteur = pictureBox1.Height / nbcolonnes;


            for (i = 0; i < nbcolonnes; i++)
                for (int j = 0; j < nblignes; j++)
                    if (matrice[i, j] == -1)
                    {
                        Rectangle rect = new Rectangle(i * largeur, j * hauteur, largeur - 1, hauteur - 1);
                        g.FillRectangle(brush2, rect);
                    }
                    else
                    {
                        Rectangle rect = new Rectangle(i * largeur, j * hauteur, largeur - 1, hauteur - 1);
                        g.FillRectangle(brush1, rect);
                    }

        }

        private void buttonInit1_Click(object sender, EventArgs e)
        {
            load_environment("environnement1.txt");
            buttonAstar.Enabled = true;
        }


        private void buttonInit2_Click(object sender, EventArgs e)
        {
            load_environment("environnement2.txt");
            buttonAstar.Enabled = true;
        }

        private void buttonInit3_Click(object sender, EventArgs e)
        {
            load_environment("environnement3.txt");
            buttonAstar.Enabled = true;
        }
    }
}
