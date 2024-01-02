//Defaults added during class generation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//additional references added for class

//for SFTP connection:
using Renci.SshNet;

//for message box usage:
using System.Windows;

//for file reading and writing
using System.IO;

//for using StringBuilder
using System.Text;

//Add Nuget package - Ssh.Net by Renci

namespace BoilerPlateDraft.Code_Repo
{
	public class SFTPConnection
	{
    //Objects needed for connection to sftp: Host, Port, Username, Password
    private static string host;
    private static int port;
    private static string userName;
    private static string password;

    //client object for SFTP connection
    public static SftpClient client;

    //boolean to track connection status - initial value is false
    private static bool sftpConnected = false;

    //function to connect SFTP using class variables
    public static void ConnectSFTP()
    {
      try
      {
        //create new connection to host
        client = new SftpClient(host, port, userName, password);
        //connect to the host
        client.Connect();
        //set boolean for helper function to determine if sftp is connected
        sftpConnected = true;
      }
      catch (Exception ex)
      {
        //MessageBox.Show("Error - " + ex);
      }
    }

    //function to connect to SFTP using passed variables
    public static void ConnectSFTP(string passed_host, int passed_port, string passed_userName, string passed_password)
    {
      try
      {
        //create new connection to host
        client = new SftpClient(passed_host, passed_port, passed_userName, passed_password);
        //connect to the host
        client.Connect();
        //set boolean for helper function to determine if sftp is connected
        sftpConnected = true;
      }
      catch (Exception ex)
      {
        //MessageBox.Show("Error - " + ex);
      }
    }

    //Disconnect from SFTP host
    public static void DisconnectSFTP()
    {
      try
      {
        client.Disconnect();
        sftpConnected = false;
      }
      catch (Exception ex)
      {
        //MessageBox.Show("Error - " + ex);
      }
    }

    //function to return status of SFTP connection
    public static bool IsSFTPConnected()
    {
      return sftpConnected;
    }

    //function to transfer files over SFTP connection
    //takes arguements filePath for location on host server to transfer file to
    //originalFileLocation for location of file being transfered on client 
    //fileName to label file when transfered to host
    public static void TransferFiles(string filePath, string originalFileLocation, string fileName)
    {
      try
      {
        //change directory to passed filePath location
        client.ChangeDirectory(filePath);

        //transfer file
        using (FileStream fs = new FileStream(originalFileLocation, FileMode.Open))
        {
          client.BufferSize = 4 * 1024;
          client.UploadFile(fs, filePath + fileName);
        }
      }
      catch (Exception ex)
      {
        //MessageBox.Show("Error - " + ex);
      }
    }

    //function to retrieve a file from the server
    //takes argument of filePath for location on file on server
    //newFileLocation for location on client to store file when retrieved
    //and fileName of the file on the host
    //If not needed to return the file info for usage, change from returning StringBuilder to void
    //and remove returnString lines marked if needed
    public static StringBuilder RetrieveFile(string filePath, string newFileLocation, string fileName)
    {
      //StringBuilder to return information
      //remove if not needed to return info
      StringBuilder returnString = new StringBuilder();

      try
      {
        //change directory on host to location passed in arguements
        client.ChangeDirectory(filePath);

        using (FileStream fs = File.Create(newFileLocation))
        {
          client.DownloadFile(filePath + fileName, fs);
        }

        //remove if not needed to return file info 
        returnString.AppendLine(System.IO.File.ReadAllText(newFileLocation));
      }
      catch (Exception ex)
      {
        //MessageBox.Show("Error - " + ex);
      }

      //remove if not needed to return file info
      return returnString;
    }
  }
}