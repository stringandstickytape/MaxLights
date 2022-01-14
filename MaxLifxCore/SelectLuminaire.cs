using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaxLifxCore
{
    public partial class SelectLuminaire : Form
    {
        List<MaxLifxCoreBulbController.Controllers.ILuminaireDevice> luminaires { get; set; }
        public SelectLuminaire(List<MaxLifxCoreBulbController.Controllers.ILuminaireDevice> luminaires)
        {


            InitializeComponent();

            foreach (var l in luminaires)
                listBox1.Items.Add(l);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Tag = listBox1.SelectedItem;
            Close();
        }
    }
}
