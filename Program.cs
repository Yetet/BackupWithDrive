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
  private static List<string> _dirList = new List<string>();

  /// <summary>
  /// Get all files last modified before a certain date in a directory recursively.
  /// </summary>
  /// <param name="dir">Directory to walk through.</param>
  /// <param name="date">The date of which to add files from before.</param>
  private static void GetFilesRecursive(string dir, DateTime date)
  {
    // Try to search through each directory within dir.
    try
    {
      // Foreach directory in dir.
      foreach (string d in Directory.GetDirectories(dir))
      {
        // Filter out not needed files.
        if (!d.Contains("$"))
        {
          // If directory LastWriteTime was before date, then...
          if (DateTime.Compare(Directory.GetLastWriteTime(d), date) < 0)
          {
            Console.WriteLine(d);
            // Foreach file in each directory within dir.
            try
            {
              foreach (string f in Directory.GetFiles(d))
              {
                // If file LastWriteTime was before date, then...
                if (DateTime.Compare(Directory.GetLastWriteTime(f), date) < 0)
                {
                  // Add f to _dirList.
                  _dirList.Add(f);
                  Console.WriteLine("    >" + f);
                }
              }
            }
            // Catch any exception.
            catch (Exception ex)
            {
              Console.WriteLine(ex.Message);
            }
            // Recursion.
            GetFilesRecursive(d, date);
          }
        }
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

  private static void GetDriveFiles()
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
    IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
    Console.WriteLine("Files:");
    if (files != null && files.Count > 0)
    {
      foreach (var file in files)
      {
        Console.WriteLine("{0} ({1})", file.Name, file.Id);
        _driveFiles.Add(file.Name);
      }
    }
    else
    {
      Console.WriteLine("No files found.");
    }
    Console.Read();
  }

  private static bool DoesFileExist(Google.Apis.Drive.v3.Data.File file)
  {

    return false;
  }

  public static void Main(String[] args)
  {
    GetDriveFiles();

    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    GetFilesRecursive(@"C:\\", DateTime.Now);

    stopwatch.Stop();
    Console.WriteLine("Process took " + stopwatch.ElapsedMilliseconds.ToString() + " milliseconds");

    Console.ReadKey();
  }
}