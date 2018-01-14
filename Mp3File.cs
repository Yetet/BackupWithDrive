using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TagLib.Id3v2;

namespace BackupWithDrive
{
  [Serializable]
  public class Mp3File
  {
    public string Path = "";
    public string Title = "";
    public string Album = "";
    public string Artist = "";

    public Mp3File()
    {

    }

    public Mp3File(string path)
    {
      Path = path;
      parseTag(path);
    }

    private void parseTag(string path)
    {
      if (!File.Exists(path))
      {
        throw new NullReferenceException();
      }
      else
      {
        TagLib.File tagFile = TagLib.File.Create(path);
        Title += tagFile.Tag.Title;
        Album += tagFile.Tag.Album;
        Artist += tagFile.Tag.FirstAlbumArtist;
      }
    }
  }
}
