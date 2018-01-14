using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BackupWithDrive;
using File = TagLib.Tiff.Rw2.File;

/// <summary>
/// Final project for 2017-2018 | First semester | Programming II class with Mrs. Finoli.
/// Started on : 01/14/2018
/// Due on : 01/16/2018
/// 
/// Backup all files from a directory to the user's Google Drive 
/// account using Google Drive REST API.
/// </summary>
class Program
{
  // List of files in the directory.
  private static List<string> _mp3List = new List<string>();
  private static List<string> _driveFileNames;
  private static List<Mp3File> _mp3Files = new List<Mp3File>();

  /// <summary>
  /// Get all files last modified before a certain date in a directory recursively.
  /// </summary>
  /// <param name="dir">Directory to walk through.</param>
  /// <param name="date">The date of which to add files from before.</param>
  private static void getFilesRecursive(string dir, DateTime date)
  {
    DirectoryInfo di = new DirectoryInfo(dir);
    // Try to search through each directory within dir.
    try
    {
      // Foreach directory in dir.
      foreach (DirectoryInfo d in di.GetDirectories())
      {
        // Filter out not needed files.
        if (!di.FullName.Contains("$"))
        {
          Console.WriteLine(d.FullName + " | " + di.CreationTime);
          // Foreach file in each directory within dir.
          try
          {
            foreach (FileInfo f in d.GetFiles())
            {
              // If file LastWriteTime was before date, then...
              if (DateTime.Compare(f.CreationTime.Date, date) > 0 && f.Extension == ".mp3")
              {
                if (!_driveFileNames.Contains(f.Name))
                {
                  Mp3File mp3File = new Mp3File(f.FullName);
                  // Add f to _mp3List.
                  _mp3Files.Add(mp3File);
                  Console.WriteLine("\t" + f.FullName + " | " + f.CreationTime);
                }
              }
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }
        // Recursion.
        getFilesRecursive(d.FullName, date);
      }
    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
      Console.WriteLine(e.StackTrace);
    }
  }

  private static string[] _scopes = { DriveService.Scope.DriveReadonly };
  private static string _applicationName = "Drive API .NET Quickstart";

  private static List<string> _driveFiles = new List<string>();

  /// <summary>
  /// Requests user to log in to their Google Account and creates a list of files that are in the Drive.
  /// </summary>
  private static List<string> getDriveFiles()
  {
    // If modifying these scopes, delete your previously saved credentials
    // at ~/.credentials/drive-dotnet-quickstart.json

    UserCredential credential;

    using (var stream =
        new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
    {
      string credPath = System.Environment.GetFolderPath(
          System.Environment.SpecialFolder.Personal);
      credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

      credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
          GoogleClientSecrets.Load(stream).Secrets,
          _scopes,
          "user",
          CancellationToken.None,
          new FileDataStore(credPath, true)).Result;
      Console.WriteLine("Credential file saved to: " + credPath);
    }

    // Create Drive API service.
    var service = new DriveService(new BaseClientService.Initializer()
    {
      HttpClientInitializer = credential,
      ApplicationName = _applicationName,
    });

    // Define parameters of request.
    FilesResource.ListRequest listRequest = service.Files.List();
    listRequest.PageSize = service.Files.List().PageSize;
    listRequest.Fields = "nextPageToken, files(id, name)";

    // List files.
    List<string> fileNames = new List<string>();
    IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
    Console.WriteLine("Files:");
    if (files != null && files.Count > 0)
    {
      foreach (var file in files)
      {
        fileNames.Add(file.Name);
        Console.WriteLine("{0} ({1})", file.Name, file.Id);
      }
    }
    else
    {
      Console.WriteLine("No files found.");
    }
    return fileNames;
  }

  private static void Serialize(List<Mp3File> f)
  {
    XmlSerializer serializer = new XmlSerializer(typeof(List<Mp3File>));
    Stream s = System.IO.File.Create("output.xml");
    serializer.Serialize(s, f);
    s.Close();
    string[] lines = System.IO.File.ReadAllLines("output.xml");
    lines[0] += "<?xml-stylesheet type=\"text / xsl\" href=\"hello.xsl\"?>";
    // TODO: Write all lines back to xml file.
  }

  public static void Main(String[] args)
  {
    _driveFileNames = getDriveFiles();

    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    getFilesRecursive(@"F:\\", DateTime.Now.AddDays(-7));

    Serialize(_mp3Files);

    stopwatch.Stop();
    Console.WriteLine("Process took " + stopwatch.ElapsedMilliseconds.ToString() + " milliseconds");

    Console.ReadKey();
  }
}