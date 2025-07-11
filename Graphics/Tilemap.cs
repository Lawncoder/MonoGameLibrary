using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Tilemap
{
    private readonly Tileset _tileset;
    private readonly int[] _tiles;

    /// <summary>
    /// Gets the total number of rows in this tilemap.
    /// </summary>
    public int Rows { get; }

    /// <summary>
    /// Gets the total number of columns in this tilemap.
    /// </summary>
    public int Columns { get; }

    /// <summary>
    /// Gets the total number of tiles in this tilemap.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets or Sets the scale factor to draw each tile at.
    /// </summary>
    public Vector2 Scale { get; set; }

    /// <summary>
    /// Gets the width, in pixels, each tile is drawn at.
    /// </summary>
    public float TileWidth => _tileset.TileWidth * Scale.X;

    /// <summary>
    /// Gets the height, in pixels, each tile is drawn at.
    /// </summary>
    public float TileHeight => _tileset.TileHeight * Scale.Y;

    public Tilemap(Tileset tileset, int columns, int rows)
    {
        _tileset = tileset;
        Rows = rows;
        Columns = columns;
        Count = Rows * Columns;
        Scale = Vector2.One;
        _tiles = new int[Count];
        

    }

    public void SetTile(int index, int tilesetID)
    {
        _tiles[index] = tilesetID;
    }

    public void SetTile(int column, int row, int tilesetID)
    {
        int index = row * Columns + column;
        SetTile(index, tilesetID);
    }

    public TextureRegion GetTile(int column, int row)
    {
        int index = row * Columns + column;
        return GetTile(index);
    }

    public TextureRegion GetTile(int index)
    {
        return _tileset.GetTile(_tiles[index]);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Count; i++)
        {
            int tileSetIndex = _tiles[i];
            TextureRegion tile = _tileset.GetTile(tileSetIndex);
            if (tile == null)
            {
                continue;
            }
            int x = i % Columns;
            int y = i / Columns;
            Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
            tile.Draw(spriteBatch, position,Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
        }
    }
    public static Tilemap FromFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;
              
               

                // The <Tileset> element contains the information about the tileset
                // used by the tilemap.
                //
                // Example
                // <Tileset region="0 0 100 100" tileWidth="10" tileHeight="10">contentPath</Tileset>
                //
                // The region attribute represents the x, y, width, and height
                // components of the boundary for the texture region within the
                // texture at the contentPath specified.
                //
                // the tileWidth and tileHeight attributes specify the width and
                // height of each tile in the tileset.
                //
                // the contentPath value is the contentPath to the texture to
                // load that contains the tileset
                XElement tilesetElement = root.Element("Tileset");

                string regionAttribute = tilesetElement.Attribute("region").Value;
                string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                int x = int.Parse(split[0]);
                int y = int.Parse(split[1]);
                int width = int.Parse(split[2]);
                int height = int.Parse(split[3]);

                int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                string contentPath = tilesetElement.Value;

                // Load the texture 2d at the content path
                Texture2D texture = content.Load<Texture2D>(contentPath);

                // Create the texture region from the texture
                TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

                // Create the tileset using the texture region
                Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                // The <Tiles> element contains lines of strings where each line
                // represents a row in the tilemap.  Each line is a space
                // separated string where each element represents a column in that
                // row.  The value of the column is the id of the tile in the
                // tileset to draw for that location.
                //
                // Example:
                // <Tiles>
                //      00 01 01 02
                //      03 04 04 05
                //      03 04 04 05
                //      06 07 07 08
                // </Tiles>
                XElement tilesElement = root.Element("Tiles");

                // Split the value of the tiles data into rows by splitting on
                // the new line character
                string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

                // Split the value of the first row to determine the total number of columns
                int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

                // Create the tilemap
                Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

                // Process each row
                for (int row = 0; row < rows.Length; row++)
                {
                    // Split the row into individual columns
                    string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    // Process each column of the current row
                    for (int column = 0; column < columnCount; column++)
                    {
                        // Get the tileset index for this location
                        int tilesetIndex = int.Parse(columns[column]);

                        // Get the texture region of that tile from the tileset
                        TextureRegion region = tileset.GetTile(tilesetIndex);

                        // Add that region to the tilemap at the row and column location
                        tilemap.SetTile(column, row, tilesetIndex);
                    }
                }

                return tilemap;
            }
        }
    }
    public static Tilemap FromFile(ContentManager content, string filename, string layerName)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);
        int mapWidth = 0;
        int mapHeight = 0;
        int tileSize = 0;
        int[][] data;
        Texture2D tileset;

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (JsonDocument document = JsonDocument.Parse(stream))
            {
                mapWidth = 
                document.RootElement.GetProperty("mapWidth").GetInt32();
                mapHeight =
                    document.RootElement.GetProperty("mapHeight").GetInt32();
                
                tileSize = document.RootElement.GetProperty("tileSize").GetInt32();
                string sourceTexture = "spritesheet";
                tileset = content.Load<Texture2D>(sourceTexture);
                data = new int[mapWidth][];
                
                for (int i = 0; i < mapWidth; i++)
                {
                    
                    data[i] = new int[mapHeight];
                    for (int j= 0; j < mapHeight; j++)
                    {
                        data[i][j] = -1;
                    }
                }
                
                foreach (JsonElement tile in document.RootElement.GetProperty("layers").EnumerateArray().Where(element =>
                             {
                                 return element.GetProperty("name").GetString().Equals(layerName);
                             }).ToList()[0].GetProperty("tiles")
                             .EnumerateArray())
                {
                    
                    int x = tile.GetProperty("x").GetInt32();
                    int y = tile.GetProperty("y").GetInt32();
                    int id = int.Parse( tile.GetProperty("id").GetString());
                  
                    
                    
                  
                  
                    data[x][y] = id;
                }
                
            }
        }
        return Tilemap.CreateTilemap(tileset, mapWidth, mapHeight, data, tileSize);
    }

    private static Tilemap CreateTilemap(Texture2D tilesetTexture, int columns, int rows, int[][] data, int tileSize)
    {
        Tileset tileset = new Tileset(new TextureRegion(tilesetTexture, 0,0,tilesetTexture.Width, tilesetTexture.Height), tileSize, tileSize);
        Tilemap tilemap = new Tilemap(tileset, columns, rows);
        for (int i = 0; i < data.Length; i++)
        {
            for(int j = 0; j < data[i].Length; j++)
            {
            
                tilemap.SetTile(i, j, data[i][j]);
            }
        }
        return tilemap;
    }
}