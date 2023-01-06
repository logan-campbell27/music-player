using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseSQLMusicApp
{
    public partial class Form1 : Form
    {

        BindingSource albumBindingSource = new BindingSource();
        BindingSource trackBindingSource = new BindingSource();
        List<Album> albums = new List<Album>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AlbumsDAO albumsDAO = new AlbumsDAO();
            Album a1 = new Album
            {
                ID = 1,
                AlbumName = "My first album",
                ArtistName = "Logan Campbell",
                Year = 2022,
                ImageURL = "Nothing yet",
                Description = "Nothing special"

            };
            Album a2 = new Album
            {
                ID = 1,
                AlbumName = "My second album",
                ArtistName = "Logan Campbell",
                Year = 2022,
                ImageURL = "Nothing yet",
                Description = "Nothing special"
            };

            albumsDAO.albums.Add(a1);
            albumsDAO.albums.Add(a2);

            albumBindingSource.DataSource = albumsDAO.getAllAlbums();
            dataGridView1.DataSource = albumBindingSource;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AlbumsDAO albumsDAO = new AlbumsDAO();
            albumBindingSource.DataSource = albumsDAO.searchTitles(textBox1.Text);
            dataGridView1.DataSource = albumBindingSource;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;

            int rowClicked = dataGridView.CurrentRow.Index;

            String imageURL = dataGridView.Rows[rowClicked].Cells[4].Value.ToString();

            pictureBox1.Load(imageURL);

            AlbumsDAO albumsDAO = new AlbumsDAO();
            trackBindingSource.DataSource = albumsDAO.getAllTracksUsingJoin((int)dataGridView.Rows[rowClicked].Cells[0].Value);
            dataGridView2.DataSource = trackBindingSource;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Album a = new Album
            {

                AlbumName = txt_album.Text,
                ArtistName = txt_artist.Text,
                Year = Int32.Parse(txt_year.Text),
                ImageURL = txt_image.Text,
                Description = txt_description.Text
            };
            AlbumsDAO albumsDAO = new AlbumsDAO();
            int result = albumsDAO.addOneAlbum(a);
            MessageBox.Show(result + " new row(s) inserted");
        }

        private void dataGridView2_click(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;

            int rowClicked = dataGridView.CurrentRow.Index;

            String videoURL = dataGridView.Rows[rowClicked].Cells[3].Value.ToString();

            webView21.Source = new Uri(videoURL);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            int rowClicked = dataGridView2.CurrentRow.Index;
            int trackID = int.Parse(dataGridView2.Rows[rowClicked].Cells[0].Value.ToString());
            MessageBox.Show("ID of deleted item: " + trackID);

            AlbumsDAO albumsDAO = new AlbumsDAO();
            int result = albumsDAO.deleteTrack(trackID);
            MessageBox.Show(result + " track deleted");
            dataGridView2.DataSource = null;
        }
    }

}
