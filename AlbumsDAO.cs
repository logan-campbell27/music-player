using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSQLMusicApp
{
    class AlbumsDAO
    {
        public List<Album> albums = new List<Album>();

        String connectionString = "datasource='192.168.86.32';port=3306;username=root;password=root;database=music2;";

        public List<Album> getAllAlbums()
        {
            List<Album> returnThese = new List<Album>();

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT ID, ALBUM_TITLE, ARTIST, YEAR, IMAGE_NAME, DESCRIPTION FROM ALBUMS", connection);
            using(MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Album a = new Album
                    {
                        ID = reader.GetInt32(0),
                        AlbumName = reader.GetString(1),
                        ArtistName = reader.GetString(2),
                        Year = reader.GetInt32(3),
                        ImageURL = reader.GetString(4),
                        Description = reader.GetString(5)
                    };
                    a.trackList = getAllTracks(a.ID);
                    returnThese.Add(a);
                }
            }
            connection.Close();
            return returnThese;
        }
        public List<Track> getAllTracks(int albumID)
        {
            List<Track> returnThese = new List<Track>();

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = new MySqlCommand();
            command.CommandText = "SELECT * FROM TRACK WHERE albums_ID = @albumid";
            command.Parameters.AddWithValue("@albumid", albumID);
            command.Connection = connection;
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Track t = new Track
                    {
                        ID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Number = reader.GetInt32(2),
                        VideoURL = reader.GetString(3),
                        Lyrics = reader.GetString(4),
                    };
                   
                    returnThese.Add(t);
                }
            }
            connection.Close();

            return returnThese;
        }

        public List<JObject> getAllTracksUsingJoin(int albumID)
        {
            List<JObject> returnThese = new List<JObject>();

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = new MySqlCommand();
            command.CommandText = "SELECT track.ID as trackID, albums.ALBUM_TITLE, `TRACK_TITLE`, `VIDEO_URL`,`LYRICS` FROM `track` JOIN albums ON albums_ID = albums.ID where albums_ID = @albumid";
            command.Parameters.AddWithValue("@albumid", albumID);
            command.Connection = connection;
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    JObject newTrack = new JObject();
                    for(int i = 0; i < reader.FieldCount; i++)
                    {
                        newTrack.Add(reader.GetName(i).ToString(), reader.GetValue(i).ToString());
                    }

                    returnThese.Add(newTrack);
                }
            }
            connection.Close();

            return returnThese;
        }

        internal int deleteTrack(int trackID)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand command = new MySqlCommand("DELETE FROM `track` WHERE `track`.`ID` = @trackID;", connection);

            command.Parameters.AddWithValue("@trackID", trackID);
            int result = command.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        public List<Album> searchTitles(String searchTerm)
        {
            List<Album> returnThese = new List<Album>();

            MySqlConnection connection = new MySqlConnection(connectionString);

            connection.Open();

            String searchWildPhrase = "%" + searchTerm + "%";
            MySqlCommand command = new MySqlCommand();
            command.CommandText = "SELECT ID, ALBUM_TITLE, ARTIST, YEAR, IMAGE_NAME, DESCRIPTION FROM ALBUMS WHERE ALBUM_TITLE LIKE @search";
            command.Parameters.AddWithValue("@search", searchWildPhrase);
            command.Connection = connection;
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Album a = new Album
                    {
                        ID = reader.GetInt32(0),
                        AlbumName = reader.GetString(1),
                        ArtistName = reader.GetString(2),
                        Year = reader.GetInt32(3),
                        ImageURL = reader.GetString(4),
                        Description = reader.GetString(5)
                    };
                    returnThese.Add(a);
                }
            }
            connection.Close();
            return returnThese;

        }

        internal int addOneAlbum(Album a)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            connection.Open();

            MySqlCommand command = new MySqlCommand("INSERT INTO `albums`(`ALBUM_TITLE`, `ARTIST`, `YEAR`, `IMAGE_NAME`, `DESCRIPTION`) " +
                "VALUES (@albumtitle,@artist,@year,@imageURL,@description)", connection);
            
            command.Parameters.AddWithValue("@albumtitle", a.AlbumName);
            command.Parameters.AddWithValue("@artist", a.ArtistName);
            command.Parameters.AddWithValue("@year", a.Year);
            command.Parameters.AddWithValue("@imageURL", a.ImageURL);
            command.Parameters.AddWithValue("@description", a.Description);

            int rows = command.ExecuteNonQuery();




            connection.Close();

            return rows;
        }
    }
}
