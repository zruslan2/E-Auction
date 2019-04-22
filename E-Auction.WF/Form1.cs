using E_Auction.BLL.Services;
using E_Auction.Core.DataModels;
using E_Auction.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace E_Auction.WF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            AuctionManagementService service = new AuctionManagementService();

            //List<AuctionInfoVM> auctions = service.FindAuction(AuctionManagementService.FindCategory.Description, "Yjdsq");

            //if (auctions.Count == 0) MessageBox.Show("Такого аукцина нет");

            //dataGridView1.DataSource = auctions;
            service.RestartAuction(1);
        }
    }
}
